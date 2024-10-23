using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBigCube : MonoBehaviour
{
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    Vector3 previousMousePosition;

    public GameObject target;

    int layerMask = 1 << 8; // 面のレイヤーマスク
    bool isFaceClicked = false;
    bool isDragging = false; // ドラッグ中かどうかのフラグ

    float speed = 200f;
    Quaternion initialRotation; // クリック時の回転状態を保持

    // 回転角度を固定
    public float fixedRotationAngle = 90f; // 90度ずつ回転

    void Start()
    {
        initialRotation = transform.rotation; // 初期の回転状態を保存
    }

    void Update()
    {
        DetectFaceClick();

        if (!isFaceClicked)
        {
            Swipe();
            Drag();
        }
    }

    void Drag()
    {
        // ドラッグ中の回転処理を削除し、マウスの動きに応じて回転をしない
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // ターゲットに回転を合わせる処理
        if (transform.rotation != target.transform.rotation)
        {
            var step = speed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target.transform.rotation, step);
        }

        previousMousePosition = Input.mousePosition;
    }

    void DetectFaceClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f, layerMask))
            {
                isFaceClicked = true;
            }
            else
            {
                isFaceClicked = false;
            }
        }
    }

    void Swipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        if (Input.GetMouseButtonUp(0))
        {
            secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);
            currentSwipe.Normalize();

            // スワイプ方向に応じて固定角度で回転
            if (LeftSwipe(currentSwipe))
            {
                target.transform.Rotate(0, fixedRotationAngle, 0, Space.World);
                Debug.Log("左に90度回転");
            }
            else if (RightSwipe(currentSwipe))
            {
                target.transform.Rotate(0, -fixedRotationAngle, 0, Space.World);
                Debug.Log("右に90度回転");
            }
            else if (UpLeftSwipe(currentSwipe))
            {
                target.transform.Rotate(fixedRotationAngle, 0, 0, Space.World);
                Debug.Log("上に90度回転");
            }
            else if (UpRightSwipe(currentSwipe))
            {
                target.transform.Rotate(0, 0, -fixedRotationAngle, Space.World);
                Debug.Log("上に90度回転");
            }
            else if (DownLeftSwipe(currentSwipe))
            {
                target.transform.Rotate(0, 0, fixedRotationAngle, Space.World);
                Debug.Log("下に90度回転");
            }
            else if (DownRightSwipe(currentSwipe))
            {
                target.transform.Rotate(-fixedRotationAngle, 0, 0, Space.World);
                Debug.Log("下に90度回転");
            }

            // ドラッグ終了時の回転状態を保存
            initialRotation = transform.rotation;
        }
    }

    bool LeftSwipe(Vector2 swipe)
    {
        return swipe.x < 0 && swipe.y > -0.5f && swipe.y < 0.5f;
    }

    bool RightSwipe(Vector2 swipe)
    {
        return swipe.x > 0 && swipe.y > -0.5f && swipe.y < 0.5f;
    }

    bool UpLeftSwipe(Vector2 swipe)
    {
        return swipe.y > 0 && swipe.x < 0f;
    }

    bool UpRightSwipe(Vector2 swipe)
    {
        return swipe.y > 0 && swipe.x > 0f;
    }

    bool DownLeftSwipe(Vector2 swipe)
    {
        return swipe.y < 0 && swipe.x < 0f;
    }

    bool DownRightSwipe(Vector2 swipe)
    {
        return swipe.y < 0 && swipe.x > 0f;
    }
}
