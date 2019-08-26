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
    public float blockSetValue;
    
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
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 100f, Color.red);

            if(Physics.Raycast(ray, out hit, 1000.0f))
            {
                Vector3 blockPos = hit.transform.position;

                if (blockPos.y <= 0) return;

                worldBlock[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = null;
                Destroy(hit.collider.gameObject);

                for (int x = -1; x <= 1; ++x)
                    for (int y = -1; y <= 1; ++y)
                        for (int z = -1; z <= 1; ++z)
                        {
                            if ((!(x == 0 && y == 0 && z == 0)))
                            {
                                if (blockPos.x + x < 0 || blockPos.x + x > Width_x) continue;
                                if (blockPos.y + y < 0 || blockPos.y + y > height) continue;
                                if (blockPos.y + z < 0 || blockPos.z + z > Width_z) continue;

                                Vector3 neighbour = new Vector3(blockPos.x + x, blockPos.y + y, blockPos.z + z);
                                DrawBlock(neighbour);
                            }
                        }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 100f, Color.red);

            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                Vector3 setBlockPos = (Camera.main.transform.position - hit.transform.position).normalized;

                Debug.Log(setBlockPos.y);

                if(setBlockPos.x > 0)
                    setBlockPos.x = setBlockPos.x < blockSetValue ? 0 : 1;
                else
                    setBlockPos.x = setBlockPos.x > -blockSetValue ? 0 : -1;

                if (setBlockPos.y > 0)
                    setBlockPos.y = setBlockPos.y < blockSetValue ? 0 : 1;
                else
                    setBlockPos.y = setBlockPos.y > -blockSetValue ? 0 : -1;

                if (setBlockPos.z > 0)
                    setBlockPos.z = setBlockPos.z < blockSetValue ? 0 : 1;
                else
                    setBlockPos.z = setBlockPos.z > -blockSetValue ? 0 : -1;

                Vector3 blockPos = hit.transform.position + setBlockPos;

                GameObject BlockObj = (GameObject)Instantiate(B_GrassPrefab, blockPos, Quaternion.identity);
                worldBlock[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(2, true, BlockObj);
                Debug.Log("블럭 설치");

                //if (blockPos.y <= 0) return;

                //worldBlock[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = null;
                //Destroy(hit.collider.gameObject);

                //for (int x = -1; x <= 1; ++x)
                //    for (int y = -1; y <= 1; ++y)
                //        for (int z = -1; z <= 1; ++z)
                //        {
                //            if ((!(x == 0 && y == 0 && z == 0)))
                //            {
                //                if (blockPos.x + x < 0 || blockPos.x + x > Width_x) continue;
                //                if (blockPos.y + y < 0 || blockPos.y + y > height) continue;
                //                if (blockPos.y + z < 0 || blockPos.z + z > Width_z) continue;

                //                Vector3 neighbour = new Vector3(blockPos.x + x, blockPos.y + y, blockPos.z + z);
                //                DrawBlock(neighbour);
                //            }
                //        }
            }
        }

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
        Debug.Log("맵 생성");

        // 구름 만들기
        yield return StartCoroutine(CreateCloud(120, 50, 80));
        Debug.Log("구름 생성");

        // 동굴 만들기
        yield return StartCoroutine(CreateMines(5, 400));
        Debug.Log("동굴 생성");
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

    // 땅을 팠을 때 주변 블럭을 생성
    private void DrawBlock(Vector3 blockPos)
    {
        if (worldBlock[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] == null) return;

        if(!worldBlock[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].vis)
        {
            GameObject newBlock = null;

            worldBlock[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].vis = true;

            if (worldBlock[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].type == 1)
                newBlock = (GameObject)Instantiate(B_SnowPrefab, blockPos, Quaternion.identity);
            else if (worldBlock[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].type == 2)
                newBlock = (GameObject)Instantiate(B_GrassPrefab, blockPos, Quaternion.identity);
            else if (worldBlock[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].type == 3)
                newBlock = (GameObject)Instantiate(B_SandPrefab, blockPos, Quaternion.identity);
            else if (worldBlock[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].type == 4)
                newBlock = (GameObject)Instantiate(B_GoldPrefab, blockPos, Quaternion.identity);
            else if (worldBlock[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].type == 5)
                newBlock = (GameObject)Instantiate(B_BedPrefab, blockPos, Quaternion.identity);
            else
                worldBlock[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].vis = false;

            if (newBlock != null)
                worldBlock[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].obj = newBlock;
        }
    }

    IEnumerator CreateMines(int Num, int UnitMineSize)
    {
        int HoleSize = 1;

        for (int i = 0; i < Num; ++i)
        {
            int xpos = Random.Range(HoleSize, Width_x - HoleSize);
            int ypos = Random.Range(HoleSize, 15);
            int zpos = Random.Range(HoleSize, Width_z - HoleSize);

            for (int j = 0; j < UnitMineSize; ++j)
            {
                for (int x = -HoleSize; x <= HoleSize; ++x)
                    for (int y = -HoleSize; y <= HoleSize; ++y)
                        for (int z = -HoleSize; z <= HoleSize; ++z)
                        {
                            Vector3 blockPos = new Vector3(xpos + x, ypos + y, zpos + z);

                            if (worldBlock[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] != null)
                            {
                                if(worldBlock[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].type == 5 ||
                                   worldBlock[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].type == 4)
                                {
                                    // 현재 타입이 골드이거나 베드락이면 뚫지 않는다.
                                    continue;
                                }
                                else
                                {
                                    Destroy(worldBlock[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].obj);
                                    worldBlock[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = null;
                                }
                            }
                        }

                while(true)
                {
                    xpos += Random.Range(-1, 2);
                    if (xpos < HoleSize || xpos >= Width_x - HoleSize) continue;
                    else break;
                }
                while(true)
                {
                    zpos += Random.Range(-1, 2);
                    if (zpos < HoleSize || zpos >= Width_z - HoleSize) continue;
                    else break;
                }
                while(true)
                {
                    ypos += Random.Range(-1, 2);
                    if (ypos < HoleSize || ypos >= height - HoleSize) continue;
                    else break;
                }
            }

            for(int z = 1; z < Width_z - 1; ++z)
            {
                for(int x = 1; x < Width_x - 1; ++x)
                {
                    for(int y = 1; y < height - 1; ++y)
                    {
                        if(worldBlock[x, y, z] == null)
                        {
                            for(int x1 = -1; x1 <= 1; ++x1)
                            {
                                for(int y1= -1; y1 <= 1; ++y1)
                                {
                                    for(int z1 = -1; z1 <= 1; ++z1)
                                    {
                                        if(!(x1 == 0 && y1 == 0 && z1 == 0))
                                        {
                                            Vector3 neighbour = new Vector3(x + x1, y + y1, z + z1);
                                            DrawBlock(neighbour);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        yield return null;
    }

    IEnumerator CreateCloud(int Num, int unitCloudSize, int Cloudbeight)
    {
        for(int  i = 0; i < Num; ++i)
        {
            int xpos = Random.Range(0, Width_x + 200);
            int zpos = Random.Range(0, Width_z + 200);

            for(int j = 0; j < unitCloudSize; ++j)
            {
                Vector3 blockPos = new Vector3(xpos, Cloudbeight, zpos);
                Instantiate(B_SnowPrefab, blockPos, Quaternion.identity);
                xpos += Random.Range(-1, 2);
                zpos += Random.Range(-1, 2);
            }
        }

        yield return null;
    }
}
