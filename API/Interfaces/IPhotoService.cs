using System;
using CloudinaryDotNet.Actions;

namespace API.Interfaces;

public interface IPhotoService
{
    // ImageUploadResult is a class from CloudinaryDotNet.Actions representing the result of an image upload to cloudinary
    // IFormFile is an interface from Microsoft.AspNetCore.Http representing a file sent with the HttpRequest (the image file to be uploaded)
    Task<ImageUploadResult> UploadPhotoAsync(IFormFile file);
    Task<DeletionResult> DeletePhotoAsync(string publicId);
}
