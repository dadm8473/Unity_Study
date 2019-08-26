using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject g_Title;
    public Text timeText;
    private float timeText_Time;
    public GameObject timerObj;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        timeText_Time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timeText_Time += Time.deltaTime;
        timeText.text = string.Format("{0:N2}", timeText_Time);
    }

    public void StartButtonClick()
    {
        Time.timeScale = 1;
        if (g_Title)
            g_Title.SetActive(false);

        timerObj.SetActive(true);
    }

    public void EndingButtonClick()
    {
        SceneManager.LoadScene(0);
    }
}
