namespace JsonDiff.Models;

public class Changed
{
    public string Path { get; }
    public string Value { get; }
    public string NewValue { get; }

    public Changed(string path, string value, string newValue)
    {
        Path = path;
        Value = value;
        NewValue = newValue;
    }
}