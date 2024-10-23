using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotRotation : MonoBehaviour
{
    private List<GameObject> activeSide;
    private Vector3 localForward;
    private Vector3 mouseRef;
    private bool dragging = false;
    private bool autoRotating = false;
    private float sensitivity = 3.0f;
    private float speed = 300f;
    private Vector3 rotation;

    private Quaternion targetQuaternion;

    private ReadCube readCube;
    private CubeState cubeState;

    // Start is called before the first frame update
    void Start()
    {
        readCube = FindObjectOfType<ReadCube>();
        cubeState = FindObjectOfType<CubeState>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dragging)
        {
            SpinSide(activeSide);
            if (Input.GetMouseButtonUp(0))
            {
                dragging = false;
                RotateToRightAngle();
            }
        }
        if (autoRotating)
        {
            AutoRotate();
        }
    }

    private void SpinSide(List<GameObject> side)
    {
        rotation = Vector3.zero;

        // マウスのオフセットを取得
        Vector3 mouseOffset = (Input.mousePosition - mouseRef);

        // スワイプ方向を計算
        Vector3 camRight = Camera.main.transform.right;
        Vector3 camUp = Camera.main.transform.up;
        Vector3 swipeDirection = (camRight * mouseOffset.x + camUp * mouseOffset.y).normalized;

        // デバッグログでスワイプ方向を表示
        Debug.Log($"Swipe Direction: {swipeDirection}");

        // スワイプ方向に基づいて回転を設定
        if (side == cubeState.front)
        {
            rotation.x = swipeDirection.y * sensitivity * -1;
        }
        else if (side == cubeState.up)
        {
            rotation.y = swipeDirection.x * sensitivity * -1;
        }
        else if (side == cubeState.down)
        {
            rotation.x = swipeDirection.y * sensitivity * 1;
        }
        else if (side == cubeState.left)
        {
            rotation.z = swipeDirection.x * sensitivity * 1;
        }
        else if (side == cubeState.right)
        {
            rotation.z = swipeDirection.x * sensitivity * -1;
        }
        else if (side == cubeState.back)
        {
            rotation.y = swipeDirection.x * sensitivity * 1;
        }

        // デバッグログで回転ベクトルを表示
        Debug.Log($"Rotation: {rotation}");

        // 回転を適用
        transform.Rotate(rotation, Space.Self);

        // マウスの参照を更新
        mouseRef = Input.mousePosition;
    }



    // 面の回転を開始する
    public void Rotate(List<GameObject> side)
    {
        activeSide = side;
        mouseRef = Input.mousePosition;
        dragging = true;

        // 回転するベクトルを設定（中心のローカル前方から作成）
        localForward = Vector3.zero - side[4].transform.parent.transform.localPosition;
    }

    // 自動的に面を回転させる
    public void StartAutoRotate(List<GameObject> side, float angle)
    {
        cubeState.PickUp(side);
        Vector3 localForward = Vector3.zero - side[4].transform.parent.transform.localPosition;
        targetQuaternion = Quaternion.AngleAxis(angle, localForward) * transform.localRotation;
        activeSide = side;
        autoRotating = true;
    }

    // 正しい角度に自動で調整する
    public void RotateToRightAngle()
    {
        Vector3 vec = transform.localEulerAngles;
        vec.x = Mathf.Round(vec.x / 90) * 90;
        vec.y = Mathf.Round(vec.y / 90) * 90;
        vec.z = Mathf.Round(vec.z / 90) * 90;

        targetQuaternion.eulerAngles = vec;
        autoRotating = true;
    }

    // 自動回転を実行する
    private void AutoRotate()
    {
        dragging = false;
        var step = speed * Time.deltaTime;
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetQuaternion, step);

        // 回転が完了した場合
        if (Quaternion.Angle(transform.localRotation, targetQuaternion) <= 1)
        {
            transform.localRotation = targetQuaternion;
            // 小さなキューブを元の親に戻す
            cubeState.PutDown(activeSide, transform.parent);
            readCube.ReadState();
            CubeState.autoRotating = false;
            autoRotating = false;
            dragging = false;
        }
    }
}
