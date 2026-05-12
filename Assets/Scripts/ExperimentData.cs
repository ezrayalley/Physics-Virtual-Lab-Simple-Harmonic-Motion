using System.Collections.Generic;

public static class ExperimentData
{
    public static List<float> lengths = new List<float>();
    public static List<float> times = new List<float>();
    public static List<int> oscillations = new List<int>();

    public static void Add(float L, float t, int n)
    {
        lengths.Add(L);
        times.Add(t);
        oscillations.Add(n);
    }

    public static void Clear()
    {
        lengths.Clear();
        times.Clear();
        oscillations.Clear();
    }
}