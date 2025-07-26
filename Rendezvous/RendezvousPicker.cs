using System.Text;
using CommunityToolkit.HighPerformance;

namespace Rendezvous;

public class RendezvousPicker<TKey>(
    IPickerHash hashCalculator,
    IEqualityComparer<TKey>? keyEquality = null) : IRendezvousPicker<TKey>
{
    private readonly IEqualityComparer<TKey> _keyEquality = keyEquality ?? EqualityComparer<TKey>.Default;
    
    public ReadOnlySpan<char> Pick(ReadOnlySpan<char> tokens, TKey key)
    {
        ReadOnlySpan<char> pickedToken = tokens;
        uint pickedHash = uint.MaxValue;
        bool picked = false;
        
        foreach (var token in tokens.Tokenize(','))
        {
            var trimmedToken = token.Trim();

            if (trimmedToken.Length != 0)
            {
                uint hash = CalculateHash(trimmedToken, key);
                if (!picked || hash < pickedHash)
                {
                    picked = true;
                    pickedHash = hash;
                    pickedToken = trimmedToken;
                }
            }
        }
        
        return picked ? pickedToken : throw new ArgumentException("No tokens in the CSV",  nameof(tokens));
    }

    string IRendezvousPicker<TKey>.Pick(string[] names, TKey key)
    {
        if (names.Length == 1)
        {
            return names[0];
        }
        
        string? pickedName = null;
        uint pickedHash = uint.MaxValue;

        foreach (string name in names)
        {
            uint hash = CalculateHash(name.AsSpan(), key);

            if (pickedName is null || hash < pickedHash)
            {
                pickedName = name;
                pickedHash = hash;
            }
        }

        return pickedName ?? throw new ArgumentException("Sequence cannot be empty", nameof(names));
    }

    private uint CalculateHash(ReadOnlySpan<char> token, TKey key)
    {
        Span<byte> buffer = stackalloc byte[token.Length * sizeof(uint)];
        int length = Encoding.UTF8.GetBytes(token, buffer);
            
        return hashCalculator.Calculate((uint)_keyEquality.GetHashCode(key!), buffer.Slice(0, length));
    }
}