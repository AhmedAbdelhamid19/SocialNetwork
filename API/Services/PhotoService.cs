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
    // CloudinarySettings is a custom class that holds Cloudinary configuration like CloudName, ApiKey, and ApiSecret you already made
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
        /* 
            what happen here step by step:
            1. check if the file length is greater than 0 (meaning the file is not empty)
            2. open a read stream to read the file content
            3. create upload parameters including the file description and transformation settings
                - you create a FileDescription object with the file name and the stream, this means that Cloudinary will read the file content from the stream
                - you create a Transformation object to specify how the image should be processed (resized to 500x500 pixels, cropped to fill the dimensions, and focused on the face area)
            4. upload the file to Cloudinary using the upload parameters
        */
        var uploadResult = new ImageUploadResult();

        if (file.Length > 0)
        {
            // using keyword ensures that the stream is distroyed after the scope to free up resources
            // OpenReadStream is a method from IFormFile that opens a stream to read the file content
            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                // FileDescription is a class from CloudinaryDotNet.Actions that describes the file to be uploaded
                File = new FileDescription(file.FileName, stream),
                // this transformation will resize the image to 500x500 pixels, crop it to fill the dimensions, and focus on the face area
                // face area means that if a face is detected in the image, it will be centered in the cropped area
                Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                // specify a folder in cloudinary to store the images, because in cloudinary all images are stored in a flat structure by default
                // to seperate every image of projects in spcific file because you may store many images from different resources
                Folder = "socialnetwork"
            };

            // UploadAsync is a method from CloudinaryDotNet that uploads the file to Cloudinary asynchronously and return publicId, url, etc
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult;
    }
}
