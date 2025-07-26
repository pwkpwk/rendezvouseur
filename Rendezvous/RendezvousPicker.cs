using System.Text;
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
        uint pickedHash = uint.MaxValue;
        
        foreach (var token in commaSeparatedList.Tokenize(','))
        {
            var trimmedToken = token.Trim();
            uint hash = CalculateHash(trimmedToken);
            if (hash <= pickedHash)
            {
                pickedHash = hash;
                pickedToken = trimmedToken;
            }
        }
        
        return pickedToken;

        uint CalculateHash(ReadOnlySpan<char> token)
        {
            Span<byte> buffer = stackalloc byte[token.Length * sizeof(uint)];
            int length = Encoding.UTF8.GetBytes(token, buffer);
            
            return hashCalculator.Calculate((uint)_keyEquality.GetHashCode(key!), buffer.Slice(0, length));
        }
    }
}