using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//キューブの現在の面の記憶をゲームプロジェクトのリストの形式で保持する

public class CubeState : MonoBehaviour
{
    public List<GameObject> front = new List<GameObject>();
    public List<GameObject> back = new List<GameObject>();
    public List<GameObject> up = new List<GameObject>();
    public List<GameObject> down = new List<GameObject>();
    public List<GameObject> left = new List<GameObject>();
    public List<GameObject> right = new List<GameObject>();

    public static bool autoRotating = false;
    public static bool started = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PickUp(List<GameObject> cubeSide)
    {
        foreach (GameObject face in cubeSide)
        {
            //各faceの親(小さな立方体)を取り付ける
            //4番目のインデックスの親(中央の小さな立方体)
            //既に第4の指標でない限り
            if (face != cubeSide[4])
            {
                face.transform.parent.transform.parent = cubeSide[4].transform.parent;
            }

        }
        //side rotation logicを起動する
        cubeSide[4].transform.parent.GetComponent<PivotRotation>().Rotate(cubeSide);

    }
    public void PutDown(List<GameObject> littleCubes, Transform pivot)
    {
        foreach (GameObject littleCube in littleCubes)
        {
            if (littleCube != littleCubes[4])
            {
                littleCube.transform.parent.transform.parent = pivot;
            }
        }
    }
}

