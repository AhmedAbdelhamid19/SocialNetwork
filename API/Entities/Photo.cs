using System;

namespace API.Entities;

public class Photo
{
    public int Id { get; set; }
    public required string Url { get; set; } // the url to the image itself
    public string? PublicId { get; set; } // related to cloud part

    // navigation property

    // as in facebook, if you post photo, this photo belong to only one
    // it maybe overkill if you made relation one to many, because you don't need to it, you only need relation 
    // in member to it's only profile image, but photo, it may be in post, message ...etc.
    public Member member { get; set; } = null!; 
}
