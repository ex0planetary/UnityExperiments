using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TBiome { BI_Undef, BI_Plains, BI_Hills, BI_Mountains1, BI_Mountains2, BI_Desert, BI_Plateaus };

public class Biome
{
    public TBiome terrainType;
    public Color32 colorGrass;
    public Color32 colorMtns;
    public Color32 colorUnderground;
    System.Random rand;
    public Biome(Color32 grassColor, int seed)
    {
        rand = new System.Random(seed);
        colorGrass = grassColor;
        colorGrass.r += (byte)(rand.Next(60) - 30);
        colorGrass.g += (byte)(rand.Next(60) - 30);
        colorGrass.b += (byte)(rand.Next(60) - 30);
        colorMtns = new Color32((byte)(rand.Next(127) + 127), (byte)(rand.Next(127) + 127), (byte)(rand.Next(127) + 127), 255);
        colorUnderground = new Color32((byte)(rand.Next(127) + 127), (byte)(rand.Next(127) + 127), (byte)(rand.Next(127) + 127), 255);
    }
    public void SelectType(int randomVal)
    {
        switch (randomVal)
        {
            case 0:
                terrainType = TBiome.BI_Plains;
                break;
            case 1:
                terrainType = TBiome.BI_Hills;
                break;
            case 2:
                terrainType = TBiome.BI_Mountains1;
                break;
            case 3:
                terrainType = TBiome.BI_Mountains2;
                break;
            case 4:
                terrainType = TBiome.BI_Desert;
                break;
            case 5:
                terrainType = TBiome.BI_Plateaus;
                break;
        }
    }
    public Color32 GetColor()
    {
        switch (terrainType)
        {
            case TBiome.BI_Undef:
                return new Color32(127, 127, 127, 255);
            case TBiome.BI_Plains:
                return new Color32(117, 211, 40, 255);
            case TBiome.BI_Hills:
                return new Color32(0, 255, 0, 255);
            case TBiome.BI_Mountains1:
                return new Color32(160, 160, 160, 255);
            case TBiome.BI_Mountains2:
                return new Color32(127, 51, 0, 255);
            case TBiome.BI_Desert:
                return new Color32(255, 204, 142, 255);
            case TBiome.BI_Plateaus:
                return new Color32(234, 69, 32, 255);
            default:
                return new Color32(0, 0, 0, 0);
        }
    }
    public void PrintTerrainType()
    {
        switch (terrainType)
        {
            case TBiome.BI_Undef:
                Debug.Log("Biome TType Undefined");
                break;
            case TBiome.BI_Plains:
                Debug.Log("Biome TType Plains");
                break;
            case TBiome.BI_Hills:
                Debug.Log("Biome TType Hills");
                break;
            case TBiome.BI_Mountains1:
                Debug.Log("Biome TType Mountains 1");
                break;
            case TBiome.BI_Mountains2:
                Debug.Log("Biome TType Mountains 2");
                break;
            case TBiome.BI_Desert:
                Debug.Log("Biome TType Desert");
                break;
            case TBiome.BI_Plateaus:
                Debug.Log("Biome TType Plateaus");
                break;
            default:
                Debug.Log("Biome TType Error");
                break;
        }
    }
}

public class TerrainGen : MonoBehaviour
{
    StoreBlocks blocks;
    Noise noise;
    SimpleNoiseFilter snf;
    RidgedNoiseFilter rnf;
    public float baseFrequency, deltaFrequency, volatility, sharpness;
    public float persistence;
    public float minHeight, maxHeight, heightOffset;
    public int octaves;
    public Vector3 Center;
    public int biomeDetail = 128;
    public float mountainHeight;
    public int circum = 600;
    public int smoothingFactor = 4;
    int[,] biomeMap;
    List<Biome> biomeList;
    /**
     * TERRAN BIOMES
     * Plains
     * Hills
     * Mountains
     * Desert
     * Plateaus
     */

    // Start is called before the first frame update

    bool BiomesComplete()
    {
        bool isComplete = true;
        for (int x = 0; x < biomeDetail; x++)
        {
            for (int y = 0; y < biomeDetail; y++)
            {
                if (biomeMap[x,y] == -1)
                {
                    isComplete = false;
                }
            }
        }
        return isComplete;
    }

