using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Math {
    public static float RandomNormal(float mean, float standardDeviation, int seed, float min = 0f, float max = float.MaxValue) {
        Random.InitState(seed);

        float u1 = 1.0f - Random.Range(0f, 1f);
        float u2 = 1.0f - Random.Range(0f, 1f);
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);

        return Mathf.Clamp(mean + standardDeviation * randStdNormal, min, max);
    }

    public static float RandomHalfNormal(float mean, float standardDeviation, int seed, float max = float.MaxValue)
    {
        Random.InitState(seed);

        float u1 = 1.0f - Random.Range(0f, 1f);
        float u2 = 1.0f - Random.Range(0f, 1f);
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);

        return Mathf.Clamp(Mathf.Abs(mean + standardDeviation * randStdNormal), mean, max);
    }

    public static float Map(float n, float minA, float maxA, float minB, float maxB)
    {
        return (n - minA) / (maxA - minA) * (maxB - minB) + minB;
    }
}

[System.Serializable]
public class GaussianParam : System.Object {
    public float Mean;
    public float StandardDeviation;
}
