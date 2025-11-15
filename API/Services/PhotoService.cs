using System;
using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Services;

public class PhotoService : IPhotoService
{
    // Cloudinary is a class from CloudinaryDotNet library that provides methods to interact with Cloudinary API
    private readonly Cloudinary _cloudinary;

    // IOptions<T> is a way to retrieve configuration settings in ASP.NET Core
    // CloudinarySettings is a custom class that holds Cloudinary configuration like CloudName, 
    // ApiKey, and ApiSecret you already made
    // in program.cs we configured these settings to be read from appsettings.json
    public PhotoService(IOptions<CloudinarySettings> config)
    {
        // Account is a class from CloudinaryDotNet library that holds the Cloudinary account credentials
        var account = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );
        // initialize the Cloudinary instance with the account credentials
        _cloudinary = new Cloudinary(account);
    }
    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        return await _cloudinary.DestroyAsync(deleteParams);
    }
    public async Task<ImageUploadResult> UploadPhotoAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();

        if (file.Length > 0)
        {
            // using keyword ensures that the stream is distroyed after the scope to free up resources
            // OpenReadStream is a method from IFormFile that opens a stream to read the file content
            await using var stream = file.OpenReadStream();
            // make required parameters for uploading the image to cloudinary
            var uploadParams = new ImageUploadParams
            {
                // FileDescription is a class from CloudinaryDotNet.Actions that describes the file to be uploaded
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                Folder = "socialnetwork"
            };

            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult;
    }
}
