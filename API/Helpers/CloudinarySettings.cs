using System;
using System.Data.SqlTypes; 

namespace API.Helpers;

/*
    - Cloudinary settings mapped from appsettings.json
    - the names of the properties must match the keys in appsettings.json
    - it's used to work in strong type way with configuration settings
    - you should add in program.cs:
        builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloundinarySettings"));
        to bind the settings from appsettings.json to this class
*/
public class CloudinarySettings
{
    public required string CloudName { get; set; }
    public required string ApiKey { get; set; }
    public required string ApiSecret { get; set; }
}