    void SetBiome(int x, int y, int biome)
    {
        int tx = x;
        int ty = y;
        if (tx < 0) tx = biomeDetail - 1;
        if (tx >= biomeDetail) tx = 0;
        if (ty < 0) ty = biomeDetail - 1;
        if (ty >= biomeDetail) ty = 0;
        biomeMap[tx, ty] = biome;
    }

    int GetBiome(int x, int y)
    {
        int tx = x;
        int ty = y;
        if (tx < 0) tx = biomeDetail - 1;
        if (tx >= biomeDetail) tx = 0;
        if (ty < 0) ty = biomeDetail - 1;
        if (ty >= biomeDetail) ty = 0;
        return biomeMap[tx, ty];
    }

    void GenerateBiomes()
    {
        Debug.Log("Started generating biomes");
        int biomeNumber = 5;
        int mseed = 345;
        System.Random rando = new System.Random(mseed);
        for (int i=0; i<biomeNumber; i++)
        {
            biomeList.Add(new Biome(new Color32(0, 200, 0, 255), mseed + i));
            biomeList[i].SelectType(rando.Next(6));
            biomeList[i].PrintTerrainType();
            int x = rando.Next(biomeDetail);
            int y = rando.Next(biomeDetail);
            biomeMap[x, y] = i;
        }
        while (!BiomesComplete())
        {
            for (int x=0; x<biomeDetail; x++)
            {
                for (int y=0; y<biomeDetail; y++)
                {
                    if (biomeMap[x,y] != -1)
                    {
                        int iniDirection = rando.Next(4); // 0=up, 1=right, 2=down, 3=left
                        int direction = iniDirection;
                        do
                        {
                            int xd = x;
                            int yd = y;
                            switch (direction)
                            {
                                case 0:
                                    xd = x;
                                    yd = y - 1;
                                    break;
                                case 1:
                                    xd = x + 1;
                                    yd = y;
                                    break;
                                case 2:
                                    xd = x;
                                    yd = y + 1;
                                    break;
                                case 3:
                                    xd = x - 1;
                                    yd = y;
                                    break;
                            }
                            if (GetBiome(xd, yd) == -1)
                            {
                                SetBiome(xd, yd, biomeMap[x, y]);
                                break;
                            }
                            direction++;
                            if (direction > 3) direction = 0;
                        } while (direction != iniDirection);
                    }
                }
            }
        }
    }

    Biome GetBiomeAt(float psi, float theta)
    {
        float u = theta / (2 * Mathf.PI);
        while (u > 1) u--;
        while (u < 0) u++;
        float v = (Mathf.Cos(psi) + 1) / 2;
        //float u = theta / (2 * Mathf.PI);
        //float v = psi / (Mathf.PI);
        int su = Mathf.RoundToInt(u * biomeDetail);
        int sv = Mathf.RoundToInt(v * biomeDetail);
        if (GetBiome(su, sv) == -1) return new Biome(new Color32(0, 0, 0, 0), 0);
        else return biomeList[GetBiome(su, sv)];
    }

