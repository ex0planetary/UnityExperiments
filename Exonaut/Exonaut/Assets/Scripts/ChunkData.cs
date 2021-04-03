using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData : MonoBehaviour
{
    public int[,,] nodeData = new int[32, 32, 32];
    public float[,,] nodeDataS = new float[32, 32, 32];
    public int[,,] nodeTypes = new int[32, 32, 32];
    public Vector3 location;
    private ChunkGenerator cg;
    public ManageChunks chunkManager;
    public bool initialized = false;

    public int getNodeDataAt(int x, int y, int z)
    {
        return nodeData[x, y, z];
    }

    public float getNodeDataSAt(int x, int y, int z)
    {
        return nodeDataS[x, y, z];
    }

    public void setNodeDataAt(int x, int y, int z, int to)
    {
        nodeData[x, y, z] = to;
    }
    
    public int getNodeTypeAt(int x, int y, int z)
    {
        return nodeTypes[x, y, z];
    }

    public Color32 getNodeColorAt(int x, int y, int z)
    {
        int type = 0;
        if (x >= 32 && y >= 32 && z >= 32)
        {
            if (chunkManager.isMaxCoord((int)location.x) || chunkManager.isMaxCoord((int)location.y) || chunkManager.isMaxCoord((int)location.z))
            {
                type = 0;
            }
            else
            {
                type = chunkManager.getChunkAt((int)location.x + 1, (int)location.y + 1, (int)location.z + 1).GetComponent<ChunkData>().getNodeTypeAt(x - 32, y - 32, z - 32);
            }
        }
        else if (x >= 32 && y >= 32)
        {
            if (chunkManager.isMaxCoord((int)location.x) || chunkManager.isMaxCoord((int)location.y))
            {
                type = 0;
            }
            else
            {
                type = chunkManager.getChunkAt((int)location.x + 1, (int)location.y + 1, (int)location.z).GetComponent<ChunkData>().getNodeTypeAt(x - 32, y - 32, z);
            }
        }
        else if (y >= 32 && z >= 32)
        {
            if (chunkManager.isMaxCoord((int)location.y) || chunkManager.isMaxCoord((int)location.z))
            {
                type = 0;
            }
            else
            {
                type = chunkManager.getChunkAt((int)location.x, (int)location.y + 1, (int)location.z + 1).GetComponent<ChunkData>().getNodeTypeAt(x, y - 32, z - 32);
            }
        }
        else if (x >= 32 && z >= 32)
        {
            if (chunkManager.isMaxCoord((int)location.x) || chunkManager.isMaxCoord((int)location.z))
            {
                type = 0;
            }
            else
            {
                type = chunkManager.getChunkAt((int)location.x + 1, (int)location.y , (int)location.z + 1).GetComponent<ChunkData>().getNodeTypeAt(x - 32, y, z - 32);
            }
        }
        else if (x >= 32)
        {
            if (chunkManager.isMaxCoord((int)location.x))
            {
                type = 0;
            }
            else
            {
                type = chunkManager.getChunkAt((int)location.x + 1, (int)location.y, (int)location.z).GetComponent<ChunkData>().getNodeTypeAt(x - 32, y, z);
            }
        }
        else if (y >= 32)
        {
            if (chunkManager.isMaxCoord((int)location.y))
            {
                type = 0;
            }
            else
            {
                type = chunkManager.getChunkAt((int)location.x, (int)location.y + 1, (int)location.z).GetComponent<ChunkData>().getNodeTypeAt(x, y - 32, z);
            }
        }
        else if (z >= 32)
        {
            if (chunkManager.isMaxCoord((int)location.z))
            {
                type = 0;
            }
            else
            {
                type = chunkManager.getChunkAt((int)location.x, (int)location.y, (int)location.z + 1).GetComponent<ChunkData>().getNodeTypeAt(x , y, z - 32);
            }
        }
        else
        {
            type = nodeTypes[x, y, z];
        }
        switch (type)
        {
            case 1:
                return new Color32(63, 184, 26, 255);
            case 2:
                return new Color32(34, 138, 212, 255);
            case 3:
                return new Color32(97, 72, 2, 255);
            default:
                return new Color32(0, 0, 0, 255);
        }
    }

    public void startGen()
    {
        cg = GetComponent<ChunkGenerator>();
        chunkManager = transform.parent.gameObject.GetComponent<ManageChunks>();
        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 32; y++)
            {
                for (int z = 0; z < 32; z++)
                {
                    if (cg.getTerrainAt((int)(x + location.x * 32 - 16), (int)(y + location.y * 32 - 16), (int)(z + location.z * 32 - 16)))
                    {
                        nodeData[x, y, z] = 1;
                        nodeTypes[x, y, z] = cg.getTerrainTypeAt((int)(x + location.x * 32 - 16), (int)(y + location.y * 32 - 16), (int)(z + location.z * 32 - 16));
                    } else
                    {
                        nodeData[x, y, z] = -1;
                    }
                    //nodeData[x,y,z] *= Mathf.CeilToInt(Mathf.Sqrt((x - 16) * (x - 16) + (y - 16) * (y - 16) + (z - 16) * (z - 16)));
                }
            }
        }
        //initialized = true;
    }

    public void ensmoothen()
    {
        int[,,] nodeDataPlus = new int[36, 36, 36];
        for (int x = 0; x < 36; x++)
        {
            for (int y = 0; y < 36; y++)
            {
                for (int z = 0; z < 36; z++)
                {
                    // TODO: this
                }
            }
        }
        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 32; y++)
            {
                for (int z = 0; z < 32; z++)
                {
                    Vector3Int start = new Vector3Int(x - 2, y - 2, z - 2);
                    Vector3Int end = new Vector3Int(x + 2, y + 2, z + 2);
                    /*if (start.x < 0) start.x = 0;
                    if (start.y < 0) start.y = 0;
                    if (start.z < 0) start.z = 0;
                    if (end.x > 31) end.x = 31;
                    if (end.y > 31) end.y = 31;
                    if (end.z > 31) end.z = 31;*/
                    int sum = 0;
                    int count = 0;
                    for (int xi = start.x; xi <= end.x; xi++)
                    {
                        for (int yi = start.y; yi <= end.y; yi++)
                        {
                            for (int zi = start.z; zi <= end.z; zi++)
                            {
                                count += 1;
                                /*if (xi >= 0 && xi < 32 && yi >= 0 && yi < 32 && zi >= 0 && zi < 32)
                                {
                                    sum += nodeData[xi, yi, zi];
                                }
                                else
                                {
                                    sum += -1;
                                }*/
                            }
                        }
                    }
                    nodeDataS[x, y, z] = sum / count;
                }
            }
        }
    }
}
