using FFmpeg.Demo.REC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private FFmpegREC fFmpegREC;
    [SerializeField]
    private Button quitbtn, startbtn, stopBtn;
    // Start is called before the first frame update
    void Start()
    {
         
        //初始化FFmpeg实例
        fFmpegREC.Init(OutPut, OnFinish);
        quitbtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        startbtn.onClick.AddListener(() =>
        {
            fFmpegREC.StartREC();

        });
        stopBtn.onClick.AddListener(() =>
        {
            fFmpegREC.StopREC();
        });
    }
    /// <summary>
    /// 输出路径回调
    /// </summary>
    /// <param name="org1"></param>
    private void OutPut(string org1)
    {
        
    }
    /// <summary>
    /// 输出完成提示回调
    /// </summary>
    /// <param name="org1"></param>
    private void OnFinish(string org1)
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
