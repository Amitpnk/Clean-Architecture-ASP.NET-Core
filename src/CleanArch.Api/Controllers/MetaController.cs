using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CleanArch.Api.Controllers;

[ApiController]
[ApiVersionNeutral]
public class MetaController : ControllerBase
{
    [HttpGet("/info")]
    public ActionResult<string> Info()
    {
        var assembly = typeof(Program).Assembly;

        var lastUpdate = System.IO.File.GetLastWriteTime(assembly.Location);
        var version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

        return Ok($"Version: {version}, Last Updated: {lastUpdate}");
    }
}