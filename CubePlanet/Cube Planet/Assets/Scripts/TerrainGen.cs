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
        int r, g, b;
        r = colorGrass.r;
        g = colorGrass.g;
        b = colorGrass.b;
        r += (rand.Next(60) - 30);
        g += (rand.Next(60) - 30);
        b += (rand.Next(60) - 30);
        colorGrass.r = (byte)r;
        colorGrass.g = (byte)g;
        colorGrass.b = (byte)b;
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
                terrainType = TBiome.BI_Plains;
                break;
            case 5:
                terrainType = TBiome.BI_Plateaus;
                break;
            default:
                terrainType = TBiome.BI_Undef;
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
        string bstr = "";
        switch (terrainType)
        {
            case TBiome.BI_Undef:
                bstr += "Biome TType Undefined";
                break;
            case TBiome.BI_Plains:
                bstr += "Biome TType Plains";
                break;
            case TBiome.BI_Hills:
                bstr += "Biome TType Hills";
                break;
            case TBiome.BI_Mountains1:
                bstr += "Biome TType Mountains 1";
                break;
            case TBiome.BI_Mountains2:
                bstr += "Biome TType Mountains 2";
                break;
            case TBiome.BI_Desert:
                bstr += "Biome TType Desert";
                break;
            case TBiome.BI_Plateaus:
                bstr += "Biome TType Plateaus";
                break;
            default:
                bstr += "Biome TType Error";
                break;
        }
        bstr += "; GColor ";
        bstr += colorGrass.ToString();
        bstr += "; UColor ";
        bstr += colorUnderground.ToString();
        bstr += "; MColor ";
        bstr += colorMtns.ToString();
        Debug.Log(bstr);

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
    int[,] biomePX, biomePY, biomePZ, biomeNX, biomeNY, biomeNZ;
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
                if (biomePX[x,y] == -1 || biomePY[x, y] == -1 || biomePZ[x, y] == -1 || biomeNX[x, y] == -1 || biomeNY[x, y] == -1 || biomeNZ[x, y] == -1)
                {
                    isComplete = false;
                }
            }
        }
        return isComplete;
    }

    Vector3Int wrapAround(int map, int x, int y)
    {
        /**
         * MAPS
         * 0 = +x
         * 1 = -x
         * 2 = +y
         * 3 = -y
         * 4 = +z
         * 5 = -z
         */
        int tx = x;
        int ty = y;
        int cmap = map;
        int nmap = map;/*
        if (tx < 0) tx = biomeDetail - 1;
        if (tx >= biomeDetail) tx = 0;
        if (ty < 0) ty = biomeDetail - 1;
        if (ty >= biomeDetail) ty = 0;
        biomeMap[tx, ty] = biome;*/
        if (tx < 0)
        {
            switch (cmap)
            {
                case 0:
                    nmap = 2;
                    tx = ty;
                    ty = biomeDetail - 1;
                    break;
                case 1:
                    nmap = 5;
                    tx = ty;
                    ty = 0;
                    break;
                case 2:
                    nmap = 1;
                    tx = ty;
                    ty = biomeDetail - 1;
                    break;
                case 3:
                    nmap = 5;
                    tx = 0;
                    break;
                case 4:
                    nmap = 1;
                    tx = biomeDetail - 1;
                    break;
                case 5:
                    nmap = 3;
                    tx = 0;
                    break;
            }
        }
        cmap = nmap;
        /**
         * MAPS
         * 0 = +x
         * 1 = -x
         * 2 = +y
         * 3 = -y
         * 4 = +z
         * 5 = -z
         */
        if (tx > biomeDetail - 1)
        {
            switch (cmap)
            {
                case 0:
                    nmap = 2;
                    tx = biomeDetail - 1;
                    break;
                case 1:
                    nmap = 4;
                    tx = 0;
                    break;
                case 2:
                    nmap = 0;
                    tx = biomeDetail - 1;
                    break;
                case 3:
                    nmap = 4;
                    tx = ty;
                    ty = 0;
                    break;
                case 4:
                    nmap = 0;
                    tx = ty;
                    ty = biomeDetail - 1;
                    break;
                case 5:
                    nmap = 2;
                    tx = ty;
                    ty = 0;
                    break;
            }
        }
        cmap = nmap;
        /**
         * MAPS
         * 0 = +x
         * 1 = -x
         * 2 = +y
         * 3 = -y
         * 4 = +z
         * 5 = -z
         */
        if (ty < 0)
        {
            switch (cmap)
            {
                case 0:
                    nmap = 5;
                    ty = biomeDetail - 1;
                    break;
                case 1:
                    nmap = 3;
                    ty = 0;
                    break;
                case 2:
                    nmap = 5;
                    ty = tx;
                    tx = biomeDetail - 1;
                    break;
                case 3:
                    nmap = 1;
                    ty = 0;
                    break;
                case 4:
                    nmap = 3;
                    ty = tx;
                    tx = biomeDetail - 1;
                    break;
                case 5:
                    nmap = 1;
                    ty = tx;
                    tx = 0;
                    break;
            }
        }
        cmap = nmap;
        /**
         * MAPS
         * 0 = +x
         * 1 = -x
         * 2 = +y
         * 3 = -y
         * 4 = +z
         * 5 = -z
         */
        if (ty > biomeDetail - 1)
        {
            switch (cmap)
            {
                case 0:
                    nmap = 4;
                    ty = tx;
                    tx = biomeDetail - 1;
                    break;
                case 1:
                    nmap = 2;
                    ty = tx;
                    tx = 0;
                    break;
                case 2:
                    nmap = 4;
                    ty = biomeDetail - 1;
                    break;
                case 3:
                    nmap = 0;
                    ty = tx;
                    tx = 0;
                    break;
                case 4:
                    nmap = 2;
                    ty = biomeDetail - 1;
                    break;
                case 5:
                    nmap = 0;
                    ty = 0;
                    break;
            }
        }
        cmap = nmap;
        return new Vector3Int(cmap, tx, ty);
    }

    void SetBiome(int map, int x, int y, int biome)
    {
        Vector3Int wrapped = wrapAround(map, x, y);
        switch (wrapped.x)
        {
            case 0:
                biomePX[wrapped.y, wrapped.z] = biome;
                break;
            case 1:
                biomeNX[wrapped.y, wrapped.z] = biome;
                break;
            case 2:
                biomePY[wrapped.y, wrapped.z] = biome;
                break;
            case 3:
                biomeNY[wrapped.y, wrapped.z] = biome;
                break;
            case 4:
                biomePZ[wrapped.y, wrapped.z] = biome;
                break;
            case 5:
                biomeNZ[wrapped.y, wrapped.z] = biome;
                break;
        }
    }

    int GetBiome(int map, int x, int y)
    {
        Vector3Int wrapped = wrapAround(map, x, y);
        switch (wrapped.x)
        {
            case 0:
                return biomePX[wrapped.y, wrapped.z];
            case 1:
                return biomeNX[wrapped.y, wrapped.z];
            case 2:
                return biomePY[wrapped.y, wrapped.z];
            case 3:
                return biomeNY[wrapped.y, wrapped.z];
            case 4:
                return biomePZ[wrapped.y, wrapped.z];
            case 5:
                return biomeNZ[wrapped.y, wrapped.z];
            default:
                return 0;
        }
    }

    void GenerateBiomes()
    {
        Debug.Log("Started generating biomes");
        int biomeNumber = 2;
        int mseed = 345;
        System.Random rando = new System.Random(mseed);
        for (int i=0; i<biomeNumber; i++)
        {
            biomeList.Add(new Biome(new Color32(0, 200, 0, 255), mseed + i));
            biomeList[i].SelectType(rando.Next(6));
            biomeList[i].PrintTerrainType();
            int x = rando.Next(biomeDetail);
            int y = rando.Next(biomeDetail);
            int m = rando.Next(6);
            switch (m)
            {
                case 0:
                    biomePX[x, y] = i;
                    break;
                case 1:
                    biomeNX[x, y] = i;
                    break;
                case 2:
                    biomePY[x, y] = i;
                    break;
                case 3:
                    biomeNY[x, y] = i;
                    break;
                case 4:
                    biomePZ[x, y] = i;
                    break;
                case 5:
                    biomeNZ[x, y] = i;
                    break;
            }
        }
        while (!BiomesComplete())
        {
            for (int x=0; x<biomeDetail; x++)
            {
                for (int y=0; y<biomeDetail; y++)
                {
                    if (biomePX[x,y] != -1)
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
                            if (GetBiome(0, xd, yd) == -1)
                            {
                                SetBiome(0, xd, yd, biomePX[x, y]);
                                break;
                            }
                            direction++;
                            if (direction > 3) direction = 0;
                        } while (direction != iniDirection);
                    }
                    if (biomeNX[x, y] != -1)
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
                            if (GetBiome(1, xd, yd) == -1)
                            {
                                SetBiome(1, xd, yd, biomeNX[x, y]);
                                break;
                            }
                            direction++;
                            if (direction > 3) direction = 0;
                        } while (direction != iniDirection);
                    }
                    if (biomePY[x, y] != -1)
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
                            if (GetBiome(2, xd, yd) == -1)
                            {
                                SetBiome(2, xd, yd, biomePY[x, y]);
                                break;
                            }
                            direction++;
                            if (direction > 3) direction = 0;
                        } while (direction != iniDirection);
                    }
                    if (biomeNY[x, y] != -1)
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
                            if (GetBiome(3, xd, yd) == -1)
                            {
                                SetBiome(3, xd, yd, biomeNY[x, y]);
                                break;
                            }
                            direction++;
                            if (direction > 3) direction = 0;
                        } while (direction != iniDirection);
                    }
                    if (biomePZ[x, y] != -1)
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
                            if (GetBiome(4, xd, yd) == -1)
                            {
                                SetBiome(4, xd, yd, biomePZ[x, y]);
                                break;
                            }
                            direction++;
                            if (direction > 3) direction = 0;
                        } while (direction != iniDirection);
                    }
                    if (biomeNZ[x, y] != -1)
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
                            if (GetBiome(5, xd, yd) == -1)
                            {
                                SetBiome(5, xd, yd, biomeNZ[x, y]);
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

    /*Biome GetBiomeAt(float psi, float theta)
    {
        float u = theta / (2 * Mathf.PI);
        float v = (Mathf.Cos(psi) + 1) / 2;
        //float u = theta / (2 * Mathf.PI);
        //float v = psi / (Mathf.PI);
        int su = Mathf.RoundToInt(u * biomeDetail);
        int sv = Mathf.RoundToInt(v * biomeDetail);
        while (su > biomeDetail - 1) su -= biomeDetail;
        while (su < 0) su += biomeDetail;
        while (sv > biomeDetail - 1) sv -= biomeDetail;
        while (sv < 0) sv += biomeDetail;
        if (GetBiome(su, sv) == -1) { Debug.Log("Invalid biome found, wtf"); return new Biome(new Color32(0, 0, 0, 0), 0); }
        else return biomeList[GetBiome(su, sv)];
    }*/

    public Biome GetBiomeAt(float x, float y, float z)
    {
        float pri, sec;
        int map = 0;
        if (Mathf.Abs(z) > Mathf.Abs(x) && Mathf.Abs(z) > Mathf.Abs(y))
        {
            if (z > 0)
            {
                //positive z, primary x, secondary y
                map = 4;
                pri = x;
                sec = y;
            }
            else
            {
                //negative z, primary y, secondary x
                map = 5;
                pri = y;
                sec = x;
            }
        }
        else if (Mathf.Abs(y) > Mathf.Abs(x))
        {
            if (y > 0)
            {
                //positive y, primary z, secondary x
                map = 2;
                pri = z;
                sec = x;
            }
            else
            {
                //negative y, primary x, secondary z
                map = 3;
                pri = x;
                sec = z;
            }
        }
        else
        {
            if (x > 0)
            {
                //positive x, primary y, secondary z
                map = 0;
                pri = y;
                sec = z;
            }
            else
            {
                //negative x, primary z, secondary y
                map = 1;
                pri = z;
                sec = y;
            }
        }
        float height = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y), Mathf.Abs(z));
        float fracPri = (pri + height) / (2 * height);
        float fracSec = (sec + height) / (2 * height);
        int priIndx = Mathf.RoundToInt(fracPri*(biomeDetail-1));
        int secIndx = Mathf.RoundToInt(fracPri*(biomeDetail-1));
        int mapAt = -1;
        switch (map)
        {
            case 0:
                mapAt = biomePX[priIndx, secIndx];
                break;
            case 1:
                mapAt = biomeNX[priIndx, secIndx];
                break;
            case 2:
                mapAt = biomePY[priIndx, secIndx];
                break;
            case 3:
                mapAt = biomeNY[priIndx, secIndx];
                break;
            case 4:
                mapAt = biomePZ[priIndx, secIndx];
                break;
            case 5:
                mapAt = biomeNZ[priIndx, secIndx];
                break;
        }
        if (mapAt == -1)
        {
            Debug.Log("Invalid biome wtf");
            return biomeList[0];
        }
        return biomeList[mapAt];
    }

    void Start()
    {
        noise = new Noise(12345);
        snf = new SimpleNoiseFilter(12345,baseFrequency,deltaFrequency,persistence,volatility,octaves,Center);
        rnf = new RidgedNoiseFilter(12345,sharpness,baseFrequency,deltaFrequency,persistence,volatility,octaves,Center);
        biomeList = new List<Biome>();
        biomeMap = new int[biomeDetail, biomeDetail];
        biomePX = new int[biomeDetail, biomeDetail];
        biomePY = new int[biomeDetail, biomeDetail];
        biomePZ = new int[biomeDetail, biomeDetail];
        biomeNX = new int[biomeDetail, biomeDetail];
        biomeNY = new int[biomeDetail, biomeDetail];
        biomeNZ = new int[biomeDetail, biomeDetail];
        for (int x=0; x<biomeDetail; x++)
        {
            for (int y=0; y<biomeDetail; y++)
            {
                biomePX[x, y] = -1;
                biomePY[x, y] = -1;
                biomePZ[x, y] = -1;
                biomeNX[x, y] = -1;
                biomeNY[x, y] = -1;
                biomeNZ[x, y] = -1;
            }
        }
        GenerateBiomes();
        Debug.Log("+x Map");
        for (int y=0; y<biomeDetail; y++)
        {
            string tstr = "";
            for (int x=0; x<biomeDetail; x++)
            {
                tstr += biomePX[x, y];
                tstr += ",";
            }
            Debug.Log(tstr);
        }
        Debug.Log("+y Map");
        for (int y = 0; y < biomeDetail; y++)
        {
            string tstr = "";
            for (int x = 0; x < biomeDetail; x++)
            {
                tstr += biomePY[x, y];
                tstr += ",";
            }
            Debug.Log(tstr);
        }
        Debug.Log("+z Map");
        for (int y = 0; y < biomeDetail; y++)
        {
            string tstr = "";
            for (int x = 0; x < biomeDetail; x++)
            {
                tstr += biomePZ[x, y];
                tstr += ",";
            }
            Debug.Log(tstr);
        }
        Debug.Log("-x Map");
        for (int y = 0; y < biomeDetail; y++)
        {
            string tstr = "";
            for (int x = 0; x < biomeDetail; x++)
            {
                tstr += biomeNX[x, y];
                tstr += ",";
            }
            Debug.Log(tstr);
        }
        Debug.Log("-y Map");
        for (int y = 0; y < biomeDetail; y++)
        {
            string tstr = "";
            for (int x = 0; x < biomeDetail; x++)
            {
                tstr += biomeNY[x, y];
                tstr += ",";
            }
            Debug.Log(tstr);
        }
        Debug.Log("-z Map");
        for (int y = 0; y < biomeDetail; y++)
        {
            string tstr = "";
            for (int x = 0; x < biomeDetail; x++)
            {
                tstr += biomeNZ[x, y];
                tstr += ",";
            }
            Debug.Log(tstr);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    float getHeightmap(float x, float y, float z)
    {
        bool simple = true;
        Vector3 sphcoords = cubeToPolar((int)x, (int)y, (int)z);
        switch (GetBiomeAt(x,y,z).terrainType)
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
            heightmap = snf.Noise(cubeToUnitSphere(x,y,z));
        }
        else
        {
            heightmap = rnf.Noise(cubeToUnitSphere(x,y,z));
        }
        return heightmap + heightOffset;
    }

    public float getHeightAt(float x, float y, float z)
    {
        // Mountains 1: Simple BF 1.5 DF 0.5 V 30 P 0.5 MiH 85 MaH 120 HO 85 O 4
        // Mountains 2: Ridged BF 1.5 DF 0.5 V 30 S 2 P 0.5 MiH 85 MaH 120 HO 85 O 4
        // Plains: Simple BF 3 DF 0.5 V 3 P 0.5 MiH 77 MaH 120 HO 75 O 4
        // Foothills: Simple BF 4 DF 0.5 V 5 P 0.5 MiH 77 MaH 120 HO 75 O 4
        // Desert: Ridged BF 4 DF 0.5 V 5 S 2 P 0.5 MiH 77 MaH 120 HO 75 O 4
        // Plateaus: Ridged BF 1.5 DF 2 V 20 S 2 P 0.5 MiH 85 MaH 100 HO 85 O 4
        
        /*float numT = (theta * circum) / (2 * Mathf.PI * smoothingFactor);
        float numP = (psi * circum) / (Mathf.PI * smoothingFactor);
        float minPsi = (Mathf.Floor(numP) * Mathf.PI * smoothingFactor) / circum;
        float maxPsi = (Mathf.Ceil(numP) * Mathf.PI * smoothingFactor) / circum;
        float minTheta = (Mathf.Floor(numT) * 2 * Mathf.PI * smoothingFactor) / circum;
        float maxTheta = (Mathf.Ceil(numT) * 2 * Mathf.PI * smoothingFactor) / circum;*/
        float height;
        //if (GetBiomeAt(minPsi, minTheta).terrainType == GetBiomeAt(minPsi, maxTheta).terrainType && GetBiomeAt(minPsi, maxTheta).terrainType == GetBiomeAt(maxPsi, maxTheta).terrainType && GetBiomeAt(maxPsi, maxTheta).terrainType == GetBiomeAt(maxPsi, minTheta).terrainType)
        //{
            //biomes agree
            //height = getHeightmap(psi, theta);
        //}
        //else
        //{
            float heightmap000 = getHeightmap(x-smoothingFactor,y-smoothingFactor,z-smoothingFactor);
            float min000 = minHeight;
            float max000 = maxHeight;
            float heightmap010 = getHeightmap(x-smoothingFactor,y+smoothingFactor,z-smoothingFactor);
            float min010 = minHeight;
            float max010 = maxHeight;
            float heightmap100 = getHeightmap(x+smoothingFactor,y-smoothingFactor,z-smoothingFactor);
            float min100 = minHeight;
            float max100 = maxHeight;
            float heightmap110 = getHeightmap(x+smoothingFactor,y+smoothingFactor,z+smoothingFactor);
            float min110 = minHeight;
            float max110 = maxHeight;
            float heightmap001 = getHeightmap(x - smoothingFactor, y - smoothingFactor, z - smoothingFactor);
            float min001 = minHeight;
            float max001 = maxHeight;
            float heightmap011 = getHeightmap(x - smoothingFactor, y + smoothingFactor, z - smoothingFactor);
            float min011 = minHeight;
            float max011 = maxHeight;
            float heightmap101 = getHeightmap(x + smoothingFactor, y - smoothingFactor, z - smoothingFactor);
            float min101 = minHeight;
            float max101 = maxHeight;
            float heightmap111 = getHeightmap(x + smoothingFactor, y + smoothingFactor, z + smoothingFactor);
            float min111 = minHeight;
            float max111 = maxHeight;
            height = (heightmap000 + heightmap001 + heightmap010 + heightmap011 + heightmap100 + heightmap101 + heightmap110 + heightmap111) / 8;
            minHeight = (min000 + min001 + min010 + min011 + min100 + min101 + min110 + min111) / 8;
            maxHeight = (max000 + max001 + max010 + max011 + max100 + max101 + max110 + max111) / 8;
        //return heightmap01*25+75;
        //float height = heightmap01 + heightOffset;

        //}
        //if (height < minHeight) height = minHeight;
        //if (height > maxHeight) height = maxHeight;
        return height;
        //return 75;
    }

    public Vector3 cubeToUnitSphere(float x, float y, float z)
    {
        float dist = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y), Mathf.Abs(z));
        float mx = (float)x / dist;
        float my = (float)y / dist;
        float mz = (float)z / dist;
        float tx = mx * Mathf.Sqrt(1 - (Mathf.Pow(my, 2) / 2) - (Mathf.Pow(mz, 2) / 2) + (Mathf.Pow(my, 2) * Mathf.Pow(mz, 2) / 3)) * dist;
        float ty = my * Mathf.Sqrt(1 - (Mathf.Pow(mz, 2) / 2) - (Mathf.Pow(mx, 2) / 2) + (Mathf.Pow(mz, 2) * Mathf.Pow(mx, 2) / 3)) * dist;
        float tz = mz * Mathf.Sqrt(1 - (Mathf.Pow(mx, 2) / 2) - (Mathf.Pow(my, 2) / 2) + (Mathf.Pow(mx, 2) * Mathf.Pow(my, 2) / 3)) * dist;
        float rho = Mathf.Sqrt(tx * tx + ty * ty + tz * tz);
        Vector3 nr = new Vector3(tx / rho, ty / rho, tz / rho);
        return nr;
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
        //float rho = Mathf.Sqrt(tx * tx + ty * ty + tz * tz);
        float rho = dist;
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
                return GetBiomeAt(x,y,z).colorGrass;
            } else
            {
                return GetBiomeAt(x, y, z).colorUnderground;
            }
            //return GetBiomeAt(polar.y, polar.z).GetColor();
            //return new Color32(255, 189, 109, 255);
        }
        if (dist > maxHeight)
        {
            return new Color32(0, 0, 0, 0);
        }
        float height = getHeightAt(x,y,z);
        if (dist < height)
        {
            //return GetBiomeAt(polar.y, polar.z).GetColor();
            if (dist > mountainHeight)
            {
                return GetBiomeAt(x, y, z).colorMtns;
            } else if (dist > height - 2)
            {
                return GetBiomeAt(x, y, z).colorGrass;
            } else
            {
                return GetBiomeAt(x, y, z).colorUnderground;
            }
            //return new Color32(255, 189, 109, 255);
        } else
        {
            return new Color32(0, 0, 0, 0);
        }
    }
}
