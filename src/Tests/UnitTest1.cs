using System.Runtime.CompilerServices;
using Core;

namespace Tests;

public class UnitTest1
{

    [Fact]
    public void OldVersion()
    {
        Issue[] issues = NewMethod();

        Assert.All(issues,
            i =>
            {
                Assert.Contains("using unsupported version ", i.Message);
            });
    }

    [Fact]
    public void GitIgnore()
    {
        var subjectPath = GetTestSubjectPath();
        var solutionChecker = new SolutionChecker(subjectPath);

        var issues = solutionChecker.Check(new GitIgnore());

        Assert.All(issues,
            i =>
            {
                Assert.Contains("missing .gitignore", i.Message);
            });
    }
    private Issue[] NewMethod()
    {
        var subjectPath = GetTestSubjectPath();
        var solutionChecker = new SolutionChecker(subjectPath);

        var issues = solutionChecker.Check(new OldVersion());
        return issues;
    }

    private string GetTestSubjectPath([CallerFilePath] string? caller = null)
    {
        var dirName = Path.GetDirectoryName(caller);
        return Path.GetFullPath(Path.Combine(dirName, "../..", "test.subject"));
    }
}