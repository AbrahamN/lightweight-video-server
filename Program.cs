using System.Linq;

var builder = WebApplication.CreateBuilder(args);
args.ToList().ForEach(arg => System.Diagnostics.Debug.WriteLine(arg));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Video Server API",
        Description = "An API for serving video files, could serve any files really... Also very insecure ;)"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

/// <summary>
/// Lists a video (any file)
/// Defaults to mp4 if nothing is supplied
/// Looks in current directory 
/// </summary>
app.MapGet("/list-video", (string? fileTypes, string? location) =>
{
    fileTypes = fileTypes ?? "mp4";
    var destinationFileTypes = fileTypes.Split(',');
    var destinationLocation = location ?? ".";
    var matchingFiles = destinationFileTypes.SelectMany(fileType => Directory.GetFiles(destinationLocation, $"*.{fileType}"));
    return matchingFiles;
})
    .WithMetadata(new Swashbuckle.AspNetCore.Annotations.SwaggerOperationAttribute(
        summary: "Lists a video file (any file)", 
        description: "Lists files matching extension in specified directory. If nothing supplied, defaults to mp4 and current directory"));

app.MapGet("/view-video", (string? filename) =>
{
    var linkedFile = filename ?? Directory.GetFiles(".").First();
    var fileStream = File.OpenRead(linkedFile);
    return Results.File(fileStream, enableRangeProcessing: true);
})
    .WithMetadata(new Swashbuckle.AspNetCore.Annotations.SwaggerOperationAttribute(
    summary: "Returns a video file (any file)",
    description: "Returns the file supplied by the filename"));


app.Run();
