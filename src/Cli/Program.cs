using Core;

var currentDirectory = Directory.GetCurrentDirectory();
var solutionChecker = new SolutionChecker(currentDirectory);

// Run all available checks
var checks = new ICheck[]
{
    new GitIgnore(),
    new OldVersion()
};

foreach (var check in checks)
{
    var issues = solutionChecker.Check(check);
    foreach (var issue in issues)
    {
        Console.WriteLine(issue.Message);
    }
}