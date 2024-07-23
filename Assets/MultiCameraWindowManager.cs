using UnityEngine;

public class MultiCameraWindowManager : MonoBehaviour
{
    public Camera camera1;
    public Camera camera2;

    private RenderTexture renderTexture1;
    private RenderTexture renderTexture2;

    private Rect windowRect1;
    private Rect windowRect2;

    void Start()
    {
        // 创建两个 RenderTexture 并将其附加到不同的摄像机上
        renderTexture1 = new RenderTexture(800, 600, 24);
        renderTexture2 = new RenderTexture(800, 600, 24);

        camera1.targetTexture = renderTexture1;
        camera2.targetTexture = renderTexture2;

        // 设置窗口的初始位置和大小
        windowRect1 = new Rect(50, 50, 800, 600);
        windowRect2 = new Rect(900, 50, 800, 600);
    }

    void OnGUI()
    {
        // 绘制第一个摄像机的 RenderTexture 到第一个窗口
        windowRect1 = GUI.Window(0, windowRect1, DrawWindow1, "Camera 1");

        // 绘制第二个摄像机的 RenderTexture 到第二个窗口
        windowRect2 = GUI.Window(1, windowRect2, DrawWindow2, "Camera 2");
    }

    void DrawWindow1(int windowID)
    {
        // 绘制 RenderTexture
        GUI.DrawTexture(new Rect(0, 0, windowRect1.width, windowRect1.height), renderTexture1);

        // 允许窗口被拖动
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }

    void DrawWindow2(int windowID)
    {
        // 绘制 RenderTexture
        GUI.DrawTexture(new Rect(0, 0, windowRect2.width, windowRect2.height), renderTexture2);

        // 允许窗口被拖动
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }
}