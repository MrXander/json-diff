namespace JsonDiff.Models;

public class HumanReadableDiffResult
{
    public IReadOnlyCollection<Added> Added { get; }
    public IReadOnlyCollection<Changed> Changed { get; }
    public IReadOnlyCollection<Removed> Removed { get; }

    public HumanReadableDiffResult(IReadOnlyCollection<Added> added,
        IReadOnlyCollection<Changed> changed,
        IReadOnlyCollection<Removed> removed)
    {
        Added = added;
        Changed = changed;
        Removed = removed;
    }

    public HumanReadableDiffResult()
    {
        Added = Array.Empty<Added>();
        Changed = Array.Empty<Changed>();
        Removed = Array.Empty<Removed>();
    }
}