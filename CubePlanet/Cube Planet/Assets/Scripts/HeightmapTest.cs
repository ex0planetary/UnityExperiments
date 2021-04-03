using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightmapTest : MonoBehaviour
{
    public int seed;
    public float volatility;
    public int minscale = 8;
    public int octaves = 3;

    void DiamondStep(int minx, int maxx, int miny, int maxy, System.Random rand, double[,] a)
    {
        if ((minx+maxx)%2==0)
        {
            //divisible by 2, carry on
            a[(minx + maxx) / 2, (miny + maxy) / 2] = ((a[minx, miny] + a[minx, maxy] + a[maxx, miny] + a[maxx, maxy]) / 4) + (((rand.NextDouble()*2)-1)* volatility * (Mathf.Pow(2,(maxx-minx)/1000)));
            SquareStep((minx + maxx) / 2, (miny + maxy) / 2, ((minx + maxx) / 2) - minx, rand, a);
        }
    }

    void SquareStep(int cx, int cy, int offset, System.Random rand, double[,] a)
    {
        a[cx + offset, cy] = a[cx, cy] + ((rand.NextDouble() * 2 - 1) * volatility * (Mathf.Pow(2, offset/1000)));
        a[cx - offset, cy] = a[cx, cy] + ((rand.NextDouble() * 2 - 1) * volatility * (Mathf.Pow(2, offset/1000)));
        a[cx, cy + offset] = a[cx, cy] + ((rand.NextDouble() * 2 - 1) * volatility * (Mathf.Pow(2, offset/1000)));
        a[cx, cy - offset] = a[cx, cy] + ((rand.NextDouble() * 2 - 1) * volatility * (Mathf.Pow(2, offset/1000)));
        DiamondStep(cx - offset, cx, cy - offset, cy, rand, a);
        DiamondStep(cx - offset, cx, cy, cy + offset, rand, a);
        DiamondStep(cx, cx + offset, cy - offset, cy, rand, a);
        DiamondStep(cx, cx + offset, cy, cy + offset, rand, a);
    }

    // Start is called before the first frame update
    void Start()
    {
        int scale = minscale * (int)Mathf.Pow(2,octaves-1);
        System.Random random = new System.Random(seed);
        double[,] a = new double[129, 129];
        float[,] ba = new float[129, 129];
        float[,] ta = new float[129, 129];
        a[0, 0] = random.NextDouble();
        a[0, 128] = random.NextDouble();
        a[128, 0] = random.NextDouble();
        a[128, 128] = random.NextDouble();
        var texture = new Texture2D(129, 129, TextureFormat.ARGB32, false);
        for (int i = 0; i < octaves; i++)
        {
            DiamondStep(0, 128, 0, 128, random, a);
            for (int x = 0; x < 129; x++)
            {
                for (int y = 0; y < 129; y++)
                {
                    ba[x, y] = Mathf.Lerp(Mathf.Lerp((float)a[(int)Mathf.Floor((float)x / scale), (int)Mathf.Floor((float)y / scale)], (float)a[(int)Mathf.Ceil((float)x / scale), (int)Mathf.Floor((float)y / scale)], (float)(x % scale) / scale), Mathf.Lerp((float)a[(int)Mathf.Floor((float)x / scale), (int)Mathf.Ceil((float)y / scale)], (float)a[(int)Mathf.Ceil((float)x / scale), (int)Mathf.Ceil((float)y / scale)], (float)(x % scale) / scale), (float)(y % scale) / scale);
                    //Debug.Log("PorX " + (float)x % scale / scale + " PorY " + (float)y % scale / scale + " MinX " + (int)Mathf.Min((float)x / scale) + " MinY " + (int)Mathf.Min((float)y / scale) + " MaxX " + (int)Mathf.Max((float)x / scale) + " MaxY " + (int)Mathf.Max((float)y / scale));
                    //ba[x, y] = Mathf.Lerp(Mathf.Lerp((float)a[(int)Mathf.Min((float)x * scale), (int)Mathf.Min((float)y * scale)], (float)a[(int)Mathf.Max((float)x * scale), (int)Mathf.Min((float)y * scale)], x % scale / scale), Mathf.Lerp((float)a[(int)Mathf.Min((float)x * scale), (int)Mathf.Max((float)y * scale)], (float)a[(int)Mathf.Max((float)x * scale), (int)Mathf.Max((float)y * scale)], x % scale / scale), y % scale / scale);
                    ta[x, y] += ba[x, y];
                }
            }
        }
        for (int x=0; x<129; x++)
        {
            for (int y=0; y<129; y++)
            {
                texture.SetPixel(x, y, new Color((float)ta[x, y], (float)ta[x, y], (float)ta[x, y], 1.0f));
            }
        }
        texture.Apply();
        GetComponent<Renderer>().material.mainTexture = texture;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
