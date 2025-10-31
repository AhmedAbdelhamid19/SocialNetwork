using System;
using System.Text.Json.Serialization;
namespace API.Entities;

public class Photo
{
    public int Id { get; set; }

    // the difference between Url and PublicId is that Url is the link to access the photo,
    // while PublicId is the identifier used by Cloudinary to manage the photo (like deleting it)
    public required string Url { get; set; }
    public string? PublicId { get; set; }


    [JsonIgnore]
    public int MemberId { get; set; }
    public Member Member { get; set; } = null!; 
}