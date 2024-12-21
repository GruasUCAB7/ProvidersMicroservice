using CloudinaryDotNet;
using DotNetEnv;

namespace ProvidersMS.Core.Infrastructure.Storage
{
    public static class CloudinaryConfiguration
    {
        public static Cloudinary ConfigureCloudinary()
        {
            Env.Load();

            var cloudinarySettings = new CloudinarySettings
            {
                CloudName = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME"),
                ApiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY"),
                ApiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET")
            };

            return new Cloudinary(new Account(
                cloudinarySettings.CloudName,
                cloudinarySettings.ApiKey,
                cloudinarySettings.ApiSecret
            ));
        }
    }
}
