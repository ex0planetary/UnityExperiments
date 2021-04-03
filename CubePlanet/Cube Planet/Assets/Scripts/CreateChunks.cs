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
    public GameObject testPlanet;
    public int chunkRadius = 4;
    public bool atmosphereTest = false;
    Transform trans;
    TerrainGen tg;
    private int chunkDiameter;
    private int chunkLimit;
    private int[,,] chunkLOD;
    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<Transform>().transform;
        tg = GetComponent<TerrainGen>();
        //StartCoroutine("BuildChunks");
        chunkDiameter = 2 * chunkRadius + 1;
        chunkLimit = chunkDiameter * chunkDiameter * chunkDiameter;
        chunkLOD = new int[chunkDiameter, chunkDiameter, chunkDiameter];
        for (int x=0; x<chunkDiameter; x++)
        {
            for (int y = 0; y < chunkDiameter; y++)
            {
                for (int z = 0; z < chunkDiameter; z++)
                {
                    chunkLOD[x, y, z] = 3;
                }
            }
        }
        float atmosphereEdge = (chunkRadius / 2) * 27 * 4;
        atmosphere.transform.localScale = new Vector3(atmosphereEdge, atmosphereEdge, atmosphereEdge);
        atmosphere2.transform.localScale = new Vector3(atmosphereEdge, atmosphereEdge, atmosphereEdge);
    }

    /*   IEnumerator BuildChunks()
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
    //    if (!atmosphereTest)
    //    {
    /*for (int x = -1 * chunkRadius; x <= chunkRadius; x++)
    {
        for (int y = -1 * chunkRadius; y <= chunkRadius; y++)
        {
            for (int z = -1 * chunkRadius; z <= chunkRadius; z++)
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
    }*/

    /* } else
     {
         Transform planTrans = testPlanet.GetComponent<Transform>();
         planTrans.localScale *= 135;
     }
 }*/
    List<GameObject> currentLOD = new List<GameObject>();
    int GetLOD(Vector3Int centralBlock)
    {
        Vector3Int centralIndx = new Vector3Int(centralBlock.x / 9 + chunkRadius, centralBlock.y / 9 + chunkRadius, centralBlock.z / 9 + chunkRadius);
        return chunkLOD[centralIndx.x, centralIndx.y, centralIndx.z];
    }

    // Update is called once per frame
    public Transform planetTransform;
    public Transform cameraTransform;
    void Update()
    {
        int chunkDiameter = 2 * chunkRadius + 1;
        for (int x = 0; x < chunkDiameter; x++)
        {
            for (int y = 0; y < chunkDiameter; y++)
            {
                for (int z = 0; z < chunkDiameter; z++)
                {
                    Vector3Int centralBlockPos = new Vector3Int(x - chunkRadius, y - chunkRadius, z - chunkRadius);
                    Vector3Int centralPos = new Vector3Int(9 * (x - chunkRadius), 9 * (y - chunkRadius), 9 * (z - chunkRadius));
                    float distance = (planetTransform.TransformPoint(centralPos) - cameraTransform.position).magnitude;
                    if (distance < 22)
                    {
                        if (chunkLOD[x, y, z] != 0)
                        {
                            chunkLOD[x, y, z] = 0;
                            GameObject chunk = (GameObject)Instantiate(chunkPrefab, trans);
                            PlanetMeshMaker mm = chunk.GetComponent<PlanetMeshMaker>();
                            mm.center = centralPos;
                            mm.planet = gameObject;
                            StoreBlocks sb = chunk.GetComponent<StoreBlocks>();
                            for (int cx = -4; cx <= 4; cx++)
                            {
                                for (int cy = -4; cy <= 4; cy++)
                                {
                                    for (int cz = -4; cz <= 4; cz++)
                                    {
                                        sb.ChangeAt(cx, cy, cz, tg.getColorAt((x - chunkRadius) * 9 + cx, (y - chunkRadius) * 9 + cy, (z - chunkRadius) * 9 + cz));
                                    }
                                }
                            }
                        }
                    } else if (distance < 139)
                    {
                        if (chunkLOD[x, y, z] != 1 && chunkLOD[x, y, z] != 2)
                        {
                            chunkLOD[x, y, z] = 1;
                        }
                    } else if (distance < 229)
                    {
                        if (chunkLOD[x, y, z] != 1 && chunkLOD[x, y, z] != 2)
                        {
                            chunkLOD[x, y, z] = 2;
                        }
                    } else if (distance >= 229)
                    {
                        chunkLOD[x, y, z] = 3;
                    }
                }
            }
        }
    }
}
