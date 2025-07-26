// See https://aka.ms/new-console-template for more information

using CommunityToolkit.HighPerformance;
using Rendezvous;

const long rangeStart = 1;
const long rangeEnd = 5_000_000;
const string csv = "one, two, three, four, five, six, seven, eight, nine";

string[] seq = ["one", "two", "three",  "four", "five", "six", "seven", "eight", "nine"];

// Analyze<MurMurHash32Bit>(csv, rangeStart, rangeEnd);
// Analyze<FowlerNollVoPickerHash32Bit>(csv, rangeStart, rangeEnd);
// Analyze<PolynomialRollingHash32Bit>(csv, rangeStart, rangeEnd);

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
