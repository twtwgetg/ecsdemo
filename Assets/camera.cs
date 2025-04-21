using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
public class CameraController : MonoBehaviour
{
    public float panSpeed = 0.5f; // 平移速度
    public float zoomSpeed = 0.1f; // 缩放速度
    public float minZoom = 5f; // 最小缩放距离
    public float maxZoom = 50f; // 最大缩放距离

    private Camera cam;
    private Vector3 lastMousePosition; // 记录鼠标上一次的位置

    void Start()
    {
        // 获取当前的摄像机组件
        cam = Camera.main;
    }

    void Update()
    {
        // 检测是否在编辑器模式下
#if UNITY_EDITOR
        HandleEditorInput();
#else
        HandleTouchInput();
#endif
    }

    // 处理移动端的触摸输入
    private void HandleTouchInput()
    {
        if (Input.touchCount == 1) // 单指操作
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                // 根据手指滑动的方向平移摄像机
                Vector3 deltaPosition = new Vector3(-touch.deltaPosition.x, -touch.deltaPosition.y, 0) * panSpeed * Time.deltaTime;
                transform.Translate(deltaPosition, Space.World);
            }
        }
        else if (Input.touchCount == 2) // 双指操作
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // 计算当前两指之间的距离
            float currentDistance = Vector2.Distance(touch1.position, touch2.position);

            // 计算上一帧两指之间的距离
            float previousDistance = Vector2.Distance(touch1.position - touch1.deltaPosition, touch2.position - touch2.deltaPosition);

            // 计算距离差
            float distanceDelta = currentDistance - previousDistance;

            // 根据距离差缩放摄像机
            float newSize = cam.orthographicSize - distanceDelta * zoomSpeed * Time.deltaTime;
            cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom); // 限制缩放范围
        }
    }

    // 处理编辑器模式下的鼠标输入
    private void HandleEditorInput()
    {
        // 鼠标滚轮缩放摄像机
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f) // 检测滚轮是否滚动
        {
            float newSize = scroll * zoomSpeed * 1000f * Time.deltaTime;
            cam.transform.position += cam.transform.forward * newSize;
        }

        // 鼠标左键按住并拖动平移摄像机
        if (Input.GetMouseButton(0)) // 检测鼠标左键是否按下
        {
            if (Input.GetMouseButtonDown(0))
            {
                // 记录鼠标按下时的位置
                lastMousePosition = Input.mousePosition;
            }

            // 计算鼠标移动的偏移量
            Vector3 deltaMousePosition = Input.mousePosition - lastMousePosition;

            // 根据鼠标移动的方向平移摄像机
            Vector3 deltaPosition = new Vector3(-deltaMousePosition.x, -deltaMousePosition.y, 0) * panSpeed * Time.deltaTime;
            transform.Translate(deltaPosition, Space.World);

            // 更新鼠标位置
            lastMousePosition = Input.mousePosition;
        }
    }
}
