using Backend.Services.Code;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Endpoints;

public static class CodeEndpoints
{
    public static void MapCodeEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/code");

        group.MapPost("/run", RunCode);
        group.MapGet("/snippet/{snippetId:int}", GetSnippetInfo);
    }

    private static async Task<IResult> RunCode([FromBody] RunService.RunRequest request, Database.AppDbContext dbContext)
    {
        return await RunService.RunCode(request, dbContext);
    }

    private static async Task<IResult> GetSnippetInfo([FromRoute] int snippetId, Database.AppDbContext dbContext)
    {
        return await SnippetInfoService.GetSnippetInfo(snippetId, dbContext);
    }
}
