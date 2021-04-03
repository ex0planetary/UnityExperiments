using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageChunks : MonoBehaviour
{
    public int chunkRad = 3;
    public GameObject chunkPrefab;
    private Transform tr;
    private GameObject[,,] chunks;
    public int loadSpeed = 0;

    public bool isMaxCoord(int n)
    {
        return n == chunkRad;
    }

    public bool isMinCoord(int n)
    {
        return n == chunkRad * -1;
    }

    public GameObject getChunkAt(int x, int y, int z)
    {
        return chunks[x + chunkRad, y + chunkRad, z + chunkRad];
    }

    // Start is called before the first frame update
    void Start()
    {
        int chunkDi = 2 * chunkRad + 1;
        chunks = new GameObject[chunkDi, chunkDi, chunkDi];
        tr = GetComponent<Transform>();
        // TODO: better dynamic loading system that loads in a radius around the player. will need public player transform to use lol
        /*for (int x = chunkRad * -1; x <= chunkRad; x++)
        {
            for (int y = chunkRad * -1; y <= chunkRad; y++)
            {
                for (int z = chunkRad * -1; z <= chunkRad; z++)
                {
                    chunks[x + chunkRad, y + chunkRad, z + chunkRad] = Object.Instantiate(chunkPrefab, tr.position + new Vector3(x, y, z) * 32, tr.rotation, tr);
                    chunks[x + chunkRad, y + chunkRad, z + chunkRad].GetComponent<ChunkData>().location = new Vector3(x, y, z);
                    chunks[x + chunkRad, y + chunkRad, z + chunkRad].GetComponent<ChunkData>().startGen();
                }
            }
        }*/
        StartCoroutine("LoadChunks");
        /*for (int x = chunkRad * -1; x <= chunkRad; x++)
        {
            for (int y = chunkRad * -1; y <= chunkRad; y++)
            {
                for (int z = chunkRad * -1; z <= chunkRad; z++)
                {
                    chunks[x + chunkRad, y + chunkRad, z + chunkRad].GetComponent<ChunkData>().chunkManager = this;
                    chunks[x + chunkRad, y + chunkRad, z + chunkRad].GetComponent<SurfaceNetsV2>().fullRender();
                }
            }
        }*/
    }

    IEnumerator LoadChunks()
    {
        for (int x = chunkRad * -1; x <= chunkRad; x++)
        {
            for (int y = chunkRad * -1; y <= chunkRad; y++)
            {
                for (int z = chunkRad * -1; z <= chunkRad; z++)
                {
                    chunks[x + chunkRad, y + chunkRad, z + chunkRad] = Object.Instantiate(chunkPrefab, tr.position + new Vector3(x, y, z) * 32, tr.rotation, tr);
                    chunks[x + chunkRad, y + chunkRad, z + chunkRad].GetComponent<ChunkGenerator>().initNoise();
                    chunks[x + chunkRad, y + chunkRad, z + chunkRad].GetComponent<ChunkData>().location = new Vector3(x, y, z);
                    chunks[x + chunkRad, y + chunkRad, z + chunkRad].GetComponent<ChunkData>().startGen();
                }
                yield return null;
            }
        }
        for (int x = chunkRad * -1; x <= chunkRad; x++)
        {
            for (int y = chunkRad * -1; y <= chunkRad; y++)
            {
                for (int z = chunkRad * -1; z <= chunkRad; z++)
                {
                    chunks[x + chunkRad, y + chunkRad, z + chunkRad].GetComponent<ChunkData>().chunkManager = this;
                    chunks[x + chunkRad, y + chunkRad, z + chunkRad].GetComponent<SurfaceNetsV2>().fullRender(1);
                    yield return null;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
