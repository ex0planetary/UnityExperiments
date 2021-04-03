using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidgedNoiseFilter
{
    public float deltaFreq, volatility, persistence, baseFreq, sharpness;
    public int octaves;
    public Vector3 Center;
    public int seed;
    Noise noise;
    float frequency;
    float amplitude;

    public RidgedNoiseFilter(int seed, float sharp, float bfreq, float dfreq, float pers, float vol, int oct, Vector3 cen)
    {
        baseFreq = bfreq;
        frequency = baseFreq;
        deltaFreq = dfreq;
        persistence = pers;
        volatility = vol;
        Center = cen;
        sharpness = sharp;
        octaves = oct;
        noise = new Noise(seed);
    }
    public void ChangeSettings(float sharp, float bfreq, float dfreq, float pers, float vol, int oct, Vector3 cen)
    {
        baseFreq = bfreq;
        frequency = baseFreq;
        deltaFreq = dfreq;
        persistence = pers;
        volatility = vol;
        Center = cen;
        sharpness = sharp;
        octaves = oct;
    }
    public float Noise(Vector3 point)
    {
        amplitude = 1;
        frequency = baseFreq;
        float weight = 1;
        float nv = 0;
        for (int i = 0; i < octaves; i++)
        {
            float v = Mathf.Pow(Mathf.Abs(noise.Evaluate(point * frequency + Center)),sharpness);
            v *= weight;
            weight = v;

            nv += v * amplitude;
            frequency *= deltaFreq;
            amplitude *= persistence;
        }
        return nv * volatility;
    }
}
