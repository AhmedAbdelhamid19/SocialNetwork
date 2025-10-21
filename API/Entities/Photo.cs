using System;
using System.Text.Json.Serialization;
namespace API.Entities;

public class Photo
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public string? PublicId { get; set; }


    [JsonIgnore]
    public int MemberId { get; set; }
    public Member Member { get; set; } = null!; 
}