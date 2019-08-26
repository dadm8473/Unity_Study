using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public GameObject[] g_CheckPint = new GameObject[] {};

    bool[] b_Check = new bool[7] {false, false, false, false, false, false, false};

    Transform t_StartTransform;

    public GameObject g_EndingUI;

    void Start()
    {
        // 시작 트렌스폼 저장
        t_StartTransform = this.transform;

        Debug.Log(b_Check.Length);
    }
    void Update()
    {
        // 스타트 화면 시작

        // R 누르면 초기화
        if(Input.GetKeyDown(KeyCode.R))
            Reset_Player();

        if (Input.GetKeyDown(KeyCode.V))
            Ending();

        // 지나간 시간을 보여줌

        // 끝나면 리스타트 / 종료
    }

    void Ending()
    {
        g_EndingUI.SetActive(true);
        Time.timeScale = 0;
    }

    void Reset_Player()
    {
        // 플레이어 포지션과 로테이션 초기화 밑 체크포인트 초기화
        SceneManager.LoadScene(0);

        Reset_Check();
    }

    void Reset_Check()
    {
        // 체크포인트 초기화
        for (int i = 0; i < b_Check.Length; i++)
            b_Check[i] = false;
    }

    void OnTriggerEnter(Collider col)
    {
        // 전 체크포인트에 도달하지 못했다면 초기화
        switch (col.gameObject.tag)
        {
            case "Ch1":
                b_Check[0] = true;
                break;
            case "Ch2":
                if (b_Check[0] == true)
                    b_Check[1] = true;
                else
                    Reset_Player();
                break;
            case "Ch3":
                if (b_Check[1] == true)
                    b_Check[2] = true;
                else
                    Reset_Player();
                break;
            case "Ch4":
                if (b_Check[2] == true)
                    b_Check[3] = true;
                else
                    Reset_Player();
                break;
            case "Ch5":
                if (b_Check[3] == true)
                    b_Check[4] = true;
                else
                    Reset_Player();
                break;
            case "Ch6":
                if (b_Check[4] == true)
                    b_Check[5] = true;
                else
                    Reset_Player();
                break;
            case "Ch7":
                if (b_Check[5] == true)
                    b_Check[6] = true;
                else
                    Reset_Player();
                break;
            case "Line":
                if(b_Check[6] == true)
                    Ending();
                break;
        }
    }
}
