using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//初めに6つのレイ変換でゲームオブジェクトをスクリプトに変換する。これにより、レイの開始がわかる。

public class ReadCube : MonoBehaviour
{
    public Transform tUp;
    public Transform tDown;
    public Transform tLeft;
    public Transform tRight;
    public Transform tFront;
    public Transform tBack;

    private List<GameObject> frontRays = new List<GameObject>();
    private List<GameObject> backRays = new List<GameObject>();
    private List<GameObject> upRays = new List<GameObject>();
    private List<GameObject> downRays = new List<GameObject>();
    private List<GameObject> leftRays = new List<GameObject>();
    private List<GameObject> rightRays = new List<GameObject>();


    private int layerMask = 1 << 8; //このレイヤーマスクは立方体の読み取り後にのみ立方体の面に適用される
    CubeState cubeState;
    public GameObject emptyGO;


    // Start is called before the first frame update
    void Start()
    {
        SetRayTransforms();

        cubeState = FindObjectOfType<CubeState>();
        ReadState();
        CubeState.started = true;


    }

    // Update is called once per frame
    void Update()
    {
        //ReadState();
    }

    public void ReadState()
    {
        cubeState = FindAnyObjectByType<CubeState>();

        //サイドリストの各位置の状態を私たちが知っているように設定をする
        //どの色がどの位置にあるか
        cubeState.up = ReadFace(upRays, tUp);
        cubeState.down = ReadFace(downRays, tDown);
        cubeState.left = ReadFace(leftRays, tLeft);
        cubeState.right = ReadFace(rightRays, tRight);
        cubeState.front = ReadFace(frontRays, tFront);
        cubeState.back = ReadFace(backRays, tBack);


    }


    void SetRayTransforms()
    {
        //立方体マップと同じように左上にRay0,右上にRay8をもつれいを立方体に当てるためのRay void setのれい変換により、立方体に向かって角度をつけた変換から発せられる
        //れいキャストをれいリストに追加する

        upRays = BuildRays(tUp, new Vector3(90, 90, 0));
        downRays = BuildRays(tDown, new Vector3(0, 270, 0));
        leftRays = BuildRays(tLeft, new Vector3(0, 180, 0));
        rightRays = BuildRays(tRight, new Vector3(0, 0, 0));
        frontRays = BuildRays(tFront, new Vector3(0, 90, 0));
        backRays = BuildRays(tBack, new Vector3(270, 90, 0));
    }

    List<GameObject> BuildRays(Transform rayTransform, Vector3 direction)
    {
        //レイのカウント初期化
        int rayCount = 0;
        //レイのリスト生成、初期化
        List<GameObject> rays = new List<GameObject>();
        //左上にRay0,右下にRay8を持つ立方体の側面の形で隆起したキューブが作成される
        //|0|1|2|
        //|3|4|5|
        //|6|7|8|
        //これを作るためにx方向とy方向のループ

        for (int y = 1; y > -2; y--)
        {
            for (int x = -1; x < 2; x++)
            {
                //レイの始点を計算
                Vector3 startPos = new Vector3(rayTransform.localPosition.x + x,
                                                rayTransform.localPosition.y + y,
                                                rayTransform.localPosition.z);
                //空のゲームオブジェクトを作成してレイの始点に配置
                GameObject rayStart = Instantiate(emptyGO, startPos, Quaternion.identity, rayTransform);
                //生成された順に名前をつける
                rayStart.name = rayCount.ToString();
                rays.Add(rayStart);
                rayCount++;
            }
        }
        //レイの発射方向を指定された方向にむける
        rayTransform.localRotation = Quaternion.Euler(direction);
        return rays;
    }

    public List<GameObject> ReadFace(List<GameObject> rayStarts, Transform rayTransform)
    {
        List<GameObject> facesHit = new List<GameObject>();

        foreach (GameObject rayStart in rayStarts)
        {
            Vector3 ray = rayStart.transform.position;
            RaycastHit hit;

            //layerMask(face)内のオブジェクトと交差をするのか？
            if (Physics.Raycast(ray, rayTransform.forward, out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(ray, rayTransform.forward * hit.distance, Color.yellow);
                facesHit.Add(hit.collider.gameObject);
                //print(hit.collider.gameObject.name);
            }
            else
            {
                Debug.DrawRay(ray, rayTransform.forward * 1000, Color.green);
            }
        }
        return facesHit;


    }

}