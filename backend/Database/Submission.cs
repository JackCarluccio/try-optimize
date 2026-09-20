namespace Database;

public class Submission
{
    public int Id { get; set; }
    public required string SourceCode { get; set; }
    public double Runtime { get; set; }

    public int SnippetId { get; set; }
    public Snippet Snippet { get; set; } = null!;
}
