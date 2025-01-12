using Moq;
using Xunit;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.Core.Application.IdGenerator;
using ProvidersMS.Core.Application.Storage.Images;
using ProvidersMS.src.Drivers.Application.Commands.UploadImagesDocuments;
using ProvidersMS.src.Drivers.Application.Commands.UploadImagesDocuments.Types;
using ProvidersMS.Core.Application.Storage.Images.Types;
using ProvidersMS.src.Images.Application.Exceptions;

namespace ProvidersMS.Tests.Drivers.Application.Commands.UploadImagesDocuments
{
    public class UploadImagesDocumentsCommandHandlerTests
    {
        private readonly Mock<IdGenerator<string>> _idGeneratorMock;
        private readonly Mock<IImageStorage> _imageStorageMock;
        private readonly UploadImagesDocumentsCommandHandler _handler;

        public UploadImagesDocumentsCommandHandlerTests()
        {
            _idGeneratorMock = new Mock<IdGenerator<string>>();
            _imageStorageMock = new Mock<IImageStorage>();
            _handler = new UploadImagesDocumentsCommandHandler(_imageStorageMock.Object);
        }

        private static IFormFile CreateMockFormFile(string fileName)
        {
            var fileMock = new Mock<IFormFile>();
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write("Dummy content");
            writer.Flush();
            ms.Position = 0;

            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), default)).Returns((Stream stream, CancellationToken token) =>
            {
                ms.CopyTo(stream);
                return Task.CompletedTask;
            });

            return fileMock.Object;
        }

        [Fact]
        public async Task ShouldUploadImagesSuccessfully()
        {
            var command = new UploadImageCommand(
                CreateMockFormFile("license.jpg"),
                CreateMockFormFile("dni.jpg"),
                CreateMockFormFile("medical.jpg"),
                CreateMockFormFile("liability.jpg")
            );

            _imageStorageMock.Setup(x => x.Save(It.IsAny<string>())).ReturnsAsync(Result<ImageSaved>.Success(new ImageSaved { Url = "http://example.com/image.jpg" }));

            var result = await _handler.Execute(command);

            Assert.True(result.IsSuccessful);
            Assert.Equal(4, result.Unwrap().Urls.Count);
            _imageStorageMock.Verify(x => x.Save(It.IsAny<string>()), Times.Exactly(4));
        }

        [Fact]
        public async Task ShouldReturnFailure_WhenImageStorageFails()
        {
            var command = new UploadImageCommand(
                CreateMockFormFile("license.jpg"),
                null,
                null,
                null
            );

            _imageStorageMock.Setup(x => x.Save(It.IsAny<string>())).ReturnsAsync(Result<ImageSaved>.Failure(new UploadImageException()));

            var result = await _handler.Execute(command);

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal("There was an error saving the image.", result.ErrorMessage);
            _imageStorageMock.Verify(x => x.Save(It.IsAny<string>()), Times.Once);
        }
    }
}