    void Start()
    {
        noise = new Noise(12345);
        snf = new SimpleNoiseFilter(12345,baseFrequency,deltaFrequency,persistence,volatility,octaves,Center);
        rnf = new RidgedNoiseFilter(12345,sharpness,baseFrequency,deltaFrequency,persistence,volatility,octaves,Center);
        biomeList = new List<Biome>();
        biomeMap = new int[biomeDetail, biomeDetail];
        for (int x=0; x<biomeDetail; x++)
        {
            for (int y=0; y<biomeDetail; y++)
            {
                biomeMap[x, y] = -1;
            }
        }
        GenerateBiomes();
        for (int y=0; y<biomeDetail; y++)
        {
            string tstr = "";
            for (int x=0; x<biomeDetail; x++)
            {
                tstr += biomeMap[x, y];
                tstr += ",";
            }
            Debug.Log(tstr);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    float getHeightmap(float psi, float theta)
    {
        bool simple = true;
        switch (GetBiomeAt(psi, theta).terrainType)
        {
            case TBiome.BI_Plains:
                snf.ChangeSettings(3, 0.5f, 0.5f, 3, 4, Center);
                //snf.ChangeSettings(12, 0.5f, 0.5f, 3, 4, Center);
                minHeight = 77;
                maxHeight = 120;
                heightOffset = 75;
                break;
            case TBiome.BI_Hills:
                snf.ChangeSettings(4, 0.5f, 0.5f, 5, 4, Center);
                //snf.ChangeSettings(12, 0.5f, 0.5f, 5, 4, Center);
                minHeight = 77;
                maxHeight = 120;
                heightOffset = 75;
                break;
            case TBiome.BI_Mountains1:
                snf.ChangeSettings(1.5f, 0.5f, 0.5f, 30, 4, Center);
                //snf.ChangeSettings(6, 0.5f, 0.5f, 30, 4, Center);
                minHeight = 85;
                maxHeight = 120;
                heightOffset = 85;
                break;
            case TBiome.BI_Mountains2:
                simple = false;
                rnf.ChangeSettings(2, 1.5f, 0.5f, 0.5f, 30, 4, Center);
                //rnf.ChangeSettings(8, 1.5f, 0.5f, 0.5f, 30, 4, Center);
                minHeight = 85;
                maxHeight = 120;
                heightOffset = 85;
                break;
            case TBiome.BI_Desert:
                simple = false;
                rnf.ChangeSettings(2, 4, 0.5f, 0.5f, 5, 4, Center);
                //rnf.ChangeSettings(8, 4, 0.5f, 0.5f, 5, 4, Center);
                minHeight = 77;
                maxHeight = 120;
                heightOffset = 75;
                break;
            case TBiome.BI_Plateaus:
                simple = false;
                rnf.ChangeSettings(2, 1.5f, 2, 0.5f, 20, 4, Center);
                //rnf.ChangeSettings(8, 1.5f, 2, 0.5f, 20, 4, Center);
                minHeight = 85;
                maxHeight = 100;
                heightOffset = 85;
                break;
        }
        float heightmap = 0;
        if (simple)
        {
            heightmap = snf.Noise(new Vector3(Mathf.Sin(psi) * Mathf.Cos(theta), Mathf.Cos(psi), Mathf.Sin(psi) * Mathf.Sin(theta)));
        }
        else
        {
            heightmap = rnf.Noise(new Vector3(Mathf.Sin(psi) * Mathf.Cos(theta), Mathf.Cos(psi), Mathf.Sin(psi) * Mathf.Sin(theta)));
        }
        return heightmap + heightOffset;
    }

    public float getHeightAt(float psi, float theta)
    {
        // Mountains 1: Simple BF 1.5 DF 0.5 V 30 P 0.5 MiH 85 MaH 120 HO 85 O 4
        // Mountains 2: Ridged BF 1.5 DF 0.5 V 30 S 2 P 0.5 MiH 85 MaH 120 HO 85 O 4
        // Plains: Simple BF 3 DF 0.5 V 3 P 0.5 MiH 77 MaH 120 HO 75 O 4
        // Foothills: Simple BF 4 DF 0.5 V 5 P 0.5 MiH 77 MaH 120 HO 75 O 4
        // Desert: Ridged BF 4 DF 0.5 V 5 S 2 P 0.5 MiH 77 MaH 120 HO 75 O 4
        // Plateaus: Ridged BF 1.5 DF 2 V 20 S 2 P 0.5 MiH 85 MaH 100 HO 85 O 4
        
        float numT = (theta * circum) / (2 * Mathf.PI * smoothingFactor);
        float numP = (psi * circum) / (Mathf.PI * smoothingFactor);
        float minPsi = (Mathf.Floor(numP) * Mathf.PI * smoothingFactor) / circum;
        float maxPsi = (Mathf.Ceil(numP) * Mathf.PI * smoothingFactor) / circum;
        float minTheta = (Mathf.Floor(numT) * 2 * Mathf.PI * smoothingFactor) / circum;
        float maxTheta = (Mathf.Ceil(numT) * 2 * Mathf.PI * smoothingFactor) / circum;
        float height;
        //if (GetBiomeAt(minPsi, minTheta).terrainType == GetBiomeAt(minPsi, maxTheta).terrainType && GetBiomeAt(minPsi, maxTheta).terrainType == GetBiomeAt(maxPsi, maxTheta).terrainType && GetBiomeAt(maxPsi, maxTheta).terrainType == GetBiomeAt(maxPsi, minTheta).terrainType)
        //{
            //biomes agree
            //height = getHeightmap(psi, theta);
        //}
        //else
        //{
            float heightmap00 = getHeightmap(minPsi,minTheta);
            float min00 = minHeight;
            float max00 = maxHeight;
            float heightmap01 = getHeightmap(minPsi,maxTheta);
            float min01 = minHeight;
            float max01 = maxHeight;
            float heightmap10 = getHeightmap(maxPsi,minTheta);
            float min10 = minHeight;
            float max10 = maxHeight;
            float heightmap11 = getHeightmap(maxPsi,maxTheta);
            float min11 = minHeight;
            float max11 = maxHeight;
            //return heightmap01*25+75;
            //float height = heightmap01 + heightOffset;
            height = Mathf.Lerp(Mathf.Lerp(heightmap00, heightmap01, numT - Mathf.Floor(numT)), Mathf.Lerp(heightmap10, heightmap11, numT - Mathf.Floor(numT)), numP - Mathf.Floor(numP));
            //minHeight = Mathf.Lerp(Mathf.Lerp(min00, min01, numT - Mathf.Floor(numT)), Mathf.Lerp(min10, min11, numT - Mathf.Floor(numT)), numP - Mathf.Floor(numP));
            minHeight = Mathf.Min(min00, min01, min10, min11);
            maxHeight = Mathf.Lerp(Mathf.Lerp(max00, max01, numT - Mathf.Floor(numT)), Mathf.Lerp(max10, max11, numT - Mathf.Floor(numT)), numP - Mathf.Floor(numP));
        //}
            //if (height < minHeight) height = minHeight;
        //if (height > maxHeight) height = maxHeight;
        return height;
        //return 75;
    }

    public Vector3 cubeToPolar(int x, int y, int z)
    {
        int dist = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y), Mathf.Abs(z));
        float mx = (float)x / dist;
        float my = (float)y / dist;
        float mz = (float)z / dist;
        float tx = mx * Mathf.Sqrt(1 - (Mathf.Pow(my, 2) / 2) - (Mathf.Pow(mz, 2) / 2) + (Mathf.Pow(my, 2) * Mathf.Pow(mz, 2) / 3)) * dist;
        float ty = my * Mathf.Sqrt(1 - (Mathf.Pow(mz, 2) / 2) - (Mathf.Pow(mx, 2) / 2) + (Mathf.Pow(mz, 2) * Mathf.Pow(mx, 2) / 3)) * dist;
        float tz = mz * Mathf.Sqrt(1 - (Mathf.Pow(mx, 2) / 2) - (Mathf.Pow(my, 2) / 2) + (Mathf.Pow(mx, 2) * Mathf.Pow(my, 2) / 3)) * dist;
        float rho = Mathf.Sqrt(tx * tx + ty * ty + tz * tz);
        float psi = Mathf.Acos(ty / rho);
        //float theta = Mathf.Asin(tz / (rho * Mathf.Sin(psi)));
        float theta = Mathf.Atan2(tz, tx);
        return new Vector3(rho, psi, theta);
    }

