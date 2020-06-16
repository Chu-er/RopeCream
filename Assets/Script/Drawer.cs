using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class Drawer : MonoBehaviour
{

    RectTransform _rect;
    Painter _painter;
    public Texture[] source;

    public Material realLineMat;
    public Material lineEdgeDectedMat;

    [Header("画线的点")]
    public RectTransform DrawTrans;

    Vector2 _drawScreenPos;

    #region  奶油相关
    public int currentPaintID = 0;

    [HideInInspector]
    public Painter currentPainter;
    [Header("本步骤所有用到的")]
    public Texture[] creamTex;
    [Header("所有的奶油Painter")]
    public Painter[] creamPainter;

    public RawImage reciveRaw;
    Texture currentSourceCreamTex;


    Vector2 painCreamPos;

    Vector2[] painterScreen;


    bool isStartPaint = false;

    public float interval = 0.1f;

    #endregion

    private  RenderTexture detectedTex;

    void Start()
    {
        _painter = GetComponent<Painter>();
        _painter.Init();

        _rect = transform as RectTransform;
        realLineMat.mainTexture = _painter.renderTexture;
        //lineEdgeDectedMat.mainTexture = _painter.renderTexture;
        _painter.renderTexture.wrapMode = TextureWrapMode.Repeat;
        ChangeDrawPos();
        StartPainteCream(creamTex[0]);
        detectedTex = _painter.renderTexture;
       
        painterScreen = new Vector2[creamPainter.Length];
        for (int i = 0; i < creamPainter.Length; i++)
        {
            painterScreen[i] = Camera.main.WorldToScreenPoint(creamPainter[i].transform.position);
        }
    }


  

    void DrawByMouse()
    {
        Vector2 localPos;

        bool isArea = RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, Input.mousePosition, Camera.main, out localPos);
        if (isArea)
        {   
            _painter.Drawing(localPos, null, _painter.renderTexture2, false, true);
            _painter.DrawRT2OtherRT(_painter.renderTexture2, _painter.renderTexture, _painter.canvasMat2);
        }
    }

    public void DrawPaint()
    {
        DrawTrans.position = new Vector3(transform.position.x, DrawTrans.position.y);
        ChangeDrawPos();
        Vector2 localPos;
        bool isArea = RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, _drawScreenPos, Camera.main, out localPos);
        if (isArea)
        {
            _painter.Drawing(localPos, null, _painter.renderTexture2, false, true);
            _painter.DrawRT2OtherRT(_painter.renderTexture2, _painter.renderTexture, _painter.canvasMat2);
        }
    }

    /// <summary>
    /// 改变line的图像
    /// </summary>
    /// <param name="index"></param>
    public void ChangeSource(int index)
    {
        if (_painter)
        {
            _painter.sourceTex = source[index];
            _painter.ClearCanvas(_painter.renderTexture2);
        }
    }

    float time = 0f;
    
    /// <summary>
    /// 根据当前点来确定 线的颜色
    /// </summary>
    /// <returns></returns>
    public void  GetTxtureByPixel()
    {
        if (currentPainter==null)
        {
            Debug.LogError("currentPainter 是 空");
            return ;
        }

        time += Time.deltaTime;
        if (time <= interval)
        {
            
            return;
        }
        

        //Transform dect = creamPainter[currentPaintID].transform;
        Vector2 localPos;
        bool isArea =  RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, painterScreen[currentPaintID], Camera.main, out localPos);

        int scrY = detectedTex.height / 2 + (int)localPos.y;

        Texture2D temp = new Texture2D(detectedTex.width, detectedTex.height, TextureFormat.ARGB32, false);
        RenderTexture actived = RenderTexture.active;
        RenderTexture.active = detectedTex;
        temp.ReadPixels(new Rect(0, 0, detectedTex.width, detectedTex.height), 20, 50);
        temp.Apply();
        RenderTexture.active = actived;
        reciveRaw.texture = temp;
        Color rece =  temp.GetPixel(detectedTex.width / 4*3, scrY);
        Debug.Log(scrY+"Color"+rece);

        time = 0;
    }


    /// <summary>
    /// 初始化奶油Painter
    /// </summary>
    public void StartPainteCream(Texture source=null )
    {
        if (isStartPaint)
        {
            return;
        }
        currentPainter = creamPainter[currentPaintID];
        currentPainter.gameObject.SetActive(true);
        currentPainter.sourceTex = source;
        currentPainter.Init();
        currentPainter.renderTexture.wrapMode = TextureWrapMode.Repeat;
        isStartPaint = true;
    }



    /// <summary>
    /// 改变Cream的图像
    /// </summary>
    /// <param name="index"></param>
    public void ChangeCurrentPainter(int index)
    {
        if (currentPainter)
        {
            currentPainter.sourceTex = source[index];
            currentPainter.ClearCanvas(_painter.renderTexture2);
        }
    }


    /// <summary>
    /// 修改画线的点
    /// </summary>
    public void ChangeDrawPos()
    {
        _drawScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, DrawTrans.position);
    }

    [Header("测试索引")]
    public int sourceIndex = 0;
    [ContextMenu("ChangeSource")]
    public void ChangeSourceTest()
    {
        if (_painter)
        {
            _painter.sourceTex = source[sourceIndex];
            _painter.ClearCanvas(_painter.renderTexture2);
        }
    }


}
