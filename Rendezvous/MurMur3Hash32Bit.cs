using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Rendezvous;

public class MurMurHash32Bit : IPickerHash
{
    uint IPickerHash.Calculate(uint seed, ReadOnlySpan<byte> data)
    {
        ref byte bp = ref MemoryMarshal.GetReference(data);
        ref uint endPoint = ref Unsafe.Add(ref Unsafe.As<byte, uint>(ref bp), data.Length >> 2);
        if (data.Length >= 4)
        {
            do
            {
                seed = BitOperations.RotateLeft(seed ^ BitOperations.RotateLeft(Unsafe.ReadUnaligned<uint>(ref bp) * 3432918353U, 15) * 461845907U, 13) * 5 - 430675100;
                bp = ref Unsafe.Add(ref bp, 4);
            } while (Unsafe.IsAddressLessThan(ref Unsafe.As<byte, uint>(ref bp), ref endPoint));
        }

        var remainder = data.Length & 3;
        if (remainder > 0)
        {
            uint num = 0;
            if (remainder > 2) num ^= Unsafe.Add(ref endPoint, 2) << 16;
            if (remainder > 1) num ^= Unsafe.Add(ref endPoint, 1) << 8;
            num ^= endPoint;

            seed ^= BitOperations.RotateLeft(num * 3432918353U, 15) * 461845907U;
        }

        seed ^= (uint)data.Length;
        seed = (uint)((seed ^ (seed >> 16)) * -2048144789);
        seed = (uint)((seed ^ (seed >> 13)) * -1028477387);
        return seed ^ seed >> 16;
    }
}