    public Color32 getColorAt(int x, int y, int z)
    {
        int dist = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y), Mathf.Abs(z));
        Vector3 polar = cubeToPolar(x, y, z);
        //Biome thisBiome = GetBiomeAt(polar.y, polar.z);
        if (dist < minHeight)
        {
            if (dist == minHeight - 1)
            {
                return GetBiomeAt(polar.y, polar.z).colorGrass;
            } else
            {
                return GetBiomeAt(polar.y, polar.z).colorUnderground;
            }
            //return GetBiomeAt(polar.y, polar.z).GetColor();
            //return new Color32(255, 189, 109, 255);
        }
        if (dist > maxHeight)
        {
            return new Color32(0, 0, 0, 0);
        }
        float height = getHeightAt(polar.y, polar.z);
        if (polar.x < height)
        {
            //return GetBiomeAt(polar.y, polar.z).GetColor();
            if (polar.x > mountainHeight)
            {
                return GetBiomeAt(polar.y, polar.z).colorMtns;
            } else if (polar.x > height - 2)
            {
                return GetBiomeAt(polar.y, polar.z).colorGrass;
            } else
            {
                return GetBiomeAt(polar.y, polar.z).colorUnderground;
            }
            //return new Color32(255, 189, 109, 255);
        } else
        {
            return new Color32(0, 0, 0, 0);
        }
    }
}
