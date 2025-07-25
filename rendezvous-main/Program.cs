// See https://aka.ms/new-console-template for more information

using Rendezvous;

Console.WriteLine("Hello, World!");

IRendezvousPicker<long> picker = new RendezvousPicker<long>(new FowlerNollVoPickerHash32Bit());
Dictionary<string, int> stats = new(11);

for (long key = 1; key < 20000; key++)
{
    var picked = picker.Pick("cluster-a, cluster-b, cluster-c, cluster-d, cluster-e ", key).ToString();

    if (stats.TryGetValue(picked, out int count))
    {
        stats[picked] = count + 1;
    }
    else
    {
        stats[picked] = 1;
    }
}

foreach (var pair in stats)
{
    Console.WriteLine(pair.Key + ": " + pair.Value);
}