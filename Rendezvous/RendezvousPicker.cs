using CommunityToolkit.HighPerformance;

namespace Rendezvous;

public class RendezvousPicker<TKey>(
    IPickerHash hashCalculator,
    IEqualityComparer<TKey>? keyEquality = null) : IRendezvousPicker<TKey>
{
    private readonly IEqualityComparer<TKey> _keyEquality = keyEquality ?? EqualityComparer<TKey>.Default;
    
    public ReadOnlySpan<char> Pick(ReadOnlySpan<char> commaSeparatedList, TKey key)
    {
        ReadOnlySpan<char> pickedToken = commaSeparatedList;
        int pickedHash = int.MaxValue;
        
        foreach (var token in commaSeparatedList.Tokenize(','))
        {
            var trimmedToken = token.Trim();
            int hash = hashCalculator.CalculateHash(_keyEquality.GetHashCode(key!), trimmedToken);
            if (hash < pickedHash)
            {
                pickedHash = hash;
                pickedToken = trimmedToken;
            }
        }
        
        return pickedToken;
    }
}