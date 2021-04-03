using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BLOD : MonoBehaviour
{
    public TerrainGen terra;
    public Vector3Int center;
    public GameObject region;
    List<Vector3> unwarpedVerts = new List<Vector3>();
    List<int> triangleUnsorted = new List<int>();
    List<Color32> colors = new List<Color32>();
    List<Vector3> normalList = new List<Vector3>();
    Mesh mesh;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
            //temporarily full LOD
            Vector3 centerPol = terra.cubeToPolar(center.x, center.y, center.z);
            Color32 color = terra.getColorAt(center.x * 27, center.y * 27, center.z * 27);
            if (color.a == 255)
            {
                int minmin;
                int minmax;
                int maxmin;
                int maxmax;
                int x = center.x * 27;
                int y = center.y * 27;
                int z = center.z * 27;
                if (terra.getColorAt(x + 27, y, z).a != 255)
                {
                    minmin = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x + 13.5f, y - 13.5f, z - 13.5f));
                    minmax = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x + 13.5f, y - 13.5f, z + 13.5f));
                    maxmax = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x + 13.5f, y + 13.5f, z + 13.5f));
                    maxmin = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x + 13.5f, y + 13.5f, z - 13.5f));
                    triangleUnsorted.Add(maxmin);
                    triangleUnsorted.Add(minmax);
                    triangleUnsorted.Add(minmin);
                    triangleUnsorted.Add(maxmin);
                    triangleUnsorted.Add(maxmax);
                    triangleUnsorted.Add(minmax);
                }
                if (terra.getColorAt(x - 27, y, z).a != 255)
                {
                    minmin = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x - 13.5f, y - 13.5f, z - 13.5f));
                    minmax = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x - 13.5f, y - 13.5f, z + 13.5f));
                    maxmax = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x - 13.5f, y + 13.5f, z + 13.5f));
                    maxmin = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x - 13.5f, y + 13.5f, z - 13.5f));
                    triangleUnsorted.Add(minmin);
                    triangleUnsorted.Add(minmax);
                    triangleUnsorted.Add(maxmin);
                    triangleUnsorted.Add(minmax);
                    triangleUnsorted.Add(maxmax);
                    triangleUnsorted.Add(maxmin);
                }
                if (terra.getColorAt(x, y + 27, z).a != 255)
                {
                    minmin = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x - 13.5f, y + 13.5f, z - 13.5f));
                    minmax = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x - 13.5f, y + 13.5f, z + 13.5f));
                    maxmax = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x + 13.5f, y + 13.5f, z + 13.5f));
                    maxmin = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x + 13.5f, y + 13.5f, z - 13.5f));
                    triangleUnsorted.Add(minmin);
                    triangleUnsorted.Add(minmax);
                    triangleUnsorted.Add(maxmin);
                    triangleUnsorted.Add(minmax);
                    triangleUnsorted.Add(maxmax);
                    triangleUnsorted.Add(maxmin);
                }
                if (terra.getColorAt(x, y - 27, z).a != 255)
                {
                    minmin = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x - 13.5f, y - 13.5f, z - 13.5f));
                    minmax = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x - 13.5f, y - 13.5f, z + 13.5f));
                    maxmax = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x + 13.5f, y - 13.5f, z + 13.5f));
                    maxmin = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x + 13.5f, y - 13.5f, z - 13.5f));
                    triangleUnsorted.Add(maxmin);
                    triangleUnsorted.Add(minmax);
                    triangleUnsorted.Add(minmin);
                    triangleUnsorted.Add(maxmin);
                    triangleUnsorted.Add(maxmax);
                    triangleUnsorted.Add(minmax);
                }
                if (terra.getColorAt(x, y, z + 27).a != 255)
                {
                    minmin = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x - 13.5f, y - 13.5f, z + 13.5f));
                    minmax = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x - 13.5f, y + 13.5f, z + 13.5f));
                    maxmax = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x + 13.5f, y + 13.5f, z + 13.5f));
                    maxmin = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x + 13.5f, y - 13.5f, z + 13.5f));
                    triangleUnsorted.Add(maxmin);
                    triangleUnsorted.Add(minmax);
                    triangleUnsorted.Add(minmin);
                    triangleUnsorted.Add(maxmin);
                    triangleUnsorted.Add(maxmax);
                    triangleUnsorted.Add(minmax);
                }
                if (terra.getColorAt(x, y, z - 27).a != 255)
                {
                    minmin = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x - 13.5f, y - 13.5f, z - 13.5f));
                    minmax = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x - 13.5f, y + 13.5f, z - 13.5f));
                    maxmax = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x + 13.5f, y + 13.5f, z - 13.5f));
                    maxmin = unwarpedVerts.Count;
                    unwarpedVerts.Add(new Vector3(x + 13.5f, y - 13.5f, z - 13.5f));
                    triangleUnsorted.Add(minmin);
                    triangleUnsorted.Add(minmax);
                    triangleUnsorted.Add(maxmin);
                    triangleUnsorted.Add(minmax);
                    triangleUnsorted.Add(maxmax);
                    triangleUnsorted.Add(maxmin);
                }
            }
            Vector3[] newVerts = new Vector3[unwarpedVerts.Count];
            float tx, ty, tz, dist;
            for (int i = 0; i < unwarpedVerts.Count; i++)
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
            for (int i = 0; i < triangleUnsorted.Count; i++)
            {
                newTriangles[i] = triangleUnsorted[i];
            }
            //colors
            Color32[] newColors = new Color32[unwarpedVerts.Count];
            for (int i = 0; i < unwarpedVerts.Count; i++)
            {
                newColors[i] = color;
            }
            //assign mesh
            mesh.Clear();
            mesh.vertices = newVerts;
            //mesh.normals = newNormals;
            mesh.colors32 = newColors;
            mesh.triangles = newTriangles;
            mesh.RecalculateNormals();
            LODGroup lg = region.GetComponent<LODGroup>();
            lg.RecalculateBounds();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
