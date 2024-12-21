using Xunit;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Cranes.Domain.Exceptions;
using ProvidersMS.src.Cranes.Domain.Events;

namespace ProvidersMS.Tests.Crane.Domain
{
    public class CraneTests
    {
        [Fact]
        public void CreateCraneWithCorrectValues()
        {
            var craneId = new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");
            var craneBrand = new CraneBrand("BrandA");
            var craneModel = new CraneModel("ModelX");
            var cranePlate = new CranePlate("AB123CD");
            var craneType = CraneSizeType.Mediana;
            var craneYear = new CraneYear(2012);

            var crane = src.Cranes.Domain.Crane.CreateCrane(craneId, craneBrand, craneModel, cranePlate, craneType, craneYear);

            Assert.Equal(craneId.GetValue(), crane.GetId());
            Assert.Equal(craneBrand.GetValue(), crane.GetBrand());
            Assert.Equal(craneModel.GetValue(), crane.GetModel());
            Assert.Equal(cranePlate.GetValue(), crane.GetPlate());
            Assert.Equal(craneType.ToString(), crane.GetCraneType());
            Assert.Equal(craneYear.GetValue(), crane.GetYear());
            Assert.True(crane.GetIsActive());
        }

        [Fact]
        public void OnCraneCreatedEvent_ShouldUpdateProperties()
        {
            var craneId = new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");
            var craneBrand = new CraneBrand("BrandA");
            var craneModel = new CraneModel("ModelX");
            var cranePlate = new CranePlate("AB123CD");
            var craneType = CraneSizeType.Mediana;
            var craneYear = new CraneYear(2012);

            var crane = src.Cranes.Domain.Crane.CreateCrane(craneId, craneBrand, craneModel, cranePlate, craneType, craneYear);

            var craneCreatedEvent = new CraneCreated(
                craneId.GetValue(),
                craneBrand.GetValue(),
                craneModel.GetValue(),
                cranePlate.GetValue(),
                craneType.ToString(),
                craneYear.GetValue()
            );

            crane.OnCraneCreatedEvent(craneCreatedEvent);

            Assert.Equal(craneId.GetValue(), crane.GetId());
            Assert.Equal(craneBrand.GetValue(), crane.GetBrand());
            Assert.Equal(craneModel.GetValue(), crane.GetModel());
            Assert.Equal(cranePlate.GetValue(), crane.GetPlate());
            Assert.Equal(craneType.ToString(), crane.GetCraneType());
            Assert.Equal(craneYear.GetValue(), crane.GetYear());
        }

        [Fact]
        public void OnCraneUpdatedEvent_ShouldUpdateIsActive()
        {
            var craneId = new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");
            var craneBrand = new CraneBrand("BrandA");
            var craneModel = new CraneModel("ModelX");
            var cranePlate = new CranePlate("AB123CD");
            var craneType = CraneSizeType.Mediana;
            var craneYear = new CraneYear(2012);

            var crane = src.Cranes.Domain.Crane.CreateCrane(craneId, craneBrand, craneModel, cranePlate, craneType, craneYear);

            var craneUpdatedEvent = new CraneUpdated(craneId.GetValue(), false);

            crane.OnCraneUpdatedEvent(craneUpdatedEvent);

            Assert.Equal(craneId.GetValue(), crane.GetId());
            Assert.False(crane.GetIsActive());
        }


        //[Fact]
        //public void CreateCraneWithIncorrectValues()
        //{
        //    var craneId = new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");
        //    var craneBrand = new CraneBrand("");
        //    var craneModel = new CraneModel("ModelX");
        //    var cranePlate = new CranePlate("AB123CDGT");
        //    var craneType = CraneSizeType.Medium;
        //    var craneYear = new CraneYear(2012);

        //    var crane = src.Cranes.Domain.Crane.CreateCrane(craneId, craneBrand, craneModel, cranePlate, craneType, craneYear);

        //    Assert.Throws<InvalidCraneBrandException>(() => src.Cranes.Domain.Crane.CreateCrane(craneId, craneBrand, craneModel, cranePlate, craneType, craneYear));
        //    Assert.Throws<InvalidCranePlateException>(() => src.Cranes.Domain.Crane.CreateCrane(craneId, craneBrand, craneModel, cranePlate, craneType, craneYear));
        //}
    }
}
