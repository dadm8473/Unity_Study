using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public int type;    // 블럭의 타입
    public bool vis;    // 보여지는가 안 보여지는가
    public GameObject obj;

    public Block(int t, bool v, GameObject blockobj)
    {
        type = t;
        vis = v;
        obj = blockobj;
    }
}


public class MapGenerator : MonoBehaviour {

    [Header("[Block]")]
    public GameObject B_SoilPrefab;
    public GameObject B_GoldPrefab;
    public GameObject B_BedPrefab;
    public GameObject B_GrassPrefab;
    public GameObject B_SandPrefab;
    public GameObject B_SnowPrefab;
    
    [Header("[Map Info]")]
    static public int Width_x = 125;
    static public int Width_z = 125;
    static public int height  = 125;
    public float GroundHeightOffset = 20;
    public float Wavelength = 0;    // 진폭
    public float Amplitude = 0;     // 파장의 최대 높이

    public Block[,,] worldBlock = new Block[Width_x, height, Width_z];  // 
    //private List<GameObject> BlockList = new List<GameObject>();

	void Start ()
    {
        StartCoroutine(InitGame());
    }

    //public bool Istest = false;

	void Update ()
    {
        //if (!Istest)
        //    return;

        //for (int i = 0; i < BlockList.Count; i++)
        //{
        //    float xCoord = (BlockList[i].transform.position.x) / Wavelength;
        //    float zCoord = (BlockList[i].transform.position.z) / Wavelength;
        //    int y = (int)(Mathf.PerlinNoise(xCoord, zCoord) * Amplitude);
        //    BlockList[i].transform.position = new Vector3(BlockList[i].transform.position.x, y, BlockList[i].transform.position.z);
        //}
    }

    IEnumerator InitGame()
    {
        // 맵 만들기
        yield return StartCoroutine(MapInit());

        // 구름 만들기
        
        // 동굴 만들기
    }

    private float seed;

    IEnumerator MapInit()
    {
        seed = (int)Random.Range(0, 100);

        for(int x = 0; x < Width_x; ++x)
        {
            for(int z = 0; z < Width_z; ++z)
            {
                float xCoord = (x + seed) / Wavelength;
                float zCoord = (z + seed) / Wavelength;
                int y = (int)(Mathf.PerlinNoise(xCoord, zCoord) * Amplitude + GroundHeightOffset);
                Vector3 pos = new Vector3(x, y, z);
                StartCoroutine(CreateBlock(y, pos, true));
                while(y > 0)
                {
                    y--;
                    pos = new Vector3(x, y, z);
                    StartCoroutine(CreateBlock(y, pos, false));
                }
            }
        }
        
        //for (int x = 0; x < Width_x; x++)
        //{
        //    for (int z = 0; z < Width_z; z++)
        //    {
        //        float xCoord = (BlockList[i].transform.position.x) / Wavelength;
        //        float zCoord = (BlockList[i].transform.position.z) / Wavelength;
        //        int y = (int)(Mathf.PerlinNoise(xCoord, zCoord) * Amplitude);
        //        //BlockList.Add((GameObject)Instantiate(B_GrassPrefab, new Vector3(x, 0, z), Quaternion.identity));
        //    }
        //}

        //for (int i = 0; i < BlockList.Count; i++)
        //{
        //    float xCoord = (BlockList[i].transform.position.x) / Wavelength;
        //    float zCoord = (BlockList[i].transform.position.z) / Wavelength;
        //    int y = (int)(Mathf.PerlinNoise(xCoord, zCoord) * Amplitude);
        //    //BlockList[i].transform.position = new Vector3(BlockList[i].transform.position.x, y, BlockList[i].transform.position.z);
        //}

        yield return null;
    }

    IEnumerator CreateBlock(int y, Vector3 blockpos, bool visual)
    {
        if ( y > 40 )
        {   // 눈
            if(visual)
            {
                GameObject BlockObj = (GameObject)Instantiate(B_SnowPrefab, blockpos, Quaternion.identity);
                worldBlock[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(1, visual, BlockObj);
            }
            else
            {
                worldBlock[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(1, visual, null);
            }
        }
        else if( y > 25 )
        {   // 잔디
            if (visual)
            {
                GameObject BlockObj = (GameObject)Instantiate(B_GrassPrefab, blockpos, Quaternion.identity);
                worldBlock[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(2, visual, BlockObj);
            }
            else
            {
                worldBlock[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(2, visual, null);
            }
        }
        else
        {   // 모래
            if (visual)
            { 
                GameObject BlockObj = (GameObject)Instantiate(B_SandPrefab, blockpos, Quaternion.identity);
                worldBlock[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(3, visual, BlockObj);
            }
            else
            {
                worldBlock[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(3, visual, null);
            }
        }

        if(y > 0 && y < 7 && Random.Range(0, 100) < 3)
        {   // 광물
            if(worldBlock[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z].obj != null)
            {
                Destroy(worldBlock[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z].obj);
            }

            if (visual)
            {
                GameObject BlockObj = (GameObject)Instantiate(B_GoldPrefab, blockpos, Quaternion.identity);
                worldBlock[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(4, visual, BlockObj);
            }
            else
            {
                worldBlock[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(4, visual, null);
            }
        }

        // 0층 베드락
        if (0 == y)
        {
            if (visual)
            {
                GameObject BlockObj = (GameObject)Instantiate(B_BedPrefab, blockpos, Quaternion.identity);
                worldBlock[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(5, visual, BlockObj);
            }
            else
            {
                worldBlock[(int)blockpos.x, (int)blockpos.y, (int)blockpos.z] = new Block(5, visual, null);
            }
        }

        yield return null;
    }
}
