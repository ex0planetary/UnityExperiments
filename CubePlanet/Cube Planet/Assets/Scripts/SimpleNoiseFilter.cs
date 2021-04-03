using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNoiseFilter
{
    public float deltaFreq, volatility, persistence, baseFreq;
    public int octaves;
    public Vector3 Center;
    public int seed;
    Noise noise;
    float frequency;
    float amplitude;

    public SimpleNoiseFilter(int seed, float bfreq, float dfreq, float pers, float vol, int oct, Vector3 cen)
    {
        baseFreq = bfreq;
        frequency = baseFreq;
        deltaFreq = dfreq;
        persistence = pers;
        volatility = vol;
        Center = cen;
        octaves = oct;
        noise = new Noise(seed);
    }
    public void ChangeSettings(float bfreq, float dfreq, float pers, float vol, int oct, Vector3 cen)
    {
        baseFreq = bfreq;
        frequency = baseFreq;
        deltaFreq = dfreq;
        persistence = pers;
        volatility = vol;
        Center = cen;
        octaves = oct;
    }
    public float Noise(Vector3 point)
    {
        amplitude = 1;
        frequency = baseFreq;
        float weight = 1;
        float nv = 0;
        for (int i=0; i<octaves; i++)
        {
            float v = (noise.Evaluate(point * frequency + Center) + 1) * 0.5f;
            v *= weight;
            weight = v;

            nv += v * amplitude;
            frequency *= deltaFreq;
            amplitude *= persistence;
        }
        return nv * volatility;
    }
}
