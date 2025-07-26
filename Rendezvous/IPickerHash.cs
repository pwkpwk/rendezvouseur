namespace Rendezvous;

public interface IPickerHash
{
    uint Calculate(uint seed, ReadOnlySpan<byte> data);
}