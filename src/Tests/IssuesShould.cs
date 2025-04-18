using System.Collections.Generic;
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
    public void GitIgnore_IsMissing()
    {
        var sut = new SolutionCheckerBuilder()
            .Create();
        Issue[] issues = sut.Check(new GitIgnore());

        Assert.All(issues,
            i =>
            {
                Assert.Contains("missing .gitignore", i.Message);
            });
    }

     [Fact]
    public void GitIgnore_IsEmpty()
    {
        var sut = new SolutionCheckerBuilder()
            .AddTextFile(".gitignore", "")
            .Create();
        Issue[] issues = sut.Check(new GitIgnore());

        Assert.All(issues,
            i =>
            {
                Assert.Contains("empty .gitignore", i.Message);
            });
    }
    private Issue[] CheckChecker(ICheck check)
    {
        var subjectPath = GetTestSubjectPath();
        ISolutionChecker solutionChecker = new SolutionChecker(subjectPath);

        var issues = solutionChecker.Check(check);
        return issues;
    }

    private string GetTestSubjectPath([CallerFilePath] string? caller = null)
    {
        var dirName = Path.GetDirectoryName(caller);
        return Path.GetFullPath(Path.Combine(dirName, "../..", "test.subject"));
    }
}

public class SolutionCheckerBuilder
{
    private Dictionary<string, TextFile> _files = [];
    public ISolutionChecker Create()
    {
        return new FakeSolutionChecker(_files);
    }

    internal SolutionCheckerBuilder AddTextFile(string path, string content)
    {
        _files.Add(path, new TextFile(true, content));
        return this;
    }
}

internal class FakeSolutionChecker : ISolutionChecker
{
    private Dictionary<string, TextFile> files;

    public FakeSolutionChecker(Dictionary<string, TextFile> files)
    {
        this.files = files;
    }

    public IEnumerable<Project> Projects => throw new NotImplementedException();

    public TextFile GetTextFile(string path)
    {
        if (files.TryGetValue(path, out var textFile))
        {
            return textFile;
        }
        return new TextFile(false, null);
    }
}