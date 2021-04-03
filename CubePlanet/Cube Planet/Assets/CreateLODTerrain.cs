using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLODTerrain : MonoBehaviour
{
    public TerrainGen p_terrainGen;
    public int subdivisions = 3;
    private Mesh initMesh;
    public Transform cameraTransform;

    Vector3 sphToRec(Vector3 sph)
    {
        return new Vector3(sph.x * Mathf.Sin(sph.y) * Mathf.Cos(sph.z), sph.x * Mathf.Cos(sph.y), sph.x * Mathf.Sin(sph.y) * Mathf.Sin(sph.z));
    }

    Vector3 recToSph(Vector3 rec)
    {
        Vector3 sph;
        sph.x = Mathf.Sqrt(rec.x * rec.x + rec.y * rec.y + rec.z * rec.z);
        sph.y = Mathf.Acos(rec.y / sph.x);
        sph.z = Mathf.Atan2(rec.z, rec.x);
        return sph;
    }

    float bsign(float i)
    {
        if (i == 0) return 0;
        return Mathf.Sign(i);
    }

    float babs(float i)
    {
        return i * bsign(i);
    }

    const float isqrt2 = 0.707f;

    Vector3 cubify(Vector3 s)
    {
        float xx2 = s.x * s.x * 2.0f;
        float yy2 = s.y * s.y * 2.0f;

        Vector2 v = new Vector2(xx2 - yy2, yy2 - xx2);

        float ii = v.y - 3.0f;
        ii *= ii;

    float isqrt = -Mathf.Sqrt(ii - 12.0f * xx2) + 3.0f;

        v.x = Mathf.Sqrt(v.x + isqrt);
        v.y = Mathf.Sqrt(v.y + isqrt);
        v *= isqrt2;
        Vector3 result = new Vector3(bsign(s.x) * v.x, bsign(s.y) * v.y, bsign(s.z));
        if (float.IsNaN(result.x)) result.x = 0;
        if (float.IsNaN(result.y)) result.y = 0;
        if (float.IsNaN(result.z)) result.z = 0;
        return result;
    }

    Vector3 sphere2cube(Vector3 sphere)
    {
        Vector3 f = new Vector3(babs(sphere.x), babs(sphere.y), babs(sphere.z));

        bool a = f.y >= f.x && f.y >= f.z;
        bool b = f.x >= f.z;
        Debug.Log(cubify(new Vector3(sphere.y, sphere.z, sphere.x)));
        return a ? new Vector3(cubify(new Vector3(sphere.x, sphere.z, sphere.y)).x, cubify(new Vector3(sphere.x, sphere.z, sphere.y)).z, cubify(new Vector3(sphere.x, sphere.z, sphere.y)).y) : b ? new Vector3(cubify(new Vector3(sphere.y, sphere.z, sphere.x)).z, cubify(new Vector3(sphere.y, sphere.z, sphere.x)).x, cubify(new Vector3(sphere.y, sphere.z, sphere.x)).y) : cubify(sphere);
    }

    Vector3 sphere2cubeException(Vector3 sphere)
    {
        Vector3 result = sphere2cube(sphere);
        if (float.IsNaN(result.x)) result.x = 0;
        if (float.IsNaN(result.y)) result.y = 0;
        if (float.IsNaN(result.z)) result.z = 0;
        return result;
    }

    private Transform localTrans;
    private void Start()
    {
        localTrans = GetComponent<Transform>().transform;
        List<Vector3> vertList = new List<Vector3>();
        List<int> triList = new List<int>();

        int subdivRemaining = subdivisions;
        // initialize with tetrahedral mesh

        /*vertList.Add(new Vector3(0, Mathf.Sqrt(2 / 3), 0));
        vertList.Add(new Vector3((-1f) / (2f * Mathf.Sqrt(3)), 0f, 0.5f));
        vertList.Add(new Vector3((-1f) / (2f * Mathf.Sqrt(3)), 0f, -0.5f));
        vertList.Add(new Vector3((1f) / (Mathf.Sqrt(3)), 0, 0));*/
        vertList.Add(new Vector3(0, 0.612f, 0));
        vertList.Add(new Vector3(-0.289f, -0.204f, 0.5f));
        vertList.Add(new Vector3(-0.289f, -0.204f, -0.5f));
        vertList.Add(new Vector3(0.577f, -0.204f, 0));

        triList.Add(0);
        triList.Add(2);
        triList.Add(1);
        triList.Add(0);
        triList.Add(3);
        triList.Add(2);
        triList.Add(0);
        triList.Add(1);
        triList.Add(3);
        triList.Add(1);
        triList.Add(2);
        triList.Add(3);

        int numTris = 4;

        while (subdivRemaining > 0)
        {
            int startTris = numTris;
            for (int i = 0; i < startTris; i++)
            {
                // subdivide triangle
                // add new vertices and balance to sphere
                int a = triList[3 * i];
                int b = triList[3 * i + 1];
                int c = triList[3 * i + 2];
                Vector3 newVert = vertList[a] + vertList[b];
                Vector3 newVertSph = recToSph(newVert);
                Vector3 newVertRound = sphToRec(new Vector3(1, newVertSph.y, newVertSph.z));
                vertList.Add(newVertRound);
                newVert = vertList[b] + vertList[c];
                newVertSph = recToSph(newVert);
                newVertRound = sphToRec(new Vector3(1, newVertSph.y, newVertSph.z));
                vertList.Add(newVertRound);
                newVert = vertList[c] + vertList[a];
                newVertSph = recToSph(newVert);
                newVertRound = sphToRec(new Vector3(1, newVertSph.y, newVertSph.z));
                vertList.Add(newVertRound);
                int x = vertList.Count - 3;
                int y = vertList.Count - 2;
                int z = vertList.Count - 1;
                triList[3 * i] = x;
                triList[3 * i + 1] = y;
                triList[3 * i + 2] = z;
                triList.Add(a);
                triList.Add(x);
                triList.Add(z);
                triList.Add(b);
                triList.Add(y);
                triList.Add(x);
                triList.Add(c);
                triList.Add(z);
                triList.Add(y);
                numTris += 4;
            }
            subdivRemaining -= 1;
        }

        Vector3[] initVerts = new Vector3[vertList.Count];
        Vector3[] initNorms = new Vector3[vertList.Count];
        Color32[] initColors = new Color32[vertList.Count];
        List<Vector3> normList = new List<Vector3>();
        int[] initTris = new int[triList.Count];
        for (int i = 0; i < vertList.Count; i++)
        {
            initVerts[i] = vertList[i];
            initNorms[i] = Vector3.Normalize(vertList[i]);
            normList.Add(Vector3.Normalize(vertList[i]));
            //vertList[i] = normList[i] * p_terrainGen.getHeightAt(normList[i].x, normList[i].y, normList[i].z);
            //terrain gen
            Debug.Log("Another one");
            Debug.Log(normList[i]);
            Vector3 blockPos = sphere2cubeException(normList[i]);
            Debug.Log(blockPos);
            float height = p_terrainGen.getHeightAt(blockPos.x, blockPos.y, blockPos.z);
            Debug.Log(height);
            vertList[i] *= height;
            Vector3 blockPosTop = sphere2cubeException(normList[i]*height);
            initColors[i] = height > p_terrainGen.mountainHeight ? p_terrainGen.GetBiomeAt(blockPosTop.x, blockPosTop.y, blockPosTop.z).colorMtns : p_terrainGen.GetBiomeAt(blockPosTop.x, blockPosTop.y, blockPosTop.z).colorGrass;
        }
        for (int i = 0; i < triList.Count; i++)
        {
            initTris[i] = triList[i];
        }
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        /*mesh.vertices = initVerts;
        mesh.triangles = initTris;
        mesh.normals = initNorms;*/
        mesh.SetVertices(vertList);
        mesh.SetTriangles(triList,0);
        mesh.RecalculateNormals();
        mesh.colors32 = initColors;
        initMesh = mesh;
        //mesh.SetNormals(normList);
    }
    private int ticks = 0;
    public int updateFrequency = 60;
    public float startClipping = 350;
    public float stopClipping = 150;
    public void DestroyLod()
    {
        Destroy(gameObject);
    }
    private void Update()
    {
        ticks++;
        if (ticks >= updateFrequency)
        {
            ticks = 0;
        }
        if (ticks == 0 && (localTrans.position - cameraTransform.position).magnitude < startClipping && (localTrans.position - cameraTransform.position).magnitude > stopClipping)
        {
            Mesh meshRemoved = initMesh;
            List<int> vertsToRemove = new List<int>();
            for (int i = 0; i < initMesh.vertexCount; i++)
            {
                if (((localTrans.TransformPoint(initMesh.vertices[i])) - cameraTransform.position).magnitude < 229)
                {
                    int j = i;
                    vertsToRemove.Add(j);
                }
            }
            List<int> newTris = new List<int>();
            for (int i = 0; i < initMesh.triangles.Length; i += 3)
            {
                bool doesTriWork = true;
                for (int j = 0; j < 3; j++)
                {
                    if (vertsToRemove.Contains(initMesh.triangles[i + j])) doesTriWork = false;
                }
                if (doesTriWork)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        newTris.Add(initMesh.triangles[i + j]);
                    }
                }
            }
            Mesh nmesh = new Mesh();
            GetComponent<MeshFilter>().mesh = nmesh;
            nmesh.vertices = initMesh.vertices;
            nmesh.normals = initMesh.normals;
            nmesh.SetTriangles(newTris, 0);
            nmesh.colors32 = initMesh.colors32;
            //meshRemoved.RecalculateNormals();
        }

    }
}
