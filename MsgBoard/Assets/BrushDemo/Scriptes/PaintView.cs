using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems; 
using UnityEngine.UI;


public class PaintView : MonoBehaviour
{
    #region 属性

    //绘图shader&material
    [SerializeField]
    private Shader _paintBrushShader;
    private Material _paintBrushMat;
    //清理renderTexture的shader&material
    [SerializeField]
    private Shader _clearBrushShader;
    private Material _clearBrushMat;
    //默认笔刷RawImage
    [SerializeField]
    private RawImage _defaultBrushRawImage;
    //默认笔刷&笔刷合集
    [SerializeField]
    private Texture _defaultBrushTex;
    //renderTexture
    private RenderTexture _renderTex;
    //默认笔刷RawImage
    [SerializeField]
    private Image _defaultColorImage;
    //绘画的画布
    [SerializeField]
    private RawImage _paintCanvas=null;
    //笔刷的默认颜色&颜色合集
    [SerializeField]
    private Color _defaultColor;
    //笔刷大小的slider
    private Text _brushSizeText;
    //笔刷的大小
    private float _brushSize;
    //屏幕的宽高
    private int _screenWidth;
    private int _screenHeight;
    //笔刷的间隔大小
    private float _brushLerpSize;
    //默认上一次点的位置
    private Vector2 _lastPoint;

    #endregion
    void Start()
    {
        SetNewPaint(); 
    }
    public void SetNewPaint() 
    {
        if (_paintCanvas!=null)
        {
            DestroyImmediate(_paintCanvas.gameObject);
            _paintCanvas= null;
        }
        //RawImage渲染实例
        _paintCanvas = Instantiate(Resources.Load<GameObject>("Prefabs/RawImage_canvas"),transform).GetComponent<RawImage>();
        //设置层级
        _paintCanvas.transform.SetAsFirstSibling();
        //添加拖拽事件
        Config.AddTriggersListener(_paintCanvas.gameObject, EventTriggerType.Drag, OnDrag);
        Config.AddTriggersListener(_paintCanvas.gameObject, EventTriggerType.EndDrag, OnDragEnd);
        InitData();
    }
    private void OnDragEnd(BaseEventData arg0)
    {
        if (Input.GetMouseButtonUp(0))
            _lastPoint = Vector2.zero;
    }

    private void OnDrag(BaseEventData arg0)
    {
        if (_renderTex && _paintBrushMat)
        {

            if (Input.GetMouseButton(0))
                LerpPaint(Input.mousePosition); 
        }
    }

    private void Update()
	{
		Color clearColor = new Color(0, 0, 0, 0);
        //if (Input.GetKeyDown(KeyCode.Space))
           // SetNewPaint();
             //_paintBrushMat.SetColor("_Color", clearColor);
    }


    #region 外部接口
    /// <summary>
    /// 设置笔刷大小
    /// </summary>
    /// <param name="size"></param>
    public void SetBrushSize(float size)
    {
       _brushSize = size;
       _paintBrushMat.SetFloat("_Size", _brushSize);
    }
    /// <summary>
    /// 设置笔刷纹理
    /// </summary>
    /// <param name="texture"></param>
    public void SetBrushTexture(Texture texture)
    {
        _defaultBrushTex = texture;
        _paintBrushMat.SetTexture("_BrushTex", _defaultBrushTex);
        _defaultBrushRawImage.texture = _defaultBrushTex;
    }
    /// <summary>
    /// 设置笔刷大小
    /// </summary>
    /// <param name="color"></param>
    public void SetBrushColor(Color color)
    {
        _defaultColor = color;
        _paintBrushMat.SetColor("_Color", _defaultColor);
        _defaultColorImage.color = _defaultColor;
    }
   
