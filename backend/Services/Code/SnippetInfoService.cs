using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public static class SnippetInfoService
{
    public class SnipperInfoResponse
    {
        public required string SourceCode { get; init; }
    }

    public static async Task<IResult> GetSnippetInfo([FromRoute] int snippetId, Database.AppDbContext dbContext)
    {
        var sourceCode = await dbContext.Snippets
            .Where(s => s.Id == snippetId)
            .Select(s => s.SourceCode)
            .FirstOrDefaultAsync();
        if (sourceCode == null)
        {
            return Results.NotFound($"Snippet with ID {snippetId} not found.");
        }

        var response = new SnipperInfoResponse
        {
            SourceCode = sourceCode
        };
        return Results.Ok(response);
    }
}
