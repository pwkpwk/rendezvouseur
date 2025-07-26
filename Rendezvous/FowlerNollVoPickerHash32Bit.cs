using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Rendezvous;

public class FowlerNollVoPickerHash32Bit : IPickerHash
{
    private const uint FnvPrime = 16777619;
    private const uint OffsetBasis = 2166136261;

    uint IPickerHash.Calculate(uint seed, ReadOnlySpan<byte> data)
    {
        ref byte bp = ref MemoryMarshal.GetReference(data);
            
        // 32-bit FNV-1a hashing algorithm
        // https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
        uint hash = OffsetBasis;

        for (int i = 0; i < data.Length; ++i)
        {
            hash = (hash ^ bp) * FnvPrime;
            bp = ref Unsafe.Add(ref bp, 1);
        }
            
        return (hash ^ seed) * FnvPrime;
    }
}