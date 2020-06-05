using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RopeSystem : MonoBehaviour
{
    /// <summary>
    /// 存储lineRenderer
    /// </summary>
    /// <typeparam name="LineRenderer"></typeparam>
    /// <returns></returns>
    List<LineRenderer>  _tempLineList = new List<LineRenderer>();
    /// <summary>
    /// 保存单个关节的池子
    /// </summary>
    /// <typeparam name="GameObject"></typeparam>
    /// <returns></returns>
    List<GameObject>  _jointObjList = new List<GameObject>();
    
    public Image Press_1;
    public Image Press_2;
    
    [Header("一整个关节组预设物")]
    public Transform PointGroup;
    [Header("关节预设物")]
    public GameObject JointPrefab;


    [Header("节点距离")]
    public float DistanceJoint = 0.4f;

     Transform[] pointTrans;

    LineRenderer _lineRender;
    Rigidbody2D _rootRigidBody;
    

    public bool enableDrawLine = false;
    void Awake(){
        _lineRender = GetComponent<LineRenderer>();
        _rootRigidBody = PointGroup.GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        _lineRender.positionCount=PointGroup.childCount;
        pointTrans = new Transform[PointGroup.childCount];
        for (int i = 0; i < PointGroup.childCount; i++)
        {
            Transform child = PointGroup.GetChild(i);
            pointTrans[i] =  child;
        }
        Debug.Log("初始化数组"+pointTrans.Length );
    }

    /// <summary>
    /// 修改lineRenderer的点
    /// </summary>
    public void SetPosition(){
        for (int i = 0; i < pointTrans.Length; i++)
        {
             _lineRender.SetPosition(i,pointTrans[i].position);
        }
    }

    void Update()
    {
        if (enableDrawLine && PointGroup !=null  ){
            SetPosition();
        }
        DetectInput();

    }

    /// <summary>
    /// 获取输入 
    /// </summary>
    void DetectInput(){
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Press_1.color =  Color.red;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            // _rootRigidBody.MovePosition( new Vector2( _rootRigidBody.position.x- Time.deltaTime/2f, _rootRigidBody.position.y)  );

        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            Press_1.color =  Color.white;

        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Press_2.color =  Color.red;
            
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            
        }
        else if(Input.GetKeyUp(KeyCode.W))
        {
            Press_2.color =  Color.white;
        }
    }
    // void Init
    
}
