using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MeshDataOld
{
    public Vector3[] vertices;
    public Vector3[] normals;
    public int[] triangles;
}

public struct NormData
{
    public int central;
    public int a;
    public int b;
    public NormData(int c, int ap, int bp)
    {
        central = c;
        a = ap;
        b = bp;
    }
}

public class SurfaceNets : MonoBehaviour
{
    private ChunkData cd;
    private MeshFilter mf;
    private int[,,] nodes = new int[32, 32, 32];
    private float[,,] density = new float[32, 32, 32];
    public int smoothingRad = 1;
    public bool cuboid = false;
    public int LOD = 0; // 0 = 32x, 1 = 16x, 2 = 8x, 3 = 4x, 4 = 2x, 5 = 1x, 6+ will break shit so don't do that

    void calculateDensity()
    {
        int ix, iy, iz, ax, ay, az;
        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 32; y++)
            {
                for (int z = 0; z < 32; z++)
                {
                    // average out the nodeness of all surrounding nodes - smoothing shit!
                    ix = x - smoothingRad;
                    iy = y - smoothingRad;
                    iz = z - smoothingRad;
                    ax = x + smoothingRad;
                    ay = y + smoothingRad;
                    az = z + smoothingRad;
                    if (ix <= 0) ix = 0;
                    if (iy <= 0) iy = 0;
                    if (iz <= 0) iz = 0;
                    if (ax >= 31) ax = 31;
                    if (ay >= 31) ay = 31;
                    if (az >= 31) az = 31;
                    density[x, y, z] = 0;
                    for (int x2 = ix; x2 <= ax; x2++)
                    {
                        for (int y2 = iy; y2 <= ay; y2++)
                        {
                            for (int z2 = iz; z2 <= az; z2++)
                            {
                                density[x, y, z] += nodes[x2, y2, z2];
                            }
                        }
                    }
                    // S T O R E
                    density[x, y, z] /= ((ax - ix + 1) * (ay - iy + 1) * (az - iz + 1));
                    //density[x, y, z] = nodes[x, y, z];
                }
            }
        }
    }

    int getVert(int x, int y, int z)
    {
        // this function converts xyz to an index in the vert/normal lists for gamering purposes
        return z + (y * 31) + (x * 31 * 31);
    }

    float uninterpolate(float a, float b, float target)
    {
        //interpolation: r=a(1-t)+b(t)=a-at+bt=t(b-a)+a
        // r-a=t(b-a)
        // t=(r-a)/(b-a)
        return (target - a) / (b - a);
    }

    MeshData generate()
    {
        /***
         * TODO: Finish coding lodStep functionality for LOD geometry reduction. Important, don't set LOD above 0 before this is done or else shit *will* break.
         */
        int lodStep = (int)Mathf.Pow(2, LOD);
        List<List<NormData>> normList = new List<List<NormData>>();
        List<Vector3> vertList = new List<Vector3>();
        List<List<float>> xInterpList = new List<List<float>>();
        List<List<float>> yInterpList = new List<List<float>>();
        List<List<float>> zInterpList = new List<List<float>>();
        List<List<Vector3>> interpList = new List<List<Vector3>>();
        for (int x = 0; x < 32 - lodStep; x += lodStep)
        {
            for (int y = 0; y < 32 - lodStep; y += lodStep)
            {
                for (int z = 0; z < 32 - lodStep; z += lodStep)
                {
                    // add all the initial vertex locations while also initializing the normal lists
                    normList.Add(new List<NormData>());
                    xInterpList.Add(new List<float>());
                    yInterpList.Add(new List<float>());
                    zInterpList.Add(new List<float>());
                    interpList.Add(new List<Vector3>());
                    //Vector3 localPos = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
                    //vertList.Add(localPos + cd.location);
                }
            }
        }
        List<int> triList = new List<int>();
        // triangles are ordered clockwise
        // x-direction edges
        NormData tempNorm;
        float tempInterp;
        Vector3 tempInterpVec;
        for (int y = 1; y < 31; y += lodStep)
        {
            for (int z = 1; z < 31; z += lodStep)
            {
                for (int x = 0; x < 31; x += lodStep)
                {
                    // edge: (x,y,z) to (x+1,y,z)
                    if (Mathf.Sign(density[x,y,z]) != Mathf.Sign(density[x+1,y,z]))
                    {
                        // can confirm there is an edge here
                        if (density[x, y, z] < density[x + 1, y, z])
                        {
                            // -x normal
                            triList.Add(getVert(x, y - 1, z - 1));
                            triList.Add(getVert(x, y - 1, z));
                            triList.Add(getVert(x, y, z - 1));
                            triList.Add(getVert(x, y - 1, z));
                            triList.Add(getVert(x, y, z));
                            triList.Add(getVert(x, y, z - 1));
                            tempNorm = new NormData(getVert(x, y - 1, z - 1), getVert(x, y - 1, z), getVert(x, y, z - 1));
                        }
                        else
                        {
                            // +x normal
                            triList.Add(getVert(x, y, z - 1));
                            triList.Add(getVert(x, y, z));
                            triList.Add(getVert(x, y - 1, z));
                            triList.Add(getVert(x, y, z - 1));
                            triList.Add(getVert(x, y - 1, z));
                            triList.Add(getVert(x, y - 1, z - 1));
                            tempNorm = new NormData(getVert(x, y - 1, z - 1), getVert(x, y, z - 1), getVert(x, y - 1, z));
                        }
                        // add the normals into the averaging equation
                        normList[getVert(x, y - 1, z - 1)].Add(tempNorm);
                        normList[getVert(x, y, z - 1)].Add(tempNorm);
                        normList[getVert(x, y - 1, z)].Add(tempNorm);
                        normList[getVert(x, y, z)].Add(tempNorm);
                        // add the interpolation value for vertex smoothing
                        tempInterp = uninterpolate(density[x, y, z], density[x + 1, y, z], 0);
                        tempInterpVec = new Vector3(x + tempInterp, y, z);
                        xInterpList[getVert(x, y - 1, z - 1)].Add(tempInterp);
                        xInterpList[getVert(x, y, z - 1)].Add(tempInterp);
                        xInterpList[getVert(x, y - 1, z)].Add(tempInterp);
                        xInterpList[getVert(x, y, z)].Add(tempInterp);
                        interpList[getVert(x, y - 1, z - 1)].Add(tempInterpVec);
                        interpList[getVert(x, y, z - 1)].Add(tempInterpVec);
                        interpList[getVert(x, y - 1, z)].Add(tempInterpVec);
                        interpList[getVert(x, y, z)].Add(tempInterpVec);
                        // add quad: (x,y-1,z-1),(x,y,z-1),(x,y-1,z),(x,y,z)
                        Debug.Log("Added x quad at " + x + "," + y + "," + z);
                    }
                }
            }
        }
        // y-direction edges
        for (int x = 1; x < 31; x += lodStep)
        {
            for (int z = 1; z < 31; z += lodStep)
            {
                for (int y = 0; y < 31; y += lodStep)
                {
                    // edge: (x,y,z) to (x,y+1,z)
                    if (Mathf.Sign(density[x, y, z]) != Mathf.Sign(density[x, y + 1, z]))
                    {
                        if (density[x, y, z] < density[x, y + 1, z])
                        {
                            // -y normal
                            triList.Add(getVert(x, y, z - 1));
                            triList.Add(getVert(x, y, z));
                            triList.Add(getVert(x - 1, y, z));
                            triList.Add(getVert(x, y, z - 1));
                            triList.Add(getVert(x - 1, y, z));
                            triList.Add(getVert(x - 1, y, z - 1));
                            tempNorm = new NormData(getVert(x - 1, y, z - 1), getVert(x, y, z - 1), getVert(x - 1, y, z));
                        }
                        else
                        {
                            // +y normal
                            triList.Add(getVert(x - 1, y, z - 1));
                            triList.Add(getVert(x - 1, y, z));
                            triList.Add(getVert(x, y, z - 1));
                            triList.Add(getVert(x - 1, y, z));
                            triList.Add(getVert(x, y, z));
                            triList.Add(getVert(x, y, z - 1));
                            tempNorm = new NormData(getVert(x - 1, y, z - 1), getVert(x - 1, y, z), getVert(x, y, z - 1));
                        }
                        normList[getVert(x - 1, y, z - 1)].Add(tempNorm);
                        normList[getVert(x, y, z - 1)].Add(tempNorm);
                        normList[getVert(x - 1, y, z)].Add(tempNorm);
                        normList[getVert(x, y, z)].Add(tempNorm);
                        tempInterp = uninterpolate(density[x, y, z], density[x, y + 1, z], 0);
                        tempInterpVec = new Vector3(x, y + tempInterp, z);
                        yInterpList[getVert(x - 1, y, z - 1)].Add(tempInterp);
                        yInterpList[getVert(x, y, z - 1)].Add(tempInterp);
                        yInterpList[getVert(x - 1, y, z)].Add(tempInterp);
                        yInterpList[getVert(x, y, z)].Add(tempInterp);
                        interpList[getVert(x - 1, y, z - 1)].Add(tempInterpVec);
                        interpList[getVert(x, y, z - 1)].Add(tempInterpVec);
                        interpList[getVert(x - 1, y, z)].Add(tempInterpVec);
                        interpList[getVert(x, y, z)].Add(tempInterpVec);
                        // add quad: (x-1,y,z-1),(x,y,z-1),(x-1,y,z),(x,y,z)
                        Debug.Log("Added y quad at " + x + "," + y + "," + z);
                    }
                }
            }
        }
        // z-direction edges
        for (int y = 1; y < 31; y += lodStep)
        {
            for (int x = 1; x < 31; x += lodStep)
            {
                for (int z = 0; z < 31; z += lodStep)
                {
                    // edge: (x,y,z) to (x,y,z+1)
                    if (Mathf.Sign(density[x, y, z]) != Mathf.Sign(density[x, y, z + 1]))
                    {
                        if (density[x, y, z] < density[x, y, z + 1])
                        {
                            // -z normal
                            triList.Add(getVert(x - 1, y, z));
                            triList.Add(getVert(x, y, z));
                            triList.Add(getVert(x, y - 1, z));
                            triList.Add(getVert(x - 1, y, z));
                            triList.Add(getVert(x, y - 1, z));
                            triList.Add(getVert(x - 1, y - 1, z));
                            tempNorm = new NormData(getVert(x - 1, y - 1, z), getVert(x - 1, y, z), getVert(x, y - 1, z));
                        }
                        else
                        {
                            // +z normal
                            triList.Add(getVert(x - 1, y - 1, z));
                            triList.Add(getVert(x, y - 1, z));
                            triList.Add(getVert(x - 1, y, z));
                            triList.Add(getVert(x, y - 1, z));
                            triList.Add(getVert(x, y, z));
                            triList.Add(getVert(x - 1, y, z));
                            tempNorm = new NormData(getVert(x - 1, y - 1, z), getVert(x, y - 1, z), getVert(x - 1, y, z));
                        }
                        normList[getVert(x - 1, y - 1, z)].Add(tempNorm);
                        normList[getVert(x, y - 1, z)].Add(tempNorm);
                        normList[getVert(x - 1, y, z)].Add(tempNorm);
                        normList[getVert(x, y, z)].Add(tempNorm);
                        tempInterp = uninterpolate(density[x, y, z], density[x, y, z + 1], 0);
                        tempInterpVec = new Vector3(x, y, z + tempInterp);
                        zInterpList[getVert(x - 1, y - 1, z)].Add(tempInterp);
                        zInterpList[getVert(x, y - 1, z)].Add(tempInterp);
                        zInterpList[getVert(x - 1, y, z)].Add(tempInterp);
                        zInterpList[getVert(x, y, z)].Add(tempInterp);
                        interpList[getVert(x - 1, y - 1, z)].Add(tempInterpVec);
                        interpList[getVert(x, y - 1, z)].Add(tempInterpVec);
                        interpList[getVert(x - 1, y, z)].Add(tempInterpVec);
                        interpList[getVert(x, y, z)].Add(tempInterpVec);
                        // add quad: (x-1,y-1,z),(x-1,y,z),(x,y-1,z),(x,y,z)
                        Debug.Log("Added z quad at " + x + "," + y + "," + z);
                    }
                }
            }
        }
        List<Vector3> normListAvg = new List<Vector3>();
        Vector3 sum = new Vector3(0, 0, 0);
        int count;
        // time to average up the shit!
        float tempSum;
        Vector3 tempSumVec;
        for (int x = 0; x < 31; x++)
        {
            for (int y = 0; y < 31; y++)
            {
                for (int z = 0; z < 31; z++)
                {
                    int i = getVert(x, y, z);
                    Vector3 localPos = new Vector3(x, y, z);
                    if (xInterpList[i].Count > 0)
                    {
                        tempSum = 0;
                        for (int j = 0; j < xInterpList[i].Count; j++)
                        {
                            tempSum += xInterpList[i][j];
                        }
                        localPos.x += (tempSum / xInterpList[i].Count);
                    }
                    else
                    {
                        localPos.x += 0.5f;
                    }
                    if (yInterpList[i].Count > 0)
                    {
                        tempSum = 0;
                        for (int j = 0; j < yInterpList[i].Count; j++)
                        {
                            tempSum += yInterpList[i][j];
                        }
                        localPos.y += (tempSum / yInterpList[i].Count);
                    }
                    else
                    {
                        localPos.y += 0.5f;
                    }
                    if (zInterpList[i].Count > 0)
                    {
                        tempSum = 0;
                        for (int j = 0; j < zInterpList[i].Count; j++)
                        {
                            tempSum += zInterpList[i][j];
                        }
                        localPos.z += (tempSum / zInterpList[i].Count);
                    }
                    else
                    {
                        localPos.z += 0.5f;
                    }
                    if (interpList[i].Count > 0)
                    {
                        tempSumVec = new Vector3(0, 0, 0);
                        for (int j = 0; j < interpList[i].Count; j++)
                        {
                            tempSumVec += interpList[i][j];
                        }
                        tempSumVec.x /= interpList[i].Count;
                        tempSumVec.y /= interpList[i].Count;
                        tempSumVec.z /= interpList[i].Count;
                        localPos = tempSumVec;
                    }
                    else
                    {
                        localPos = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
                    }
                    if (cuboid)
                        localPos = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
                    vertList.Add(localPos + cd.location);
                }
            }
        }
        for (int i = 0; i < normList.Count; i++)
        {
            sum = new Vector3(0, 0, 0);
            for (int j = 0; j < normList[i].Count; j++)
            {
                NormData nd = normList[i][j];
                sum += Vector3.Cross(Vector3.Normalize(vertList[nd.a] - vertList[nd.central]), Vector3.Normalize(vertList[nd.b] - vertList[nd.central]));
            }
            if (sum != new Vector3(0, 0, 0))
            {
                normListAvg.Add(sum / normList[i].Count);
            }
            else
            {
                // TODO: mark this with a flag to just get rid of these vertices, since they have no edges
                normListAvg.Add(Vector3.forward);
            }
        }
        // store all data into MeshData struct
        Vector3[] vertices = vertList.ToArray();
        int[] triangles = triList.ToArray();
        Vector3[] normals = normListAvg.ToArray();
        MeshData ret = new MeshData();
        ret.vertices = vertices;
        ret.triangles = triangles;
        ret.normals = normals;
        return ret;
    }

    void render(MeshData md)
    {
        // basically unpack the MeshData struct into usable shit for the rendering engine
        Mesh mesh = new Mesh();
        mf.mesh = mesh;
        mesh.SetVertices(md.vertices);
        mesh.SetTriangles(md.triangles,0);
        mesh.SetNormals(md.normals);
    }

    void Start()
    {
        // SHIT GETS REAL
        cd = GetComponent<ChunkData>();
        mf = GetComponent<MeshFilter>();
        nodes = cd.nodeData;
        calculateDensity();
        MeshData renderData = generate();
        render(renderData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
