using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODLvlCreation : MonoBehaviour
{
    public GameObject chunkPrefab;
    public GameObject lodPrefab;
    public GameObject blodPrefab;
    public Vector3Int center;
    public GameObject planet;
    LODGroup group;
    TerrainGen terra;
    Transform trans;
    Transform myTrans;
    // Start is called before the first frame update
    void Start()
    {
        group = GetComponent<LODGroup>();
        terra = planet.GetComponent<TerrainGen>();
        trans = planet.GetComponent<Transform>().transform;
        myTrans = GetComponent<Transform>().transform;
        int tdist = Mathf.Max(Mathf.Abs(center.x*3), Mathf.Abs(center.y * 3), Mathf.Abs(center.z * 3));
        if (tdist == 0)
        {
            myTrans.position = new Vector3(0, 0, 0);
        }
        else
        {
            float ttx = (center.x * 3) / tdist;
            float tty = (center.y * 3) / tdist;
            float ttz = (center.z * 3) / tdist;
            myTrans.position = new Vector3(ttx * Mathf.Sqrt(1 - (Mathf.Pow(tty, 2) / 2) - (Mathf.Pow(ttz, 2) / 2) + (Mathf.Pow(tty, 2) * Mathf.Pow(ttz, 2) / 3)) * tdist, tty * Mathf.Sqrt(1 - (Mathf.Pow(ttz, 2) / 2) - (Mathf.Pow(ttx, 2) / 2) + (Mathf.Pow(ttz, 2) * Mathf.Pow(ttx, 2) / 3)) * tdist, ttz * Mathf.Sqrt(1 - (Mathf.Pow(ttx, 2) / 2) - (Mathf.Pow(tty, 2) / 2) + (Mathf.Pow(ttx, 2) * Mathf.Pow(tty, 2) / 3)) * tdist);
            myTrans.Translate(trans.position);
        }
        LOD[] lods = new LOD[4];
        //create chunks and add to LOD 0
        Renderer[] lod0rend = new Renderer[27];
        for (int x=-1; x<=1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    GameObject chunk = (GameObject)Instantiate(chunkPrefab, myTrans);
                    chunk.GetComponent<Transform>().transform.Translate(-1 * myTrans.position);
                    chunk.GetComponent<Transform>().transform.Translate(trans.position);
                    PlanetMeshMaker mm = chunk.GetComponent<PlanetMeshMaker>();
                    mm.center = new Vector3Int(9 * (x+center.x), 9 * (y+center.y), 9 * (z+center.z));
                    mm.planet = gameObject;
                    StoreBlocks sb = chunk.GetComponent<StoreBlocks>();
                    for (int cx = -4; cx <= 4; cx++)
                    {
                        for (int cy = -4; cy <= 4; cy++)
                        {
                            for (int cz = -4; cz <= 4; cz++)
                            {
                                sb.ChangeAt(cx, cy, cz, terra.getColorAt((x + center.x) * 9 + cx, (y + center.y) * 9 + cy, (z + center.z) * 9 + cz));
                            }
                        }
                    }
                    lod0rend[(x + 1) * 9 + (y + 1) * 3 + (z + 1)] = chunk.GetComponent<Renderer>();
                }
            }
        }
        //create LOD 1 renderers
        Renderer[] lod1rend = new Renderer[27];
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    GameObject lodObj = (GameObject)Instantiate(lodPrefab, myTrans);
                    CLOD lod = lodObj.GetComponent<CLOD>();
                    lod.GetComponent<Transform>().transform.Translate(-1 * myTrans.position);
                    lod.GetComponent<Transform>().transform.Translate(trans.position);
                    lod.terra = terra;
                    lod.center = new Vector3Int((x + center.x), (y + center.y), (z + center.z));
                    lod.lod = 1;
                    lod.region = gameObject;
                    lod1rend[(x + 1) * 9 + (y + 1) * 3 + (z + 1)] = lodObj.GetComponent<Renderer>();
                }
            }
        }
        //create LOD 2 renderers
        Renderer[] lod2rend = new Renderer[27];
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    GameObject lodObj = (GameObject)Instantiate(lodPrefab, myTrans);
                    CLOD lod = lodObj.GetComponent<CLOD>();
                    lod.GetComponent<Transform>().transform.Translate(-1 * myTrans.position);
                    lod.GetComponent<Transform>().transform.Translate(trans.position);
                    lod.terra = terra;
                    lod.center = new Vector3Int((x + center.x), (y + center.y), (z + center.z));
                    lod.lod = 2;
                    lod.region = gameObject;
                    lod2rend[(x + 1) * 9 + (y + 1) * 3 + (z + 1)] = lodObj.GetComponent<Renderer>();
                }
            }
        }
        //create LOD 3 renderer
        Renderer[] lod3rend = new Renderer[1];
        GameObject blodObj = (GameObject)Instantiate(blodPrefab, myTrans);
        BLOD blod = blodObj.GetComponent<BLOD>();
        blod.GetComponent<Transform>().transform.Translate(-1 * myTrans.position);
        blod.GetComponent<Transform>().transform.Translate(trans.position);
        blod.terra = terra;
        blod.center = new Vector3Int(center.x/3, center.y/3, center.z/3);
        blod.region = gameObject;
        lod3rend[0] = blodObj.GetComponent<Renderer>();
        lods[0] = new LOD(0.7f, lod0rend);
        lods[1] = new LOD(0.4f, lod1rend);
        lods[2] = new LOD(0.2f, lod2rend);
        lods[3] = new LOD(0.0f, lod3rend);
        group.SetLODs(lods);
        group.RecalculateBounds();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
