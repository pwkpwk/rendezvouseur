// See https://aka.ms/new-console-template for more information

using CommunityToolkit.HighPerformance;
using Rendezvous;

const long rangeStart = 1;
const long rangeEnd = 5_000_000;
const string csv = "one, two, three, four, five, six, seven, eight, nine";

Analyze<MurMurHash32Bit>(csv, rangeStart, rangeEnd);
Analyze<FowlerNollVoPickerHash32Bit>(csv, rangeStart, rangeEnd);

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

static void Analyze<THash>(string csv, long start, long end) where THash : IPickerHash, new()
{
    int count = 0;
    foreach (var token in csv.Tokenize(',')) ++count;
    var distribution = Distribute(new RendezvousPicker<long>(new THash()), csv, start, end);

    double mean = (double)(end - start) / count;
    double sum = 0;
    foreach (var pair in distribution)
        sum += Math.Pow(mean - pair.Value, 2);
    double adjusted = Math.Sqrt(sum / count) * 100 / mean;
    double fill = (double)distribution.Count * 100 / count;
    
    Console.WriteLine($"{typeof(THash).Name} | deviation = {adjusted:F2}%, fill = {fill:F1}%");
}
