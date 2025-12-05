// See https://aka.ms/new-console-template for more information

using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance;
using Rendezvous;

const long rangeStart = 1;
const long rangeEnd = 5_000_000;
const string csv = "one, two, three, four, five, six, seven, eight, nine";

string[] seq = ["one", "two", "three",  "four", "five", "six", "seven", "eight", "nine"];

// Analyze<MurMurHash32Bit>(csv, rangeStart, rangeEnd);
// Analyze<FowlerNollVoPickerHash32Bit>(csv, rangeStart, rangeEnd);
// Analyze<PolynomialRollingHash32Bit>(csv, rangeStart, rangeEnd);

Console.WriteLine(Pack([-5, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 14, 15, 28, 29]));

AnalyzeSequence<MurMurHash32Bit>(seq, rangeStart, rangeEnd);
AnalyzeSequence<FowlerNollVoPickerHash32Bit>(seq, rangeStart, rangeEnd);
AnalyzeSequence<PolynomialRollingHash32Bit>(seq, rangeStart, rangeEnd);

return;

static IDictionary<string, int> Distribute(IRendezvousPicker<long> picker, string csv, long start, long end)
{
    Dictionary<string, int> distribution = new(11);
    
    for (long key = start; key <= end; key++)
    {
        var picked = picker.Pick(csv, key).ToString();

        if (distribution.TryGetValue(picked, out int count))
        {
            distribution[picked] = count + 1;
        }
        else
        {
            distribution[picked] = 1;
        }
    }
    
    return distribution;
}

static void PrintAnalysis<THash>(IDictionary<string, int> distribution, double size, int count)
{
    double mean = size / count;
    double sum = 0;
    foreach (var pair in distribution)
        sum += Math.Pow(mean - pair.Value, 2);
    double adjusted = Math.Sqrt(sum / count) * 100 / mean;
    double fill = (double)distribution.Count * 100 / count;
    
    Console.WriteLine($"{typeof(THash).Name} | deviation = {adjusted:F2}%, fill = {fill:F1}%");
}

static void Analyze<THash>(string csv, long start, long end) where THash : IPickerHash, new()
{
    int count = 0;
    foreach (var token in csv.Tokenize(',')) ++count;

    PrintAnalysis<THash>(
        Distribute(new RendezvousPicker<long>(new THash()), csv, start, end),
        end - start,
        count);
}

static IDictionary<string, int> DistributeSequence(IRendezvousPicker<long> picker, string[] names, long start, long end)
{
    Dictionary<string, int> distribution = new(11);
    
    for (long key = start; key <= end; key++)
    {
        var picked = picker.Pick(names, key);

        if (distribution.TryGetValue(picked, out int count))
        {
            distribution[picked] = count + 1;
        }
        else
        {
            distribution[picked] = 1;
        }
    }
    
    return distribution;
}

static void AnalyzeSequence<THash>(string[] names, long start, long end) where THash : IPickerHash, new()
{
    PrintAnalysis<THash>(
        DistributeSequence(new RendezvousPicker<long>(new THash()), names, start, end),
        end - start,
        names.Length);
}

static string Pack(int[] numbers)
{
    ref int np = ref MemoryMarshal.GetReference(numbers.AsSpan());
    ref int e = ref np;

    int min = np;
    int max = min;

    for (int i = 1; i < numbers.Length; ++i)
    {
        e = ref Unsafe.Add(ref e, 1);
        
        if (e < min) min = e;
        else if (e > max) max = e;
    }

    int distance = max - min;
    int byteLength = distance / 8 + 1;
    byte[] output = ArrayPool<byte>.Shared.Rent(byteLength);

    try
    {
        var outputSpan = output.AsSpan().Slice(0, byteLength); 
        ref byte bp = ref MemoryMarshal.GetReference(outputSpan);

        for (int i = 0; i < numbers.Length; ++i)
        {
            int bitIndex = np - min;
            ref byte b = ref Unsafe.Add(ref bp, bitIndex / 8);
            b |= (byte)(1 << (bitIndex % 8));
        
            np = ref Unsafe.Add(ref np, 1);
        }
    
        return Convert.ToBase64String(outputSpan);
    }
    finally
    {
        ArrayPool<byte>.Shared.Return(output);
    }
}