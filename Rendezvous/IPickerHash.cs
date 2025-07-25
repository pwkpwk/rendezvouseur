namespace Rendezvous;

public interface IPickerHash
{
    int CalculateHash(int keyHash, ReadOnlySpan<char> token);
}