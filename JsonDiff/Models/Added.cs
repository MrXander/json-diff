namespace JsonDiff.Models;

public class Added
{
    public string Path { get; }
    public string Value { get; }

    public Added(string path, string value)
    {
        Path = path;
        Value = value;
    }
}