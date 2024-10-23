using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//選択した面に基づいて回転する側を選択する

public class SelectFace : MonoBehaviour
{

    //立方体の状態と立方体の読み取りのスクリプトにアクセスする
    CubeState cubeState;
    ReadCube readCube;

    //マウスから発せられるレイキャストを使用し、再び画面に向かって発射する(これはfacesに当たるだけ)
    int layerMask = 1 << 8;

    // Start is called before the first frame update
    void Start()
    {
        readCube = FindObjectOfType<ReadCube>();
        cubeState = FindObjectOfType<CubeState>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !CubeState.autoRotating)
        {
            //立方体の現在の状態を読み取る
            readCube.ReadState();

            //マウスからキューブに向かってレイキャストして、faceがヒットするかどうか確認する
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f, layerMask))
            {
                GameObject face = hit.collider.gameObject;
                //全てのsidesのリストを作成する(face GameObjectのリスト)
                List<List<GameObject>> cubeSides = new List<List<GameObject>>()
                {
                    cubeState.up,
                    cubeState.down,
                    cubeState.left,
                    cubeState.right,
                    cubeState.front,
                    cubeState.back
                };
                //faceの当たりがそれぞれの側面内に存在する場合
                foreach (List<GameObject> cubeSide in cubeSides)
                {
                    if (cubeSide.Contains(face))
                    {
                        //選ぶ
                        cubeState.PickUp(cubeSide);

                    }
                }

            }

        }
    }
}
