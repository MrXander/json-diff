namespace JsonDiff.Models;

public class Removed
{
    public string Path { get; }
    public string Value { get; }

    public Removed(string path, string value)
    {
        Path = path;
        Value = value;
    }
}