using System.Threading.Tasks;

namespace Core;

public class SolutionChecker(string subjectPath)
{
    internal readonly string SolutionDir = subjectPath;
    private Lazy<Project[]> projects = new Lazy<Project[]>(() =>
    {
        var projects = Directory.GetFiles(subjectPath, "*.csproj", SearchOption.AllDirectories)
        .Select(x => new Project(x));
        return projects.ToArray();
    });

    public IEnumerable<Project> Projects => projects.Value;

    public Issue[] Check(ICheck check)
    {
        return check.Issues(this).ToBlockingEnumerable().ToArray();
    }
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

    public async IAsyncEnumerable<Issue> Issues(SolutionChecker solutionChecker)
    {
        yield return new Issue
        {
            Message = "The project file is using unsupported version net6.0"
        };
    }
}

public class GitIgnore : ICheck
{
    public async IAsyncEnumerable<Issue> Issues(SolutionChecker solutionChecker)
    {
        var solution = "Suggest to do a curl https://www.toptal.com/developers/gitignore?templates=macos,rider,windows,dotnetcore,visualstudio > .gitignore";
        var gitIgnorePath = Path.Combine(solutionChecker.SolutionDir, ".gitignore");
        if (File.Exists(gitIgnorePath))
        {
            var content = File.ReadAllText(gitIgnorePath);
            if (string.IsNullOrWhiteSpace(content))
            {
                yield return new Issue
                {
                    Message = $"""
                    empty .gitignore
                    {solution}
                    """
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
                """
            };
        }
    }
}

public interface ICheck
{
    public IAsyncEnumerable<Issue> Issues(SolutionChecker solutionChecker);
}

public class Issue
{
    public string Message { get; internal set; }
}