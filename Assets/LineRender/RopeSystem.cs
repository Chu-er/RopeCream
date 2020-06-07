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


    [Header("拉伸速度")]
    public float StreachSpeed=1f;
    [Header("间隔时间")]
    public float IntervalTime = 0.15f;
    [Header("力大小")]
    public float ForceRate = 1f;


    Transform[] pointTrans;
    LineRenderer _lineRender;
    Rigidbody2D _rootRigidBody;
    GameObject _newGenerateJointObj;
    /// <summary>
    /// 每次要连接的第一根关节  即 PointGroup 的
    /// </summary>
    Rigidbody2D _firstRig2D;

    /// <summary>
    /// PointGroup 要拉长的三根关节
    /// </summary>
    DistanceJoint2D _firstJoint2D;
    DistanceJoint2D _secondJoint2D;
    DistanceJoint2D _thirdJoint2D;


    #region  数值 数据
    float _pressTime = 0f;
    float _tempStreach = 0f;



    #endregion
    public bool enableDrawLine = false;
    public bool enbaleInput = true;

    void Awake(){
        _lineRender = GetComponent<LineRenderer>();
        _rootRigidBody = PointGroup.GetComponent<Rigidbody2D>();
    }
    void Start()
    {
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

        if (_lineRender.positionCount != PointGroup.childCount)
        {
            _lineRender.positionCount = PointGroup.childCount;

        }
        for (int i = 0; i < PointGroup.childCount; i++)
        {
             _lineRender.SetPosition(i,PointGroup.GetChild(i).position);
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
        if (!enbaleInput)
        {
            return;
        }


        if (Input.GetKeyDown(KeyCode.Q))
        {
            Press_1.color =  Color.red;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            // _rootRigidBody.MovePosition( new Vector2( _rootRigidBody.position.x- Time.deltaTime/2f, _rootRigidBody.position.y)  );
            _pressTime += Time.deltaTime;
            if (_pressTime >= IntervalTime)
            {
                generateJointObj();
                _pressTime = 0f;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
           
            ResetData();
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

    /// <summary>
    /// 生成一个 关节预设 单独     TODO  对象池
    /// </summary>
    void generateJointObj()
    {
        if (PointGroup ==null)
        {
            Debug.LogError("PoinrGroup  is null  check");
            return;
        }
        if (_firstRig2D == null)
        {
            _firstRig2D = PointGroup.GetComponent<Rigidbody2D>();
        }
        if (_firstJoint2D ==null)
        {
            _firstJoint2D = PointGroup.GetChild(1).GetComponent<DistanceJoint2D>();
            _secondJoint2D = PointGroup.GetChild(2).GetComponent<DistanceJoint2D>();
            _thirdJoint2D = PointGroup.GetChild(3).GetComponent<DistanceJoint2D>();
        }
        _tempStreach = Time.deltaTime * StreachSpeed;


        _firstJoint2D.distance += _tempStreach;
        _secondJoint2D.distance += _tempStreach;
        _thirdJoint2D.distance += _tempStreach;


        _firstJoint2D.attachedRigidbody.position += new Vector2(0, _tempStreach*0.98f);
        _secondJoint2D.attachedRigidbody.position += new Vector2(0, _tempStreach * 0.98f);
        _thirdJoint2D.attachedRigidbody.position += new Vector2(0, _tempStreach * 0.98f);


        float tempForce = ForceRate;
        if (_firstJoint2D.distance>=0.77f)
        {
            tempForce = 10f;
        }
        _firstJoint2D.attachedRigidbody.AddForce(new Vector2(Random.onUnitSphere.x * tempForce, 0f), ForceMode2D.Force);
        _secondJoint2D.attachedRigidbody.AddForce(new Vector2(Random.onUnitSphere.x * tempForce, 0f), ForceMode2D.Impulse);
        _thirdJoint2D.attachedRigidbody.AddForce(new Vector2(Random.onUnitSphere.x * tempForce, 0f),ForceMode2D.Impulse);
        //_firstJoint2D.attachedRigidbody.angularVelocity = Vecto

    }


    /// <summary>
    /// 按钮抬起  重置数据
    /// </summary>
    void ResetData()
    {
        _tempStreach = 0f;
        _pressTime = 0f;
        Press_1.color = Color.white;

        if (_pointGroupDrop==null)
        {
            //_pointGroupDrop = StartCoroutine(pointGroupDrop());
        }
    }

    UnityEngine.Coroutine _pointGroupDrop = null;
    IEnumerator pointGroupDrop()
    {
        if (PointGroup == null)
        {
             yield break;
        }

        Rigidbody2D first = PointGroup.GetChild(0).GetComponent<Rigidbody2D>();
        first.isKinematic = false;
        PointGroup.GetChild(0).GetComponent<DistanceJoint2D>().enabled = false;
        //for (float i = _firstRig2D.position.y; i >= toValue; i-= rate)
        //{
        //    _firstRig2D.position = new Vector2(_firstRig2D.position.x, i);
        //    yield return new WaitForSeconds(0.1f);
        //}
        yield return new WaitForEndOfFrame();
        _pointGroupDrop = null;
    }



    // void Init



}
