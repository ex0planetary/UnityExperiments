using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMeshMaker : MonoBehaviour
{
    public Vector3Int center;
    public GameObject planet;
    public int LOD = 0;
    //format: r,p,t
    List<Vector3> unwarpedVerts = new List<Vector3>();
    List<int> triangleUnsorted = new List<int>();
    List<Color32> colors = new List<Color32>();
    List<Vector3> normalList = new List<Vector3>();
    Mesh mesh;
    StoreBlocks blocks;
    Transform trans;

    public void DestroyChunk()
    {
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        blocks = GetComponent<StoreBlocks>();
        trans = GetComponent<Transform>().transform;
        int minmin;
        int minmax;
        int maxmin;
        int maxmax;

        for (int x=-4; x<= 4; x++)
        {
            for (int y=-4; y<= 4; y++)
            {
                for (int z=-4; z<= 4; z++)
                {
                    if (!blocks.Transparent(x,y,z))
                    {
                        //try to render block
                        //x: y,z
                        //y: x,z
                        //z: x,y
                        //+x face
                        if (blocks.Transparent(x + 1, y, z))
                        {
                            minmin = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x+0.5f,y-0.5f,z-0.5f));
                            minmax = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x+0.5f,y-0.5f,z+0.5f));
                            maxmax = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x+0.5f,y+0.5f,z+0.5f));
                            maxmin = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x+0.5f,y+0.5f,z-0.5f));
                            triangleUnsorted.Add(maxmin);
                            triangleUnsorted.Add(minmax);
                            triangleUnsorted.Add(minmin);
                            triangleUnsorted.Add(maxmin);
                            triangleUnsorted.Add(maxmax);
                            triangleUnsorted.Add(minmax);
                        }
                        //-x face
                        if (blocks.Transparent(x - 1, y, z))
                        {
                            minmin = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x-0.5f,y-0.5f,z-0.5f));
                            minmax = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x-0.5f,y-0.5f,z+0.5f));
                            maxmax = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x-0.5f,y+0.5f,z+0.5f));
                            maxmin = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x-0.5f,y+0.5f,z-0.5f));
                            triangleUnsorted.Add(minmin);
                            triangleUnsorted.Add(minmax);
                            triangleUnsorted.Add(maxmin);
                            triangleUnsorted.Add(minmax);
                            triangleUnsorted.Add(maxmax);
                            triangleUnsorted.Add(maxmin);
                        }
                        //+y face
                        if (blocks.Transparent(x, y + 1, z))
                        {
                            minmin = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x-0.5f,y+0.5f,z-0.5f));
                            minmax = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x-0.5f,y+0.5f,z+0.5f));
                            maxmax = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x+0.5f,y+0.5f,z+0.5f));
                            maxmin = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x+0.5f,y+0.5f,z-0.5f));
                            triangleUnsorted.Add(minmin);
                            triangleUnsorted.Add(minmax);
                            triangleUnsorted.Add(maxmin);
                            triangleUnsorted.Add(minmax);
                            triangleUnsorted.Add(maxmax);
                            triangleUnsorted.Add(maxmin);
                        }
                        //-y face
                        if (blocks.Transparent(x, y - 1, z))
                        {
                            minmin = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x-0.5f,y-0.5f,z-0.5f));
                            minmax = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x-0.5f,y-0.5f,z+0.5f));
                            maxmax = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x+0.5f,y-0.5f,z+0.5f));
                            maxmin = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x+0.5f,y-0.5f,z-0.5f));
                            triangleUnsorted.Add(maxmin);
                            triangleUnsorted.Add(minmax);
                            triangleUnsorted.Add(minmin);
                            triangleUnsorted.Add(maxmin);
                            triangleUnsorted.Add(maxmax);
                            triangleUnsorted.Add(minmax);
                        }
                        //+z face
                        if (blocks.Transparent(x, y, z + 1))
                        {
                            minmin = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x-0.5f,y-0.5f,z+0.5f));
                            minmax = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x-0.5f,y+0.5f,z+0.5f));
                            maxmax = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x+0.5f,y+0.5f,z+0.5f));
                            maxmin = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x+0.5f,y-0.5f,z+0.5f));
                            triangleUnsorted.Add(maxmin);
                            triangleUnsorted.Add(minmax);
                            triangleUnsorted.Add(minmin);
                            triangleUnsorted.Add(maxmin);
                            triangleUnsorted.Add(maxmax);
                            triangleUnsorted.Add(minmax);
                        }
                        //-z face
                        if (blocks.Transparent(x, y, z - 1))
                        {
                            minmin = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x-0.5f,y-0.5f,z-0.5f));
                            minmax = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x-0.5f,y+0.5f,z-0.5f));
                            maxmax = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x+0.5f,y+0.5f,z-0.5f));
                            maxmin = unwarpedVerts.Count;
                            unwarpedVerts.Add(new Vector3(x+0.5f,y-0.5f,z-0.5f));
                            triangleUnsorted.Add(minmin);
                            triangleUnsorted.Add(minmax);
                            triangleUnsorted.Add(maxmin);
                            triangleUnsorted.Add(minmax);
                            triangleUnsorted.Add(maxmax);
                            triangleUnsorted.Add(maxmin);
                        }
                        while (colors.Count < unwarpedVerts.Count)
                        {
                            colors.Add(blocks.GetTerrain(x, y, z));
                        }
                    }
                }
            }
        }
        Vector3[] newVerts = new Vector3[unwarpedVerts.Count];
        float tx, ty, tz, dist;
        for (int i=0; i<unwarpedVerts.Count; i++)
        {
            unwarpedVerts[i] += center;
        }
        for (int i=0; i< unwarpedVerts.Count; i++)
        {
            dist = Mathf.Max(Mathf.Abs(unwarpedVerts[i].x), Mathf.Abs(unwarpedVerts[i].y), Mathf.Abs(unwarpedVerts[i].z));
            tx = unwarpedVerts[i].x / dist;
            ty = unwarpedVerts[i].y / dist;
            tz = unwarpedVerts[i].z / dist;
            //newVerts[i].x = tx * totalSize;
            //newVerts[i].y = ty * totalSize;
            //newVerts[i].z = tz * totalSize;
            newVerts[i].x = tx * Mathf.Sqrt(1 - (Mathf.Pow(ty, 2) / 2) - (Mathf.Pow(tz, 2) / 2) + (Mathf.Pow(ty, 2) * Mathf.Pow(tz, 2) / 3)) * dist;
            newVerts[i].y = ty * Mathf.Sqrt(1 - (Mathf.Pow(tz, 2) / 2) - (Mathf.Pow(tx, 2) / 2) + (Mathf.Pow(tz, 2) * Mathf.Pow(tx, 2) / 3)) * dist;
            newVerts[i].z = tz * Mathf.Sqrt(1 - (Mathf.Pow(tx, 2) / 2) - (Mathf.Pow(ty, 2) / 2) + (Mathf.Pow(tx, 2) * Mathf.Pow(ty, 2) / 3)) * dist;
        }
        //convert triangles
        int[] newTriangles = new int[triangleUnsorted.Count];
        for (int i=0; i<triangleUnsorted.Count; i++)
        {
            newTriangles[i] = triangleUnsorted[i];
        }
        //colors
        Color32[] newColors = new Color32[unwarpedVerts.Count];
        for (int i=0; i<colors.Count; i++)
        {
            newColors[i] = colors[i];
        }
        //assign mesh
        mesh.Clear();
        mesh.vertices = newVerts;
        //mesh.normals = newNormals;
        mesh.colors32 = newColors;
        mesh.triangles = newTriangles;
        mesh.RecalculateNormals();
        //Debug.Log("Vertices" + mesh.vertexCount);
        //Debug.Log("Tris" + mesh.triangles.Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
