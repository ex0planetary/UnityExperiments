using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateChunks : MonoBehaviour
{
    public GameObject chunkPrefab;
    public GameObject lodPrefab;
    public GameObject blodPrefab;
    public GameObject megaChunkPrefab;
    public GameObject atmosphere;
    public GameObject atmosphere2;
    public int chunkRadius = 4;
    Transform trans;
    TerrainGen tg;
    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<Transform>().transform;
        tg = GetComponent<TerrainGen>();
        StartCoroutine("BuildChunks");
    }

    IEnumerator BuildChunks()
    {
        /*for (int x = -21; x <= 21; x++)
        {
            for (int y = -21; y <= 21; y++)
            {
                for (int z = -21; z <= 21; z++)
                {
                    GameObject chunk = (GameObject)Instantiate(chunkPrefab, trans);
                    PlanetMeshMaker mm = chunk.GetComponent<PlanetMeshMaker>();
                    mm.center = new Vector3Int(9 * x, 9 * y, 9 * z);
                    mm.planet = gameObject;
                    StoreBlocks sb = chunk.GetComponent<StoreBlocks>();
                    for (int cx = -4; cx <= 4; cx++)
                    {
                        for (int cy = -4; cy <= 4; cy++)
                        {
                            for (int cz = -4; cz <= 4; cz++)
                            {
                                sb.ChangeAt(cx, cy, cz, tg.getColorAt(x * 9 + cx, y * 9 + cy, z * 9 + cz));
                            }
                        }
                    }
                    /*GameObject lodObj = (GameObject)Instantiate(lodPrefab, trans);
                    CLOD lod = lodObj.GetComponent<CLOD>();
                    lod.terra = tg;
                    lod.center = new Vector3Int(x, y, z);
                    lod.lod = 2;*/
        /*}
        //yield return null;
        yield return null;
    }
}*/
        int chunkDiameter = 2 * chunkRadius + 1;
        int chunkLimit = chunkDiameter * chunkDiameter * chunkDiameter;
        for (int x=-1*chunkRadius; x<=chunkRadius; x++)
        {
            for (int y= -1 * chunkRadius; y<= chunkRadius; y++)
            {
                for (int z= -1 * chunkRadius; z<= chunkRadius; z++)
                {
                    GameObject mchunk = (GameObject)Instantiate(megaChunkPrefab, trans);
                    LODLvlCreation llc = mchunk.GetComponent<LODLvlCreation>();
                    llc.chunkPrefab = chunkPrefab;
                    llc.lodPrefab = lodPrefab;
                    llc.blodPrefab = blodPrefab;
                    llc.center = new Vector3Int(x * 3, y * 3, z * 3);
                    llc.planet = gameObject;
                    int chunksDone = (x + chunkRadius) * chunkDiameter * chunkDiameter + (y + chunkRadius) * chunkDiameter + (z + chunkRadius) + 1;
                    Debug.Log("MegaChunk formed at (" + x + "," + y + "," + z + "), " + chunksDone + "/" + chunkLimit + " done (" + ((float)chunksDone / chunkLimit) * 100 + "%)");
                }
                yield return null;
            }
        }
        float atmosphereEdge = (chunkRadius / 2) * 27 * 4;
        atmosphere.transform.localScale = new Vector3(atmosphereEdge, atmosphereEdge, atmosphereEdge);
        atmosphere2.transform.localScale = new Vector3(atmosphereEdge, atmosphereEdge, atmosphereEdge);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
