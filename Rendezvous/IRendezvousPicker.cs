namespace Rendezvous;

public interface IRendezvousPicker<in TKey>
{
    /// <summary>
    /// Pick one of the comma-separated tokens from the span by Rendezvous hashing with the specified key.
    /// </summary>
    /// <param name="tokens">Comma-separated list of tokens</param>
    /// <param name="key">Key used to consistently pick a random token from the span</param>
    /// <returns>One of the tokens from the span array picked randomly but consistently for the key</returns>
    /// <remarks>Order of tokens in the span does not matter; the algorithm always picks the same token
    /// for the same value of the key.</remarks>
    ReadOnlySpan<char> Pick(ReadOnlySpan<char> tokens, TKey key);

    /// Same as <see cref="Pick(System.ReadOnlySpan{char},TKey)"/> but takes tokens as a string
    sealed ReadOnlySpan<char> Pick(string tokens, TKey key) => Pick(tokens.AsSpan(), key);

    /// <summary>
    /// Pick one of the names from an array by Rendezvous hashing with the specified key.
    /// </summary>
    /// <param name="names">Non-empty array of names</param>
    /// <param name="key">Key used to consistently pick a random name from the array</param>
    /// <returns>One of the strings from the names array picked randomly but consistently for the key</returns>
    /// <exception cref="ArgumentException">Thrown if the array of names is empty</exception>
    /// <remarks>Order of strings in the array does not matter; the algorithm always picks the same string
    /// for the same value of the key.</remarks>
    string Pick(string[] names, TKey key);
}