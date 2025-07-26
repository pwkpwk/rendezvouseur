using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Rendezvous;

public class PolynomialRollingHash32Bit : IPickerHash
{
    private const uint P = 31;
    private const uint M = 1000000009;

    uint IPickerHash.Calculate(uint seed, ReadOnlySpan<byte> data)
    {
        ref byte bp = ref MemoryMarshal.GetReference(data);
            
        ulong hash = 0;
        ulong ppow = 1;

        for (int i = 0; i < data.Length; ++i)
        {
            hash = (hash + bp * ppow) % M;
            ppow = ppow * P % M;
            bp = ref Unsafe.Add(ref bp, 1);
        }
            
        hash  = (hash + seed * ppow) % M;

        return (uint)(hash ^ (hash >> 32));
    }
}