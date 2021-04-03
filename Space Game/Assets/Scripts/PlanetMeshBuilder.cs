using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingCubesProject;

public class PlanetMeshBuilder : MonoBehaviour
{
    BlockStorage storage;
    TerrainGen tgen;
    ChunkInfo info;
    public Material terrainMaterial;
    public float scaleValue = 1;
    public PhysicMaterial physicMaterial;

    void BuildMesh()
    {
        List<GameObject> meshes = new List<GameObject>();

        //Marching marching = new MarchingCubes();
        Marching marching = new MarchingCubes();

        //Surface is the value that represents the surface of mesh
        //For example the perlin noise has a range of -1 to 1 so the mid point is where we want the surface to cut through.
        //The target value does not have to be the mid point it can be any value with in the range.
        marching.Surface = 0.5f;

        //The size of voxel array.
        int width, height, length;
        width = 34;
        height = 34;
        length = 34;

        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();
        List<Color> colors = new List<Color>();

        //The mesh produced is not optimal. There is one vert for each index.
        //Would need to weld vertices for better quality mesh.
        marching.Generate(storage.formatNodes(), width, height, length, verts, indices);
        for (int i=0; i<verts.Count; i += 3)
        {
            Vector3 avgTri = (verts[i]+verts[i+1]+verts[i+2])/3;
            Color avgColor = (tgen.GetColorAt(avgTri + (info.position * 32)));
            colors.Add(avgColor);
            colors.Add(avgColor);
            colors.Add(avgColor);
        }

        //Added by Kaden: Downscale vertices
        for (int i=0; i<verts.Count; i++)
        {
            verts[i] *= scaleValue;
            verts[i] *= info.scale;
        }

        //A mesh in unity can only be made up of 65000 verts.
        //Need to split the verts between multiple meshes.

        int maxVertsPerMesh = 30000; //must be divisible by 3, ie 3 verts == 1 triangle
        int numMeshes = verts.Count / maxVertsPerMesh + 1;

        for (int i = 0; i < numMeshes; i++)
        {

            List<Vector3> splitVerts = new List<Vector3>();
            List<int> splitIndices = new List<int>();
            List<Color> splitCols = new List<Color>();

            for (int j = 0; j < maxVertsPerMesh; j++)
            {
                int idx = i * maxVertsPerMesh + j;

                if (idx < verts.Count)
                {
                    splitVerts.Add(verts[idx]);
                    splitIndices.Add(j);
                    splitCols.Add(colors[idx]);
                }
            }

            if (splitVerts.Count == 0) continue;

            Mesh mesh = new Mesh();
            mesh.SetVertices(splitVerts);
            mesh.SetTriangles(splitIndices, 0);
            mesh.SetColors(splitCols);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            GameObject go = new GameObject("Mesh");
            go.transform.parent = transform;
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            go.GetComponent<Renderer>().material = terrainMaterial;
            go.GetComponent<MeshFilter>().mesh = mesh;
            //go.transform.localPosition = new Vector3(-32 / 2 * scaleValue, -32 / 2 * scaleValue, -32 / 2 * scaleValue);
            go.transform.localPosition = new Vector3(0, 0, 0);

            go.AddComponent<MeshCollider>();
            go.GetComponent<MeshCollider>().material = physicMaterial;
            go.GetComponent<MeshCollider>().sharedMesh = mesh;

            /*go.AddComponent<Rigidbody>();
            go.GetComponent<Rigidbody>().isKinematic = true;
            go.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            go.GetComponent<Rigidbody>().useGravity = false;*/

            meshes.Add(go);
        }
    }

    void Generate()
    {
        for (int x=-18; x< 18; x++)
        {
            for (int y=-18; y< 18; y++)
            {
                for (int z=-18; z< 18; z++)
                {
                    Vector3Int posVec = new Vector3Int(x, y, z);
                    storage.SetBlock(posVec, tgen.GetNodeAt(posVec + info.position*32));
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        storage = GetComponent<BlockStorage>();
        info = GetComponent<ChunkInfo>();
        tgen = info.planetTerrain;
        Generate();
        BuildMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
