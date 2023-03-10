using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenCamera : MonoBehaviour
{
    private RawImage showPicture;
    private WebCamTexture webCamTexture;
    public int width = 640, hight = 480, fps=30;
  
    void Start()
    {
        StartCoroutine("IOpenCamera");
    }

    /// <summary>
    /// 打开摄像机
    /// </summary> 
    public IEnumerator IOpenCamera()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            if (webCamTexture != null)
            {
                webCamTexture.Stop();
            }

            // 监控第一次授权，是否获得到设备（因为很可能第一次授权了，但是获得不到设备，这里这样避免）
            // 多次 都没有获得设备，可能就是真没有摄像头，结束获取 camera
            int i = 0;
            while (WebCamTexture.devices.Length <= 0 && 1 < 300)
            {
                yield return new WaitForEndOfFrame();
                i++;
            }
            WebCamDevice[] devices = WebCamTexture.devices;//获取可用设备
            if (WebCamTexture.devices.Length <= 0)
                Debug.LogError("没有获取到摄像头设备！");
            else
            {
                string devicename = devices[0].name;
                webCamTexture = new WebCamTexture(devicename, width, hight, fps)
                {
                    wrapMode = TextureWrapMode.Repeat
                };
                showPicture=GetComponent<RawImage>();
                // 渲染到 UI  
                if (showPicture != null)
                    showPicture.texture = webCamTexture;

                webCamTexture.Play();
            }

        }
        else
            Debug.LogError("未获得读取摄像头权限");
    }

    private void OnApplicationPause(bool pause)
    {
        // 应用暂停的时候暂停camera，继续的时候继续使用
        if (webCamTexture != null)
        {
            if (pause)
            {
                webCamTexture.Pause();
            }
            else
            {
                webCamTexture.Play();
            }
        }

    }
    private void OnDestroy()
    {
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }
}

