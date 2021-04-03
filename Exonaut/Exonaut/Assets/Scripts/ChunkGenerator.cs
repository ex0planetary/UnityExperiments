using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public enum genAlgo { SINGLENODE, SPHERE, CUBE };
    public genAlgo algorithm;
    public float radius = 10;
    public float volatility = 7.5f;
    FastNoise noise, noise2, noise3;

    public bool getTerrainAt(int x, int y, int z)
    {
        bool node = false;
        if (algorithm == genAlgo.SINGLENODE)
        {
            if (x == 16 && y == 16 && z == 16)
            {
                node = true;
            }
        }
        if (algorithm == genAlgo.SPHERE)
        {
            Vector3 posNorm = Vector3.Normalize(new Vector3(x, y, z)) * radius;
            float height = radius + noise.GetNoise(posNorm.x, posNorm.y, posNorm.z) * volatility;
            if (height < radius) height = radius;
            if (Mathf.Sqrt(x * x + y * y + z * z) < height)
            {
                //Debug.Log(noise2.GetNoise(x, y, z));
                if (noise2.GetNoise(x, y, z) <= 0.05f && -0.05f <= noise2.GetNoise(x, y, z) && noise3.GetNoise(x, y, z) <= -0.25f)
                    node = false;
                else
                    node = true;
            }
        }
        if (algorithm == genAlgo.CUBE)
        {
            if (x > 16 - radius && x < 16 + radius && y > 16 - radius && y < 16 + radius && z > 16 - radius && z < 16 + radius)
            {
                node = true;
            }
        }
        return node;
    }

    public int getTerrainTypeAt(int x, int y, int z)
    {
        Vector3 posNorm = Vector3.Normalize(new Vector3(x, y, z)) * radius;
        if (Mathf.Sqrt(x*x + y*y + z*z) < radius - 4)
        {
            return 3;
        }
        if (noise.GetNoise(posNorm.x, posNorm.y, posNorm.z) < 0)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }

    public void initNoise()
    {
        noise = new FastNoise();
        noise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        noise2 = new FastNoise(7113);
        noise2.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        noise2.SetFrequency(0.025f);
        noise3 = new FastNoise(7113);
        noise3.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
