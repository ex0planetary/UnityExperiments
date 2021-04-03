using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MeshData
{
    public Vector3[] vertices;
    public Vector3[] normals;
    public int[] triangles;
    public Color32[] colors;
    public MeshData(Vector3[] v,Vector3[] n,int[] t,Color32[] c)
    {
        vertices = v;
        normals = n;
        triangles = t;
        colors = c;
    }
}

public struct Tri
{
    public Vector3Int a, b, c;
    public Vector3 normal;
    public Color32 color;
    public Tri(Vector3Int pa, Vector3Int pb, Vector3Int pc, Color32 co)
    {
        a = pa;
        b = pb;
        c = pc;
        color = co;
        normal = Vector3.Cross(Vector3.Normalize(b - a), Vector3.Normalize(c - a));
    }
}

public class SurfaceNetsV2 : MonoBehaviour
{
    private ChunkData cd;
    private MeshFilter mf;
    private int[,,] nodes = new int[32, 32, 32];
    public bool cuboid;

    public MeshData Generate(int lod)
    {
        /**
         * STEP 1: Construct seamless node array from chunk data of this and neighboring chunks
         */

        int nodeGridSize = (int)(32 * Mathf.Pow(0.5f, lod));
        int reductionFactor = (int)Mathf.Pow(2, lod);
        // reduce this chunk's node data to its LODified bits
        int[,,] nodesLod = new int[nodeGridSize + 2, nodeGridSize + 2, nodeGridSize + 2];
        float[,,] nodesLodS = new float[nodeGridSize + 2, nodeGridSize + 2, nodeGridSize + 2];
        for (int x = 0; x < nodeGridSize; x++)
        {
            for (int y = 0; y < nodeGridSize; y++)
            {
                for (int z = 0; z < nodeGridSize; z++)
                {
                    nodesLod[x, y, z] = cd.getNodeDataAt(x * reductionFactor, y * reductionFactor, z * reductionFactor);
                    nodesLodS[x, y, z] = cd.getNodeDataSAt(x * reductionFactor, y * reductionFactor, z * reductionFactor);
                }
            }
        }
        // neighbor data: planes
        if (cd.chunkManager.isMaxCoord((int)cd.location.x))
        {
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < nodeGridSize; y++)
                {
                    for (int z = 0; z < nodeGridSize; z++)
                    {
                        nodesLod[nodeGridSize + x, y, z] = -1;
                        nodesLodS[nodeGridSize + x, y, z] = -1;
                    }
                }
            }
        }
        else
        {
            ChunkData pxData = cd.chunkManager.getChunkAt((int)cd.location.x + 1, (int)cd.location.y, (int)cd.location.z).GetComponent<ChunkData>();
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < nodeGridSize; y++)
                {
                    for (int z = 0; z < nodeGridSize; z++)
                    {
                        nodesLod[nodeGridSize + x, y, z] = pxData.getNodeDataAt(x * reductionFactor, y * reductionFactor, z * reductionFactor);
                        nodesLodS[nodeGridSize + x, y, z] = pxData.getNodeDataSAt(x, y, z);
                    }
                }
            }
        }
        if (cd.chunkManager.isMaxCoord((int)cd.location.y))
        {
            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < nodeGridSize; x++)
                {
                    for (int z = 0; z < nodeGridSize; z++)
                    {
                        nodesLod[x, nodeGridSize + y, z] = -1;
                        nodesLodS[x, nodeGridSize + y, z] = -1;
                    }
                }
            }
        }
        else
        {
            ChunkData pyData = cd.chunkManager.getChunkAt((int)cd.location.x, (int)cd.location.y + 1, (int)cd.location.z).GetComponent<ChunkData>();
            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < nodeGridSize; x++)
                {
                    for (int z = 0; z < nodeGridSize; z++)
                    {
                        nodesLod[x, nodeGridSize + y, z] = pyData.getNodeDataAt(x * reductionFactor, y * reductionFactor, z * reductionFactor);
                        nodesLodS[x, nodeGridSize + y, z] = pyData.getNodeDataSAt(x, y, z);
                    }
                }
            }
        }
        if (cd.chunkManager.isMaxCoord((int)cd.location.z))
        {
            for (int z = 0; z < 2; z++)
            {
                for (int y = 0; y < nodeGridSize; y++)
                {
                    for (int x = 0; x < nodeGridSize; x++)
                    {
                        nodesLod[x, y, nodeGridSize + z] = -1;
                        nodesLodS[x, y, nodeGridSize + z] = -1;
                    }
                }
            }
        }
        else
        {
            ChunkData pzData = cd.chunkManager.getChunkAt((int)cd.location.x, (int)cd.location.y, (int)cd.location.z + 1).GetComponent<ChunkData>();
            for (int z = 0; z < 2; z++)
            {
                for (int y = 0; y < nodeGridSize; y++)
                {
                    for (int x = 0; x < nodeGridSize; x++)
                    {
                        nodesLod[x, y, nodeGridSize + z] = pzData.getNodeDataAt(x * reductionFactor, y * reductionFactor, z * reductionFactor);
                        nodesLodS[x, y, nodeGridSize + z] = pzData.getNodeDataSAt(x, y, z);
                    }
                }
            }
        }
        // neighbor data: lines
        if (cd.chunkManager.isMaxCoord((int)cd.location.x) || cd.chunkManager.isMaxCoord((int)cd.location.y))
        {
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    for (int z = 0; z < nodeGridSize; z++)
                    {
                        nodesLod[nodeGridSize + x, nodeGridSize + y, z] = -1;
                        nodesLodS[nodeGridSize + x, nodeGridSize + y, z] = -1;
                    }
                }
            }
        }
        else
        {
            ChunkData pxyData = cd.chunkManager.getChunkAt((int)cd.location.x + 1, (int)cd.location.y + 1, (int)cd.location.z).GetComponent<ChunkData>();
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    for (int z = 0; z < nodeGridSize; z++)
                    {
                        nodesLod[nodeGridSize + x, nodeGridSize + y, z] = pxyData.getNodeDataAt(x * reductionFactor, y * reductionFactor, z * reductionFactor);
                        nodesLodS[nodeGridSize + x, nodeGridSize + y, z] = pxyData.getNodeDataSAt(x, y, z);
                    }
                }
            }
        }
        if (cd.chunkManager.isMaxCoord((int)cd.location.x) || cd.chunkManager.isMaxCoord((int)cd.location.z))
        {
            for (int x = 0; x < 2; x++)
            {
                for (int z = 0; z < 2; z++)
                {
                    for (int y = 0; y < nodeGridSize; y++)
                    {
                        nodesLod[nodeGridSize + x, y, nodeGridSize + z] = -1;
                        nodesLodS[nodeGridSize + x, y, nodeGridSize + z] = -1;
                    }
                }
            }
        }
        else
        {
            ChunkData pxzData = cd.chunkManager.getChunkAt((int)cd.location.x + 1, (int)cd.location.y, (int)cd.location.z + 1).GetComponent<ChunkData>();
            for (int x = 0; x < 2; x++)
            {
                for (int z = 0; z < 2; z++)
                {
                    for (int y = 0; y < nodeGridSize; y++)
                    {
                        nodesLod[nodeGridSize + x, y, nodeGridSize + z] = pxzData.getNodeDataAt(x * reductionFactor, y * reductionFactor, z * reductionFactor);
                        nodesLodS[nodeGridSize + x, y, nodeGridSize + z] = pxzData.getNodeDataSAt(x, y, z);
                    }
                }
            }
        }
        if (cd.chunkManager.isMaxCoord((int)cd.location.z) || cd.chunkManager.isMaxCoord((int)cd.location.y))
        {
            for (int z = 0; z < 2; z++)
            {
                for (int y = 0; y < 2; y++)
                {
                    for (int x = 0; x < nodeGridSize; x++)
                    {
                        nodesLod[x, nodeGridSize + y, nodeGridSize + z] = -1;
                        nodesLodS[x, nodeGridSize + y, nodeGridSize + z] = -1;
                    }
                }
            }
        }
        else
        {
            ChunkData pyzData = cd.chunkManager.getChunkAt((int)cd.location.x, (int)cd.location.y + 1, (int)cd.location.z + 1).GetComponent<ChunkData>();
            for (int z = 0; z < 2; z++)
            {
                for (int y = 0; y < 2; y++)
                {
                    for (int x = 0; x < nodeGridSize; x++)
                    {
                        nodesLod[x, nodeGridSize + y, nodeGridSize + z] = pyzData.getNodeDataAt(x * reductionFactor, y * reductionFactor, z * reductionFactor);
                        nodesLodS[x, nodeGridSize + y, nodeGridSize + z] = pyzData.getNodeDataSAt(x, y, z);
                    }
                }
            }
        }
        // neighbor data: corner
        if (cd.chunkManager.isMaxCoord((int)cd.location.x) || cd.chunkManager.isMaxCoord((int)cd.location.y) || cd.chunkManager.isMaxCoord((int)cd.location.z))
        {
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    for (int z = 0; z < 2; z++)
                    {
                        nodesLod[nodeGridSize + x, nodeGridSize + y, nodeGridSize + z] = -1;
                        nodesLodS[nodeGridSize + x, nodeGridSize + y, nodeGridSize + z] = -1;
                    }
                }
            }
        }
        else
        {
            ChunkData pxyzData = cd.chunkManager.getChunkAt((int)cd.location.x + 1, (int)cd.location.y + 1, (int)cd.location.z + 1).GetComponent<ChunkData>();
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    for (int z = 0; z < 2; z++)
                    {
                        nodesLod[nodeGridSize + x, nodeGridSize + y, nodeGridSize + z] = pxyzData.getNodeDataAt(x * reductionFactor, y * reductionFactor, z * reductionFactor);
                        nodesLodS[nodeGridSize + x, nodeGridSize + y, nodeGridSize + z] = pxyzData.getNodeDataSAt(x, y, z);
                    }
                }
            }
        }

        /**
         * STEP 2: Parse through all edges, pushing new triangle objects when necessary, as well as pushing edges for vertex adjustment prep
         */

        // For non-dominant directions: [1, nodeGridSize]
        // For dominant directions: [0, nodeGridSize]
        // Edges go from n to n+1 in dominant direction

        List<Tri> tris = new List<Tri>();
        List<Vector3>[,,] vertLocs = new List<Vector3>[nodeGridSize + 1, nodeGridSize + 1, nodeGridSize + 1];
        for (int x = 0; x < nodeGridSize + 1; x++)
        {
            for (int y = 0; y < nodeGridSize + 1; y++)
            {
                for (int z = 0; z < nodeGridSize + 1; z++)
                {
                    vertLocs[x, y, z] = new List<Vector3>();
                }
            }
        }

        // X-dominant edges
        for (int y = 1; y < nodeGridSize + 2; y++)
        {
            for (int z = 1; z < nodeGridSize + 2; z++)
            {
                for (int x = 0; x < nodeGridSize + 1; x++)
                {
                    // Edge = (x,y,z)-(x+1,y,z)
                    // Edge in actual coords is multiplied by reductionFactor
                    if (nodesLod[x, y, z] != nodesLod[x + 1, y, z])
                    {
                        //float offset = nodesLodS[x, y, z] / (nodesLodS[x, y, z] - nodesLodS[x + 1, y, z]);
                        float offset = 0.5f;
                        if (y != 0 && y != nodeGridSize + 1 && z != 0 && z != nodeGridSize + 1)
                        {
                            // There's a rect around this! Tri1 = --, -+, +-, Tri2 = ++, +-, -+
                            // x = x
                            // n=(y,z) = n-1 (-), n (+)
                            if (nodesLod[x, y, z] > nodesLod[x + 1, y, z])
                            {
                                tris.Add(new Tri(new Vector3Int(x, y - 1, z), new Vector3Int(x, y - 1, z - 1), new Vector3Int(x, y, z - 1), cd.getNodeColorAt(x * reductionFactor, y * reductionFactor, z * reductionFactor)));
                                tris.Add(new Tri(new Vector3Int(x, y, z - 1), new Vector3Int(x, y, z), new Vector3Int(x, y - 1, z), cd.getNodeColorAt(x * reductionFactor, y * reductionFactor, z * reductionFactor)));
                            }
                            else
                            {
                                tris.Add(new Tri(new Vector3Int(x, y - 1, z - 1), new Vector3Int(x, y - 1, z), new Vector3Int(x, y, z - 1), cd.getNodeColorAt((x + 1) * reductionFactor, y * reductionFactor, z * reductionFactor)));
                                tris.Add(new Tri(new Vector3Int(x, y, z), new Vector3Int(x, y, z - 1), new Vector3Int(x, y - 1, z), cd.getNodeColorAt((x + 1) * reductionFactor, y * reductionFactor, z * reductionFactor)));
                            }
                            vertLocs[x, y - 1, z - 1].Add(new Vector3(x + offset, y, z) * reductionFactor);
                            vertLocs[x, y - 1, z].Add(new Vector3(x + offset, y, z) * reductionFactor);
                            vertLocs[x, y, z - 1].Add(new Vector3(x + offset, y, z) * reductionFactor);
                            vertLocs[x, y, z].Add(new Vector3(x + offset, y, z) * reductionFactor);
                        }
                        else
                        {
                            // Add data for vertex smoothing but nothing else
                            if (y-1 >= 0 && z-1 >= 0)
                            {
                                vertLocs[x, y - 1, z - 1].Add(new Vector3(x + offset, y, z) * reductionFactor);
                            }
                            if (y-1 >= 0 && z < nodeGridSize + 1)
                            {
                                vertLocs[x, y - 1, z].Add(new Vector3(x + offset, y, z) * reductionFactor);
                            }
                            if (y < nodeGridSize + 1 && z-1 >= 0)
                            {
                                vertLocs[x, y, z - 1].Add(new Vector3(x + offset, y, z) * reductionFactor);
                            }
                            if (y < nodeGridSize + 1 && z < nodeGridSize + 1)
                            {
                                vertLocs[x, y, z].Add(new Vector3(x + offset, y, z) * reductionFactor);
                            }
                        }
                    }
                }
            }
        }

        // Y-dominant edges
        for (int x = 0; x < nodeGridSize + 2; x++)
        {
            for (int z = 0; z < nodeGridSize + 2; z++)
            {
                for (int y = 0; y < nodeGridSize + 1; y++)
                {
                    // Edge = (x,y,z)-(x,y+1,z)
                    // Edge in actual coords is multiplied by reductionFactor
                    if (nodesLod[x, y, z] != nodesLod[x, y + 1, z])
                    {
                        //float offset = nodesLodS[x, y, z] / (nodesLodS[x, y, z] - nodesLodS[x, y + 1, z]);
                        float offset = 0.5f;
                        if (x != 0 && x != nodeGridSize + 1 && z != 0 && z != nodeGridSize + 1)
                        {
                            // There's a rect around this! Tri1 = --, -+, +-, Tri2 = ++, +-, -+
                            if (nodesLod[x, y, z] > nodesLod[x, y + 1, z])
                            {
                                tris.Add(new Tri(new Vector3Int(x - 1, y, z - 1), new Vector3Int(x - 1, y, z), new Vector3Int(x, y, z - 1), cd.getNodeColorAt(x * reductionFactor, y * reductionFactor, z * reductionFactor)));
                                tris.Add(new Tri(new Vector3Int(x, y, z), new Vector3Int(x, y, z - 1), new Vector3Int(x - 1, y, z), cd.getNodeColorAt(x * reductionFactor, y * reductionFactor, z * reductionFactor)));
                            }
                            else
                            {
                                tris.Add(new Tri(new Vector3Int(x - 1, y, z), new Vector3Int(x - 1, y, z - 1), new Vector3Int(x, y, z - 1), cd.getNodeColorAt(x * reductionFactor, (y + 1) * reductionFactor, z * reductionFactor)));
                                tris.Add(new Tri(new Vector3Int(x, y, z - 1), new Vector3Int(x, y, z), new Vector3Int(x - 1, y, z), cd.getNodeColorAt(x * reductionFactor, (y + 1) * reductionFactor, z * reductionFactor)));
                            }
                            vertLocs[x - 1, y, z - 1].Add(new Vector3(x, y + offset, z) * reductionFactor);
                            vertLocs[x - 1, y, z].Add(new Vector3(x, y + offset, z) * reductionFactor);
                            vertLocs[x, y, z - 1].Add(new Vector3(x, y + offset, z) * reductionFactor);
                            vertLocs[x, y, z].Add(new Vector3(x, y + offset, z) * reductionFactor);
                        }
                        else
                        {
                            // Add data for vertex smoothing but nothing else
                            if (x - 1 >= 0 && z - 1 >= 0)
                            {
                                vertLocs[x - 1, y, z - 1].Add(new Vector3(x, y + offset, z) * reductionFactor);
                            }
                            if (x - 1 >= 0 && z < nodeGridSize + 1)
                            {
                                vertLocs[x - 1, y, z].Add(new Vector3(x, y + offset, z) * reductionFactor);
                            }
                            if (x < nodeGridSize + 1 && z - 1 >= 0)
                            {
                                vertLocs[x, y, z - 1].Add(new Vector3(x, y + offset, z) * reductionFactor);
                            }
                            if (x < nodeGridSize + 1 && z < nodeGridSize + 1)
                            {
                                vertLocs[x, y, z].Add(new Vector3(x, y + offset, z) * reductionFactor);
                            }
                        }
                    }
                }
            }
        }

        // Z-dominant edges
        for (int x = 0; x < nodeGridSize + 2; x++)
        {
            for (int y = 0; y < nodeGridSize + 2; y++)
            {
                for (int z = 0; z < nodeGridSize + 1; z++)
                {
                    // Edge = (x,y,z)-(x+1,y,z)
                    // Edge in actual coords is multiplied by reductionFactor
                    if (nodesLod[x, y, z] != nodesLod[x, y, z + 1])
                    {
                        //float offset = nodesLodS[x, y, z] / (nodesLodS[x, y, z] - nodesLodS[x, y, z + 1]);
                        float offset = 0.5f;
                        if (x != 0 && x != nodeGridSize + 1 && y != 0 && y != nodeGridSize + 1)
                        {
                            // There's a rect around this! Tri1 = --, -+, +-, Tri2 = ++, +-, -+
                            if (nodesLod[x, y, z] > nodesLod[x, y, z + 1])
                            {
                                tris.Add(new Tri(new Vector3Int(x - 1, y, z), new Vector3Int(x - 1, y - 1, z), new Vector3Int(x, y - 1, z), cd.getNodeColorAt(x * reductionFactor, y * reductionFactor, z * reductionFactor)));
                                tris.Add(new Tri(new Vector3Int(x, y - 1, z), new Vector3Int(x, y, z), new Vector3Int(x - 1, y, z), cd.getNodeColorAt(x * reductionFactor, y * reductionFactor, z * reductionFactor)));
                            }
                            else
                            {
                                tris.Add(new Tri(new Vector3Int(x - 1, y - 1, z), new Vector3Int(x - 1, y, z), new Vector3Int(x, y - 1, z), cd.getNodeColorAt(x * reductionFactor, y * reductionFactor, (z + 1) * reductionFactor)));
                                tris.Add(new Tri(new Vector3Int(x, y, z), new Vector3Int(x, y - 1, z), new Vector3Int(x - 1, y, z), cd.getNodeColorAt(x * reductionFactor, y * reductionFactor, (z + 1) * reductionFactor)));
                            }
                            vertLocs[x - 1, y - 1, z].Add(new Vector3(x, y, z + offset) * reductionFactor);
                            vertLocs[x, y - 1, z].Add(new Vector3(x, y, z + offset) * reductionFactor);
                            vertLocs[x - 1, y, z].Add(new Vector3(x, y, z + offset) * reductionFactor);
                            vertLocs[x, y, z].Add(new Vector3(x, y, z + offset) * reductionFactor);
                        }
                        else
                        {
                            // Add data for vertex smoothing but nothing else
                            if (x - 1 >= 0 && y - 1 >= 0)
                            {
                                vertLocs[x - 1, y - 1, z].Add(new Vector3(x, y, z + offset) * reductionFactor);
                            }
                            if (x - 1 >= 0 && y < nodeGridSize + 1)
                            {
                                vertLocs[x - 1, y, z].Add(new Vector3(x, y, z + offset) * reductionFactor);
                            }
                            if (x < nodeGridSize + 1 && y - 1 >= 0)
                            {
                                vertLocs[x, y - 1, z].Add(new Vector3(x, y, z + offset) * reductionFactor);
                            }
                            if (x < nodeGridSize + 1 && y < nodeGridSize + 1)
                            {
                                vertLocs[x, y, z].Add(new Vector3(x, y, z + offset) * reductionFactor);
                            }
                        }
                    }
                }
            }
        }

        /**
         * STEP 3: Adjust vertices based on edge crossings to smooth out the surface
         */

        Vector3[,,] finalVertLocs = new Vector3[nodeGridSize + 1, nodeGridSize + 1, nodeGridSize + 1];
        for (int x = 0; x < nodeGridSize + 1; x++)
        {
            for (int y = 0; y < nodeGridSize + 1; y++)
            {
                for (int z = 0; z < nodeGridSize + 1; z++)
                {
                    if (vertLocs[x, y, z].Count == 0 || cuboid)
                    {
                        finalVertLocs[x, y, z] = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f) * reductionFactor;
                    }
                    else
                    {
                        Vector3 sum = new Vector3(0, 0, 0);
                        for (int i = 0; i < vertLocs[x,y,z].Count; i++)
                        {
                            sum += vertLocs[x, y, z][i];
                        }
                        finalVertLocs[x, y, z] = sum / vertLocs[x, y, z].Count;
                    }
                }
            }
        }

        /**
         * STEP 4: Parse through and unpack triangles
         */

        List<Vector3> vertList = new List<Vector3>();
        List<Vector3> normList = new List<Vector3>();
        List<Color32> colList = new List<Color32>();
        List<int> triList = new List<int>();

        for (int i = 0; i < tris.Count; i++)
        {
            int vert0Indx = vertList.Count;
            vertList.Add(finalVertLocs[tris[i].a.x, tris[i].a.y, tris[i].a.z]);
            vertList.Add(finalVertLocs[tris[i].b.x, tris[i].b.y, tris[i].b.z]);
            vertList.Add(finalVertLocs[tris[i].c.x, tris[i].c.y, tris[i].c.z]);
            for (int j = 0; j < 3; j++)
            {
                normList.Add(tris[i].normal);
                colList.Add(tris[i].color);
            }
            triList.Add(vert0Indx);
            triList.Add(vert0Indx + 1);
            triList.Add(vert0Indx + 2);
        }

        /**
         * STEP 5: Construct and return MeshData
         */

        MeshData ret = new MeshData(vertList.ToArray(), normList.ToArray(), triList.ToArray(), colList.ToArray());
        return ret;
    }

    void render(MeshData md)
    {
        // basically unpack the MeshData struct into usable shit for the rendering engine
        Mesh mesh = new Mesh();
        mf.mesh = mesh;
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.SetVertices(md.vertices);
        mesh.SetTriangles(md.triangles, 0);
        mesh.SetNormals(md.normals);
        mesh.SetColors(md.colors, 0, md.colors.Length);
    }
    
    // LOD can range from 0 (no smoothing) to 4 (max smoothing), any smaller and the game segfaults due to how chunk data is stored internally
    public void fullRender(int lod)
    {
        cd = GetComponent<ChunkData>();
        mf = GetComponent<MeshFilter>();
        nodes = cd.nodeData;
        MeshData generatedData = Generate(lod);
        render(generatedData);
    }
}
