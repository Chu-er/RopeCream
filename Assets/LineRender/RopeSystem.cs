using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class RopeSystem : MonoBehaviour
{
    /// <summary>
    /// 存储lineRenderer
    /// </summary>
    /// <typeparam name="LineRenderer"></typeparam>
    /// <returns></returns>
    public static List<GameObject>  _tempLineList = new List<GameObject>();
    public static GameObject CurrentLineObj;

    public static float MaxDistance = 1.4f;
    static bool isInitLine = false;
    static Transform _linePoolTrans;
    static GameObject _lineSystemObj;
    static Vector2[] _recordLinePos = null;

    public Image Press_1;
    public Image Press_2;
    [Header("一整个关节组预设物")]
    public Transform PointGroup;
    [Header("拉伸速度")]
    public float StreachSpeed=1f;
    [Header("间隔时间")]
    public float IntervalTime = 0.15f;
    [Header("力大小")]
    public float ForceRate = 1f;


    Transform[] pointTrans;
    LineRenderer _lineRender;
    /// <summary>
    /// PointGroup 要拉长的三根关节
    /// </summary>
    [Header("")]
    public DistanceJoint2D _firstJoint2D;
    public DistanceJoint2D _secondJoint2D;
    public DistanceJoint2D _thirdJoint2D;


    #region  数值 数据
    float _pressTime = 0f;
    float _tempStreach = 0f;

    #endregion
    public bool enableDrawLine = false;
    public bool enbaleInput = true;

    void Awake(){
        _lineRender = GetComponent<LineRenderer>();
        if (_lineSystemObj==null)
        {
            _lineSystemObj = this.gameObject;
        }
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
        Invoke("SetPosition", 0.5f);
        InitTempLine();
    }

    /// <summary>
    /// 修改lineRenderer的点
    /// </summary>
    public void SetPosition(){

        if (_lineRender.positionCount != PointGroup.childCount)
        {
            _lineRender.positionCount = PointGroup.childCount;

        }
        for (int i = 0; i < pointTrans.Length; i++)
        {
             _lineRender.SetPosition(i, pointTrans[i].position);
        }
        if (_recordLinePos == null)
        {
            _recordLinePos = new Vector2[PointGroup.childCount];
            for (int i = 0; i < pointTrans.Length; i++)
            {
                _recordLinePos[i] = pointTrans[i].position;
            }
        }
    }

    void Update()
    {
        SetPosition();
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
           
            onMouseUp();
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
        if (_firstJoint2D.distance> MaxDistance)
        {
            AddForce(2f);
            return;
        }
        _tempStreach = Time.deltaTime * StreachSpeed;
        _firstJoint2D.distance += _tempStreach;
        _secondJoint2D.distance += _tempStreach;
        _thirdJoint2D.distance += _tempStreach;

        _firstJoint2D.attachedRigidbody.position += new Vector2(0, _tempStreach*0.98f);
        _secondJoint2D.attachedRigidbody.position += new Vector2(0, _tempStreach * 0.98f);
        _thirdJoint2D.attachedRigidbody.position += new Vector2(0, _tempStreach * 0.98f);
        AddForce();
    }
    /// <summary>
    /// 施加左右摇摆的力
    /// </summary>
    void AddForce(float forceFactor = 1)
    {
        float tempForce = ForceRate;
        if (_firstJoint2D.distance >= 0.77f)
        {
            tempForce = 10f;
        }

        _firstJoint2D.attachedRigidbody.AddForce(new Vector2(Random.onUnitSphere.x * tempForce* forceFactor, 0f), ForceMode2D.Force);
        _secondJoint2D.attachedRigidbody.AddForce(new Vector2(Random.onUnitSphere.x * tempForce * forceFactor, 0f), ForceMode2D.Impulse);
        _thirdJoint2D.attachedRigidbody.AddForce(new Vector2(Random.onUnitSphere.x * tempForce * forceFactor, 0f), ForceMode2D.Impulse);
    }

    /// <summary>
    /// 指针抬起
    /// </summary>
    private void onMouseUp()
    {
        _tempStreach = 0f;
        _pressTime = 0f;
        Press_1.color = Color.white;

        if (_pointGroupDrop == null &&  _firstJoint2D.distance>0.2f)
        {
            _pointGroupDrop = StartCoroutine(pointGroupDrop());
        }
    }

    UnityEngine.Coroutine _pointGroupDrop = null;
    IEnumerator pointGroupDrop()
    {

        Rigidbody2D first = PointGroup.GetChild(0).GetComponent<Rigidbody2D>();
        first.isKinematic = false;
        PointGroup.GetChild(0).GetComponent<DistanceJoint2D>().enabled = false;
        Color col_1 = new Color(1, 1, 1, 1);
        Color col_2 = new Color(1, 1, 1, 0);
        yield return _lineRender.DOColor(new Color2(col_1, col_1), new Color2(col_2, col_2), 2).WaitForCompletion();
        ResetData();
        yield return new WaitForSeconds(1.2f);
        _lineRender.startColor = Color.white;
        _lineRender.endColor = Color.white;

        RopeSystem.BackPoolLine(gameObject);
        GameObject newLine = RopeSystem.GetLine<GameObject>();


        newLine.transform.SetParent(null);
        newLine.transform.position = Vector3.zero;
        newLine.SetActive(true);
        _pointGroupDrop = null;
    }

    /// <summary>
    /// 重置数据
    /// </summary>
    void ResetData()
    {
        _firstJoint2D.distance = 0.1f;
        _secondJoint2D.distance = 0.1f;
        _thirdJoint2D.distance = 0.1f;
        for (int i = 0; i < PointGroup.childCount; i++)
        {
            DistanceJoint2D child = PointGroup.GetChild(i).GetComponent<DistanceJoint2D>();
            child.attachedRigidbody.velocity = Vector2.zero;
            if (i==0)
            {
                child.attachedRigidbody.isKinematic = true;
                child.enabled = true;
                Debug.Log(_recordLinePos[0]);
                Debug.Log(child.attachedRigidbody.position+"Posi");
                child.transform.position = _recordLinePos[0];
            }
            else
            {
                child.attachedRigidbody.MovePosition(_recordLinePos[i]);
            }
            child.attachedRigidbody.MovePosition(_recordLinePos[i]);

        }
      
    }

    /// <summary>
    ///初始化绳
    /// </summa子数量ry>
    public static void InitTempLine(int count = 10)
    {
        if (isInitLine)
        {
            return;
        }
        if (_tempLineList.Count>0)
        {
            _tempLineList.Clear();
        }
        _linePoolTrans = GameObject.Find("LineRendererPool").transform;
        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(_lineSystemObj, _linePoolTrans);
            _tempLineList.Add(go);
            go.SetActive(false);
        }
        isInitLine = true;
    }

    public static T GetLine<T>() where T : class
    {
        T temp ;
        temp = _tempLineList[0] as T;
        _tempLineList.RemoveAt(0);
        return temp;
    }

    /// <summary>
    /// 回收到池子里
    /// </summary>
    public static void BackPoolLine(GameObject go)
    {
        go.transform.SetParent(_linePoolTrans);
        _tempLineList.Add(go);
        go.SetActive(false);
    }


    public static void ClearTempLine()
    {
        _tempLineList.Clear();
    }


}
