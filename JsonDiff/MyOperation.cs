namespace JsonDiff;

public class MyOperation
{
    public MyOperation()
    {
    }

    public MyOperation(string op, string path, string from)
    {
        Op = op;
        Path = path;
        From = from;
    }

    public MyOperation(string op, string path, string from, object oldValue, object value)
    {
        Op = op;
        Path = path;
        From = from;
        OldValue = oldValue;
        Value = value;
    }

    public string Path { get; }

    public string Op { get; }

    public string From { get; }

    public object OldValue { get; }

    public object Value { get; }
}