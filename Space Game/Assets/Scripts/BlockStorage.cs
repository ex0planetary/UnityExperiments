using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Make shit work
// I hate laundry, why tf is today so goddamn busy

public class BlockStorage : MonoBehaviour
{
    int[,,] nodes = new int[36, 36, 36];
    ChunkInfo info;

    private void Start()
    {
        info = GetComponent<ChunkInfo>();
    }

    public bool terrainAt(Vector3Int pos)
    {
        if (pos.x < 0 || pos.x > 35 || pos.y < 0 || pos.y > 35 || pos.z < 0 || pos.z > 35)
        {
            return false;
        } else
        {
            return nodes[pos.x + 1, pos.y + 1, pos.z + 1] == 1;
        }
    }

    public float getTerrainDensityAt(Vector3Int pos)
    {
        bool nnn, nnz, nnp, nzn, nzz, nzp, npn, npz, npp,
            znn, znz, znp, zzn, zzz, zzp, zpn, zpz, zpp,
            pnn, pnz, pnp, pzn, pzz, pzp, ppn, ppz, ppp;
        // CENTER
        zzz = terrainAt(pos + new Vector3Int(0,0,0));
        // FACES
        // +x
        pzz = terrainAt(pos + new Vector3Int(1, 0, 0));
        // -x
        nzz = terrainAt(pos + new Vector3Int(-1, 0, 0));
        // +y
        zpz = terrainAt(pos + new Vector3Int(0, 1, 0));
        // -y
        znz = terrainAt(pos + new Vector3Int(0, -1, 0));
        // +z
        zzp = terrainAt(pos + new Vector3Int(0, 0, 1));
        // -z
        zzn = terrainAt(pos + new Vector3Int(0, 0, -1));
        // CORNERS
        // ---
        nnn = terrainAt(pos + new Vector3Int(-1, -1, -1));
        // --+
        nnp = terrainAt(pos + new Vector3Int(-1, -1, 1));
        // -+-
        npn = terrainAt(pos + new Vector3Int(-1, 1, -1));
        // -++
        npp = terrainAt(pos + new Vector3Int(-1, 1, 1));
        // +--
        pnn = terrainAt(pos + new Vector3Int(1, -1, -1));
        // +-+
        pnp = terrainAt(pos + new Vector3Int(1, -1, 1));
        // ++-
        ppn = terrainAt(pos + new Vector3Int(1, 1, -1));
        // +++
        ppp = terrainAt(pos + new Vector3Int(1, 1, 1));
        // EDGES
        // --0
        nnz = terrainAt(pos + new Vector3Int(-1, -1, 0));
        // -0-
        nzn = terrainAt(pos + new Vector3Int(-1, 0, -1));
        // -0+
        nzp = terrainAt(pos + new Vector3Int(-1, 0, 1));
        // -+0
        npz = terrainAt(pos + new Vector3Int(-1, 1, 0));
        // 0--
        znn = terrainAt(pos + new Vector3Int(0, -1, -1));
        // 0-+
        znp = terrainAt(pos + new Vector3Int(0, -1, 1));
        // 0+-
        zpn = terrainAt(pos + new Vector3Int(0, 1, -1));
        // 0++
        zpp = terrainAt(pos + new Vector3Int(0, 1, 1));
        // +-0
        pnz = terrainAt(pos + new Vector3Int(1, -1, 0));
        // +0-
        pzn = terrainAt(pos + new Vector3Int(1, 0, -1));
        // +0+
        pzp = terrainAt(pos + new Vector3Int(1, 0, 1));
        // ++0
        ppz = terrainAt(pos + new Vector3Int(1, 1, 0));
        // sum
        int sum = 0;
        if (nnn) sum++;
        if (nnz) sum++;
        if (nnp) sum++;
        if (nzn) sum++;
        if (nzz) sum++;
        if (nzp) sum++;
        if (npn) sum++;
        if (npz) sum++;
        if (npp) sum++;
        if (znn) sum++;
        if (znz) sum++;
        if (znp) sum++;
        if (zzn) sum++;
        if (zzz) sum++;
        if (zzp) sum++;
        if (zpn) sum++;
        if (zpz) sum++;
        if (zpp) sum++;
        if (pnn) sum++;
        if (pnz) sum++;
        if (pnp) sum++;
        if (pzn) sum++;
        if (pzz) sum++;
        if (pzp) sum++;
        if (ppn) sum++;
        if (ppz) sum++;
        if (ppp) sum++;
        return (float)sum/27;
    }

    public IList<float> formatNodes()
    {
        float[] result;
        result = new float[34 * 34 * 34];
        for (int z = 0; z < 34; z++)
        {
            for (int y = 0; y < 34; y++)
            {
                for (int x = 0; x < 34; x++)
                {
                    /*if (nodes[x, y, z] == 1)
                        result[x + y * 34 + z * 34 * 34] = 1.0f;
                    else
                        result[x + y * 34 + z * 34 * 34] = 0.0f;*/
                    result[x + y * 34 + z * 34 * 34] = getTerrainDensityAt(new Vector3Int(x, y, z));
                }
            }
        }
        return result;
    }

    public void SetBlock(Vector3Int pos, int to)
    {
        nodes[pos.x + 18, pos.y + 18, pos.z + 18] = to;
    }
}
