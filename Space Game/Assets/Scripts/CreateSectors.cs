using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateSectors : MonoBehaviour
{
    GameObject[,,] sectors = new GameObject[3, 3, 3];
    public GameObject sectorPrefab;
    public Vector3Int currentCenterPos = new Vector3Int(5, 0, 0);
    public CreateStars.GalaxySector galaxySector;
    public GameObject nebulae;
    public Transform cam;
    public float renderDist;

    // Start is called before the first frame update
    void Start()
    {
        nebulae.GetComponent<SpaceCloudPositionData>().sector = galaxySector;
        nebulae.GetComponent<SpaceCloudPositionData>().center = currentCenterPos;
        for (int x=-1; x<=1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    sectors[x + 1, y + 1, z + 1] = (GameObject)Instantiate(sectorPrefab, GetComponent<Transform>());
                    sectors[x + 1, y + 1, z + 1].GetComponent<CreateStars>().galaxySector = galaxySector;
                    sectors[x + 1, y + 1, z + 1].GetComponent<Transform>().Translate(x * 40, y * 40, z * 40);
                    sectors[x + 1, y + 1, z + 1].GetComponent<CreateStars>().sectorPosRaw = new Vector3(x, y, z) + currentCenterPos;
                    float noiseVal = ValueNoise.Value3D(new Vector3(x, y, z) + currentCenterPos, 1);
                    CreateStars.SectorType stype;
                    if (noiseVal < 0.05)
                    {
                        stype = CreateStars.SectorType.GLOB_CLUSTER;
                    } else if (noiseVal < 0.2)
                    {
                        stype = CreateStars.SectorType.DENSE;
                    } else if (noiseVal < 0.8)
                    {
                        stype = CreateStars.SectorType.NORMAL;
                    } else
                    {
                        stype = CreateStars.SectorType.SPARSE;
                    }
                    sectors[x + 1, y + 1, z + 1].GetComponent<CreateStars>().sectorType = stype;
                    sectors[x + 1, y + 1, z + 1].GetComponent<CreateStars>().cam = cam;
                    sectors[x + 1, y + 1, z + 1].GetComponent<CreateStars>().renderDist = renderDist;
                    sectors[x + 1, y + 1, z + 1].GetComponent<CreateStars>().ResetStars();
                }
            }
        }
    }
   

    public void SectorShift(int px, int py, int pz)
    {
        currentCenterPos += new Vector3Int(px, py, pz);
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    sectors[x + 1, y + 1, z + 1].GetComponent<Transform>().Translate(px * -40, py * -40, pz * -40);
                    GameObject curSec = sectors[x + 1, y + 1, z + 1];
                    Transform curSecTrans = curSec.GetComponent<Transform>();
                    Vector3 curSecPos = curSecTrans.position;
                    bool reload = false;
                    if (curSecPos.x < -40)
                    {
                        curSecTrans.Translate(120, 0, 0, Space.World);
                        curSec.GetComponent<CreateStars>().sectorPosRaw.x += 3;
                        reload = true;
                    }
                    if (curSecPos.x > 40)
                    {
                        curSecTrans.Translate(-120, 0, 0, Space.World);
                        curSec.GetComponent<CreateStars>().sectorPosRaw.x -= 3;
                        reload = true;
                    }
                    if (curSecPos.y < -40)
                    {
                        curSecTrans.Translate(0, 120, 0, Space.World);
                        curSec.GetComponent<CreateStars>().sectorPosRaw.y += 3;
                        reload = true;
                    }
                    if (curSecPos.y > 40)
                    {
                        curSecTrans.Translate(0, -120, 0, Space.World);
                        curSec.GetComponent<CreateStars>().sectorPosRaw.y -= 3;
                        reload = true;
                    }
                    if (curSecPos.z < -40)
                    {
                        curSecTrans.Translate(0, 0, 120, Space.World);
                        curSec.GetComponent<CreateStars>().sectorPosRaw.z += 3;
                        reload = true;
                    }
                    if (curSecPos.z > 40)
                    {
                        curSecTrans.Translate(0, 0, -120, Space.World);
                        curSec.GetComponent<CreateStars>().sectorPosRaw.z -= 3;
                        reload = true;
                    }
                    if (reload)
                    {
                        float noiseVal = ValueNoise.Value3D(curSec.GetComponent<CreateStars>().sectorPosRaw, 1);
                        CreateStars.SectorType stype;
                        if (noiseVal < 0.05)
                        {
                            stype = CreateStars.SectorType.GLOB_CLUSTER;
                        }
                        else if (noiseVal < 0.2)
                        {
                            stype = CreateStars.SectorType.DENSE;
                        }
                        else if (noiseVal < 0.8)
                        {
                            stype = CreateStars.SectorType.NORMAL;
                        }
                        else
                        {
                            stype = CreateStars.SectorType.SPARSE;
                        }
                        sectors[x + 1, y + 1, z + 1].GetComponent<CreateStars>().sectorType = stype;
                        sectors[x + 1, y + 1, z + 1].GetComponent<CreateStars>().cam = cam;
                        sectors[x + 1, y + 1, z + 1].GetComponent<CreateStars>().renderDist = renderDist;
                        sectors[x + 1, y + 1, z + 1].GetComponent<CreateStars>().ResetStars();
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
