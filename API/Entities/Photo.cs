using System;

namespace API.Entities;

public class Photo
{
    public int Id { get; set; }
    public required string Url { get; set; } // the url to the image itself
    public string? PublicId { get; set; } // related to cloud part

    // Nav Props
    // as in facebook, if you post photo, this photo belong to only one member, it may be in post, message ...etc.
    public Member Member { get; set; } = null!; 
    public int MemberId { get; set; }
}