    /// <summary>
    /// 通过Slider设置笔刷大小
    /// </summary>
    /// <param name="value"></param>
    public void BrushSizeChanged(Slider slider)
    {
      //  float value = slider.maxValue + slider.minValue - slider.value;
        SetBrushSize(Remap(slider.value,300.0f,30.0f));
        if (_brushSizeText == null)
        {
            _brushSizeText=slider.transform.Find("Background/Text").GetComponent<Text>();
        }
        _brushSizeText.text = slider.value.ToString("f2");
    }

    /// <summary>
    /// 拖拽
    /// </summary>
    public void DragUpdate()
    {
        if (_renderTex && _paintBrushMat)
        {

            if (Input.GetMouseButton(0))
                LerpPaint(Input.mousePosition);

           
        }
    }
    /// <summary>
    /// 拖拽结束
    /// </summary>
    public void DragEnd()
    {
        if (Input.GetMouseButtonUp(0))
            _lastPoint = Vector2.zero;
    }

    #endregion

    #region 内部函数
	
    //初始化数据
    void InitData()
    {
        _brushSize = 300.0f;
        _brushLerpSize = (_defaultBrushTex.width + _defaultBrushTex.height) / 2.0f / _brushSize;
        _lastPoint = Vector2.zero;

        if (_paintBrushMat == null)
        {
            UpdateBrushMaterial();
        }
        if(_clearBrushMat==null)
        _clearBrushMat = new Material(_clearBrushShader);
        if (_renderTex == null)
        {
            _screenWidth = Screen.width;
            _screenHeight = Screen.height;

            _renderTex = RenderTexture.GetTemporary(_screenWidth, _screenHeight, 24);
            _paintCanvas.texture = _renderTex;
        }
        Graphics.Blit(null, _renderTex, _clearBrushMat);
    }

    //更新笔刷材质
    private void UpdateBrushMaterial()
    {
        _paintBrushMat = new Material(_paintBrushShader);
        _paintBrushMat.SetTexture("_BrushTex", _defaultBrushTex);
        _paintBrushMat.SetColor("_Color", _defaultColor);
        _paintBrushMat.SetFloat("_Size", _brushSize);
    }

    //插点
    private void LerpPaint(Vector2 point)
    {
        Paint(point);

        if (_lastPoint == Vector2.zero)
        {
            _lastPoint = point;
            return;
        }

        float dis = Vector2.Distance(point, _lastPoint);
        if (dis > _brushLerpSize)
        {
            Vector2 dir = (point - _lastPoint).normalized;
            int num = (int)(dis / _brushLerpSize);
            for (int i = 0; i < num; i++)
            {
                Vector2 newPoint = _lastPoint + dir * (i + 1) * _brushLerpSize;
                Paint(newPoint);
            }
        }
        _lastPoint = point;
    }

    //画点
    private void Paint(Vector2 point)
    {
        if (point.x < 0 || point.x > _screenWidth || point.y < 0 || point.y > _screenHeight)
            return;

        Vector2 uv = new Vector2(point.x / (float)_screenWidth,
            point.y / (float)_screenHeight);
        _paintBrushMat.SetVector("_UV", uv);
        Graphics.Blit(_renderTex, _renderTex, _paintBrushMat);
    }
    /// <summary>
    /// 重映射  默认  value 为1-100
    /// </summary>
    /// <param name="value"></param>
    /// <param name="maxValue"></param>
    /// <param name="minValue"></param>
    /// <returns></returns>
    private float Remap(float value, float startValue, float enValue)
    {
        float returnValue = (value - 1.0f) / (100.0f - 1.0f);
        returnValue = (enValue - startValue) * returnValue + startValue;
        return returnValue;
    }

    #endregion

}
public class Config 
{
    /// <summary>
    /// 绑定拖拽功能
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="eventID"></param>
    /// <param name="action"></param>
    public static void AddTriggersListener(GameObject obj, EventTriggerType eventID, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = obj.AddComponent<EventTrigger>();
        }

        if (trigger.triggers.Count == 0)
        {
            trigger.triggers = new List<EventTrigger.Entry>();
        }

        UnityAction<BaseEventData> callback = new UnityAction<BaseEventData>(action);
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventID;
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }

}