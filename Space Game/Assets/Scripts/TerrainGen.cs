using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGen : MonoBehaviour
{
    public double radius;
    public int seed;
    public Color color1;
    public Color color2;

    public int GetNodeAt(Vector3Int pos)
    {
        Vector3 pos2 = (Vector3)pos / 10f;
        float noise = Perlin.Noise(pos2.x, pos2.y, pos2.z);
        double height = (2.0f * noise) + radius;
        if (pos.magnitude <= height)
            return 1;
        else
            return 0;
    }

    Color colorLerp(Color a, Color b, float t)
    {
        return (1 - t) * a + t * b;
    }

    public Color GetColorAt(Vector3 pos)
    {
        Vector3 pos2 = (Vector3)pos / 10f;
        float noise = Perlin.Noise(pos2.x + 10, pos2.y + 100, pos2.z + 1000);
        float noisem = (noise + 1) / 2;
        //return colorLerp(color1, color2, noisem);
        if (noisem > 0.6)
        {
            return color1;
        } else
        {
            return color2;
        }
    }

    public ProceduralNoiseProject.INoise GetNoise(ProceduralNoiseProject.NOISE_TYPE noiseType)
    {
        switch (noiseType)
        {
            case ProceduralNoiseProject.NOISE_TYPE.PERLIN:
                return new ProceduralNoiseProject.PerlinNoise(seed, 20);

            case ProceduralNoiseProject.NOISE_TYPE.VALUE:
                return new ProceduralNoiseProject.ValueNoise(seed, 20);

            case ProceduralNoiseProject.NOISE_TYPE.SIMPLEX:
                return new ProceduralNoiseProject.SimplexNoise(seed, 20);

            case ProceduralNoiseProject.NOISE_TYPE.VORONOI:
                return new ProceduralNoiseProject.VoronoiNoise(seed, 20);

            case ProceduralNoiseProject.NOISE_TYPE.WORLEY:
                return new ProceduralNoiseProject.WorleyNoise(seed, 20, 1.0f);

            default:
                return new ProceduralNoiseProject.PerlinNoise(seed, 20);
        }
    }
}
