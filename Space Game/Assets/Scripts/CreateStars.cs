using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateStars : MonoBehaviour
{
    public GameObject starPrefab;
    public enum SectorType { SPARSE, NORMAL, DENSE, GLOB_CLUSTER };
    public enum GalaxySector { EPSILON, DELTA, GAMMA, BETA, ALPHA };
    public SectorType sectorType;
    public GalaxySector galaxySector;
    public Vector3 sectorPosRaw;
    public Transform cam;
    public float renderDist;

    Color GetStarColorRaw(Vector3 pos)
    {
        //float noiseVal = Perlin.Noise((pos.x + sectorPos.x + 12345) / 2, (pos.y + sectorPos.y + 12345) / 2, (pos.z + sectorPos.z + 12345) / 2) / 2;
        float noiseVal = ValueNoise.Value3D(pos, 1) * 2 - 1;
        float diffVal = ValueNoise.Value3D(pos + new Vector3(12345, 67890, 58921), 1) * 2 - 1;
        if (diffVal < -0.6)
        {
            // very easy
            if (noiseVal < 0)
            {
                // yellow (warm)
                return new Color(1, 0.93f, 0.58f, 1);
            } else
            {
                // blue (gentle)
                return new Color(0.52f, 0.59f, 1, 1);
            }
        } else if (diffVal < 0.06)
        {
            // easy
            if (noiseVal < 0)
            {
                // pink (exotic)
                return new Color(1, 0.67f, 0.95f);
            }
            else
            {
                // white (temperate)
                return new Color(1, 1, 1);
            }
        } else if (diffVal < 0.6)
        {
            // medium
            if (noiseVal < -0.33)
            {
                // orange (hot)
                return new Color(1, 0.73f, 0.55f);
            } else if (noiseVal < 0.33)
            {
                // lime (strange)
                return new Color(0.59f, 1, 0.64f);
            } else
            {
                // green (radioactive)
                return new Color(0.46f, 0.63f, 0.48f);
            }
        } else if (diffVal < 0.73)
        {
            // hard
            if (noiseVal < 0)
            {
                // red (fiery)
                return new Color(1, 0.64f, 0.64f);
            }
            else
            {
                // cyan (frigid)
                return new Color(0.58f, 1, 1);
            }
        } else
        {
            // very hard
            if (noiseVal < 0)
            {
                // purple (mysterious)
                return new Color(0.67f, 0.56f, 1);
            }
            else
            {
                // neutron
                return new Color(0.84f, 0.49f, 1);
            }
        }
    }

    Color GetStarColor(Vector3 pos)
    {
        return (Color.white + GetStarColorRaw(pos)) / 2;
    }

    GameObject[,,] starObjects = new GameObject[40, 40, 40];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ResetStars()
    {
        StartCoroutine("ResetStarsUtil");
    }

    IEnumerator ResetStarsUtil()
    {
        // Sectors will be 40m x 40m x 40m, aka 20m radius
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        yield return null;
        Vector3 instPos;
        Vector3 sectorPos = sectorPosRaw * 41;
        for (int y = 0; y < 40; y++)
        {
            for (int x = 0; x < 40; x++)
            {
                for (int z = 0; z < 40; z++)
                {
                    instPos = new Vector3(x - 20, y - 20, z - 20);
                    Vector3 shake = new Vector3(
                        Perlin.Noise((x + sectorPos.x) / 2, (y + sectorPos.y) / 2, (z + sectorPos.z) / 2) / 2,
                        Perlin.Noise((x + sectorPos.x + 12345) / 2, (y + sectorPos.y + 12345) / 2, (z + sectorPos.z + 12345) / 2) / 2,
                        Perlin.Noise((x + sectorPos.x + 54321) / 2, (y + sectorPos.y + 54321) / 2, (z + sectorPos.z + 54321) / 2) / 2
                        );
                    float ScaleVal = (Perlin.Noise((x + sectorPos.x) / 2, (y + sectorPos.y) / 2, (z + sectorPos.z) / 2) + 3) / 2;
                    if (sectorType == SectorType.SPARSE)
                    {
                        if (Perlin.Noise((x + sectorPos.x) / 2, (y + sectorPos.y) / 2, (z + sectorPos.z) / 2) < -0.75)
                        {
                            // make star
                            starObjects[x, y, z] = (GameObject)Instantiate(starPrefab, GetComponent<Transform>());
                        }
                        else
                        {
                            starObjects[x, y, z] = gameObject;
                        }
                    }
                    if (sectorType == SectorType.NORMAL)
                    {
                        if (Perlin.Noise((x + sectorPos.x) / 2, (y + sectorPos.y) / 2, (z + sectorPos.z) / 2) < -0.5)
                        {
                            // make star
                            starObjects[x, y, z] = (GameObject)Instantiate(starPrefab, GetComponent<Transform>());
                        }
                        else
                        {
                            starObjects[x, y, z] = gameObject;
                        }
                    }
                    else if (sectorType == SectorType.DENSE)
                    {
                        if (Perlin.Noise((x + sectorPos.x) / 2, (y + sectorPos.y) / 2, (z + sectorPos.z) / 2) < -0.25)
                        {
                            // make star
                            starObjects[x, y, z] = (GameObject)Instantiate(starPrefab, GetComponent<Transform>());
                        }
                        else
                        {
                            starObjects[x, y, z] = gameObject;
                        }
                    }
                    else if (sectorType == SectorType.GLOB_CLUSTER)
                    {
                        Vector3 RelativePos = new Vector3(x, y, z);
                        Vector3 CentralPos = new Vector3(20, 20, 20);
                        RelativePos -= CentralPos;
                        if (Perlin.Noise((x + sectorPos.x) / 2, (y + sectorPos.y) / 2, (z + sectorPos.z) / 2) < (1 - (RelativePos.magnitude / 15)))
                        {
                            // make star
                            starObjects[x, y, z] = (GameObject)Instantiate(starPrefab, GetComponent<Transform>());
                        }
                        else
                        {
                            starObjects[x, y, z] = gameObject;
                        }
                    }
                    if (starObjects[x, y, z] != gameObject)
                    {
                        starObjects[x, y, z].GetComponent<Renderer>().material.SetColor("_Color1", GetStarColor(instPos + sectorPos));
                        //starObjects[x, y, z].GetComponent<Renderer>().material.SetFloat("_Rotation1", Random.value * 2 * Mathf.PI);
                        starObjects[x, y, z].GetComponent<Transform>().Translate(instPos);
                        starObjects[x, y, z].GetComponent<Transform>().Translate(shake);
                        starObjects[x, y, z].GetComponent<Transform>().localScale.Set(ScaleVal, ScaleVal, ScaleVal);
                        starObjects[x, y, z].GetComponent<CameraHide>().cam = cam;
                        starObjects[x, y, z].GetComponent<CameraHide>().renderDist = renderDist;
                        //starObjects[x, y, z].GetComponent<StarRotation>().cam = cam;
                        float ScaleValue = Random.value + 0.5f;
                        starObjects[x, y, z].GetComponent<Transform>().localScale *= ScaleValue;
                    }
                }
            }
            if (y%4 != 0)
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
