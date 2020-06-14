using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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

    void Start()
    {
        _painter = GetComponent<Painter>();
        _painter.Init();

        _rect = transform as RectTransform;
        realLineMat.mainTexture = _painter.renderTexture;
        //lineEdgeDectedMat.mainTexture = _painter.renderTexture;
        _painter.renderTexture.wrapMode = TextureWrapMode.Repeat;
        ChangeDrawPos();
    }



    void Update()
    {
       
        if (Input.GetMouseButton(0))
        {
            DrawByMouse();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _painter.EndDraw();
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


    public void ChangeSource(int index)
    {
        if (_painter)
        {
            _painter.sourceTex = source[index];
            _painter.ClearCanvas(_painter.renderTexture2);
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
