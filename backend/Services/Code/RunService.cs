using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Database;

namespace Backend.Services.Code;

public static class RunService
{

    private const string TempSourceFile = "temp_program.cc";
    private const string TempExecutableFile = "temp_program";

    public record RunRequest
    {
        public int SnippetId { get; init; }
        public required string SourceCode { get; init; }
    }

    public record RunResponse
    {
        public double Runtime { get; init; }
        public required string StandardOutput { get; init; }
        public required string StandardError { get; init; }
    }

    private class FailedToStartProcessException : Exception
    {
        public FailedToStartProcessException() {}
        public FailedToStartProcessException(string message) : base(message) {}
        public FailedToStartProcessException(string message, Exception inner) : base(message, inner) {}
    }

    private class CouldNotCompileException : Exception
    {
        public CouldNotCompileException() {}
        public CouldNotCompileException(string message) : base(message) {}
        public CouldNotCompileException(string message, Exception inner) : base(message, inner) {}
    }

    public async static Task<IResult> RunCode([FromBody] RunRequest request, AppDbContext dbContext)
    {
        var snippetExists = await dbContext.Snippets.AnyAsync(s => s.Id == request.SnippetId);
        if (!snippetExists)
        {
            return Results.BadRequest($"Invalid submission: Snippet ID {request.SnippetId} does not exist.");
        }

        try
        {
            Compile(request);
        }
        catch (CouldNotCompileException e)
        {
            DeleteSourceFile();
            return Results.Ok($"Compilation error: {e.Message}");
        }
        catch (Exception e)
        {
            DeleteSourceFile();
            return Results.Problem($"Failed to compile: {e.Message}");
        }

        IResult result;

        try
        {
            var runResult = Run(request);

            var newSubmission = new Submission
            {
                SnippetId = request.SnippetId,
                SourceCode = request.SourceCode,
                Runtime = runResult.Runtime
            };

            dbContext.Submissions.Add(newSubmission);
            await dbContext.SaveChangesAsync();

            result = Results.Ok(new RunResponse
            {
                Runtime = runResult.Runtime,
                StandardOutput = runResult.StandardOutput,
                StandardError = runResult.StandardError,
            });
        }
        catch (FailedToStartProcessException e)
        {
            result = Results.Problem($"Failed to start execution: {e.Message}");
        }
        catch (Exception e)
        {
            result = Results.Problem($"Error during execution: {e.Message}");
        }

        DeleteFiles();
        return result;
    }

    private static void Compile(RunRequest request)
    {
        File.WriteAllText(TempSourceFile, request.SourceCode);

        const string command = $"g++ -o {TempExecutableFile} {TempSourceFile}";
        var startInfo = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"{command}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        if (process == null)
        {
            throw new FailedToStartProcessException("Error starting compilation process.");
        }

        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            throw new CouldNotCompileException(process.StandardError.ReadToEnd());
        }
    }


    private static RunResponse Run(RunRequest request)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = $"./{TempExecutableFile}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        if (process == null)
        {
            throw new FailedToStartProcessException("Error starting execution process.");
        }

        var stopwatch = Stopwatch.StartNew();

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();
        Task.WaitAll(outputTask, errorTask);

        process.WaitForExit();
        stopwatch.Stop();

        return new RunResponse
        {
            Runtime = stopwatch.Elapsed.TotalMilliseconds,
            StandardOutput = outputTask.Result,
            StandardError = errorTask.Result
        };
    }

    private static void DeleteFiles()
    {
        DeleteSourceFile();
        DeleteExecutableFile();
    }

    private static void DeleteSourceFile()
    {
        try
        {
            if (File.Exists(TempSourceFile))
            {
                File.Delete(TempSourceFile);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error deleting temporary source file: {e.Message}");
        }
    }

    private static void DeleteExecutableFile()
    {
        try
        {
            if (File.Exists(TempExecutableFile))
            {
                File.Delete(TempExecutableFile);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error deleting temporary executable file: {e.Message}");
        }
    }
}
