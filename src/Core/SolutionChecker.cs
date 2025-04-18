using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Core;

public class SolutionChecker(string subjectPath) : ISolutionChecker
{
    internal readonly string SolutionDir = subjectPath;
    private Lazy<Project[]> projects = new Lazy<Project[]>(() =>
    {
        var projects = Directory.GetFiles(subjectPath, "*.csproj", SearchOption.AllDirectories)
        .Select(x => new Project(x));
        return projects.ToArray();
    });

    public IEnumerable<Project> Projects => projects.Value;    
    
    public TextFile GetTextFile(string v)
    {
        var gitIgnorePath = Path.Combine(SolutionDir, v);

        bool exists = File.Exists(gitIgnorePath);
        return new TextFile(
exists,
exists ? File.ReadAllText(gitIgnorePath) : null
        );

    }
}

public interface ISolutionChecker
{
    public Issue[] Check(ICheck check)
    {
        return check.Issues(this).ToBlockingEnumerable().ToArray();
    }
    public IEnumerable<Project> Projects { get; }

    TextFile GetTextFile(string v);
}

public class Project
{
    private string v;

    public Project(string v)
    {
        this.v = v;
    }
}

public class OldVersion : ICheck
{

    public async IAsyncEnumerable<Issue> Issues(ISolutionChecker solutionChecker)
    {
        yield return new Issue
        {
            Message = "The project file is using unsupported version net6.0"
        };
    }
}

public class GitIgnore : ICheck
{
    public async IAsyncEnumerable<Issue> Issues(ISolutionChecker solutionChecker)
    {
        var solution = "Suggest to do a curl https://www.toptal.com/developers/gitignore?templates=macos,rider,windows,dotnetcore,visualstudio > .gitignore";
        var gitIgnore = solutionChecker.GetTextFile(".gitignore");
        if (gitIgnore.Exists)
        {
            var content = gitIgnore.Content;
            if (string.IsNullOrWhiteSpace(content))
            {
                yield return new Issue
                {
                    Message = $"""
                    empty .gitignore
                    {solution}
                    """,
                    Solution = solution
                };
            }
        }
        else
        {
            yield return new Issue
            {
                Message = $"""
                missing .gitignore"
                {solution}
                """,
                Solution = solution
            };
        }
    }
}

public interface ICheck
{
    public IAsyncEnumerable<Issue> Issues(ISolutionChecker solutionChecker);
}

public class Issue
{
    public string Message { get; internal set; }
    public string Solution { get; internal set; }
}

public record TextFile(
// [MemberNotNullWhen(true, nameof(TextFile.Content))]
bool Exists,
string? Content);
