using Backend.Services.Code;

namespace Backend.Endpoints;

public static class CodeEndpoints
{
    public static void MapCodeEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/code");

        group.MapPost("/run", RunSourceCode);
    }

    private static IResult RunSourceCode(RunService.RunRequest request)
    {
        return RunService.RunCode(request);
    }
}
