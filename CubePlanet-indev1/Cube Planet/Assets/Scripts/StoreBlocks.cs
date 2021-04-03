using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreBlocks : MonoBehaviour
{
    public Color32[,,] terrain = new Color32[9, 9, 9];
    // Start is called before the first frame update
    void Awake()
    {
        for (int x=0; x< 9; x++)
        {
            for (int y=0; y< 9; y++)
            {
                for (int z=0; z< 9; z++)
                {
                    terrain[x, y, z].r = 0;
                    terrain[x, y, z].g = 0;
                    terrain[x, y, z].b = 0;
                    terrain[x, y, z].a = 0;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeAt(int x, int y, int z, Color32 color)
    {
        terrain[x + 4, y + 4, z + 4] = color;
    }

    public Color32 GetTerrain(int x, int y, int z)
    {
        if (x >= -4 && x <= 4 && y >= -4 && y <= 4 && z >= -4 && z <= 4)
        {
            return terrain[x + 4, y + 4, z + 4];
        } else
        {
            return new Color32(0, 0, 0, 0);
        }
    }

    public bool Transparent(int x, int y, int z)
    {
        return GetTerrain(x, y, z).a != 255;
    }
}
