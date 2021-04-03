using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkCreator : MonoBehaviour
{
    public int radius = 2;
    public GameObject chunkPrefab;
    public float scale = 1f;
    public GameObject[,,] chunks;

    // Start is called before the first frame update
    void Start()
    {
        chunks = new GameObject[radius * 2 + 1, radius * 2 + 1, radius * 2 + 1];
        for (int x = radius * -1; x <= radius; x++)
        {
            for (int y = radius * -1; y <= radius; y++)
            {
                for (int z = radius * -1; z <= radius; z++)
                {
                    chunks[x + radius, y + radius, z + radius] = (GameObject)Instantiate(chunkPrefab, GetComponent<Transform>());
                    GameObject nc = chunks[x + radius, y + radius, z + radius];
                    ChunkInfo info = nc.GetComponent<ChunkInfo>();
                    info.planetTerrain = GetComponent<TerrainGen>();
                    info.position = new Vector3Int(x, y, z);
                    info.chunkRad = radius * -1;
                    info.scale = scale;
                    Transform trans = nc.GetComponent<Transform>();
                    trans.localPosition = new Vector3(x, y, z) * 3.2f * scale - new Vector3(1.6f, 1.6f, 1.6f) * scale;
                }
            }
        }
    }
}
