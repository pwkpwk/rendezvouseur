namespace Rendezvous;

public interface IRendezvousPicker<in TKey>
{
    ReadOnlySpan<char> Pick(ReadOnlySpan<char> commaSeparatedList, TKey key);

    sealed ReadOnlySpan<char> Pick(string commaSeparatedList, TKey key) =>
        Pick(commaSeparatedList.AsSpan(), key);
}