using System.Runtime.CompilerServices;
using Core;

namespace Tests;

public class IssuesShould
{

    [Fact]
    public void OldVersion()
    {
        Issue[] issues = CheckChecker(new OldVersion());

        Assert.All(issues,
            i =>
            {
                Assert.Contains("using unsupported version ", i.Message);
            });
    }

    [Fact]
    public void GitIgnore()
    {
        Issue[] issues = CheckChecker(new OldVersion());

        Assert.All(issues,
            i =>
            {
                Assert.Contains("missing .gitignore", i.Message);
            });
    }
    private Issue[] CheckChecker(ICheck check)
    {
        var subjectPath = GetTestSubjectPath();
        var solutionChecker = new SolutionChecker(subjectPath);

        var issues = solutionChecker.Check(check);
        return issues;
    }

    private string GetTestSubjectPath([CallerFilePath] string? caller = null)
    {
        var dirName = Path.GetDirectoryName(caller);
        return Path.GetFullPath(Path.Combine(dirName, "../..", "test.subject"));
    }
}