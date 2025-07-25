namespace Rendezvous;

public class FowlerNollVoPickerHash32Bit : IPickerHash
{
    private const int FnvPrime = 16777619;
    private const int OffsetBasis = unchecked((int)2166136261);

    int IPickerHash.CalculateHash(int keyHash, ReadOnlySpan<char> token)
    {
        // 32-bit FNV-1a hashing algorithm
        // https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
        int hash = OffsetBasis;
        var trimmedToken = token.Trim();
            
        foreach (char c in trimmedToken)
        {
            hash ^= c;
            hash *= FnvPrime;
        }
            
        hash ^= keyHash;
        return hash * FnvPrime;
    }
}