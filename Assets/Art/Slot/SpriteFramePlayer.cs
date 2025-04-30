using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

[RequireComponent(typeof(Image))] [ExecuteAlways]
public class SpriteFramePlayer : MonoBehaviour
{
    [Header("动画帧序列（按顺序放）")]
    public Sprite[] frames;

    [Tooltip("帧率；比如 12 表示每秒 12 帧")]
    public float frameRate = 12f;
    
    public void SetFraneRate(float rate) { frameRate = rate; }

    [Tooltip("是否循环播放")]
    public bool loop = true;

    private Image _sr;
    private int _currentIndex;
    private float _timer;
    
    public bool AutoPlayInEditMode = false;

    void Awake()
    {
        _sr = GetComponent<Image>();
    }

    void OnEnable()
    {
        #if UNITY_EDITOR
        _lastEditorTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += EditorTick;
        #endif
    }

    
    void Tick(float dt)
    {
        _timer += dt;
        float frameTime = 1f / frameRate;

        while (_timer >= frameTime)
        {
            _timer -= frameTime;
            _currentIndex = loop
                ? (_currentIndex + 1) % frames.Length
                : Mathf.Min(_currentIndex + 1, frames.Length - 1);

            if (frames[_currentIndex] == null)
            {
                // create a small transparent texture
                Texture2D tex = new Texture2D(1, 1);
                tex.SetPixel(0, 0, new Color(0, 0, 0, 0));
                tex.Apply();
                Sprite emptySprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
                emptySprite.name = "EmptySprite";
                frames[_currentIndex] = emptySprite;
            }

            _sr.sprite = frames[_currentIndex];
            
            
        }
    }
    
    public void GoToSpecificFrame(int index)
    {
        if (index < 0 || index >= frames.Length) return;

        _currentIndex = index;
        
        _sr.sprite    = frames[_currentIndex];
    }

    void Update()
    {
        if (!Application.isPlaying || frames.Length == 0) return;
        Tick(Time.deltaTime);
    }

    // 可选：在代码里随时调用播放不同序列
    public void Play(Sprite[] newFrames, float newFrameRate = -1f, bool newLoop = true)
    {
        frames = newFrames;
        if (newFrameRate > 0f) frameRate = newFrameRate;
        loop = newLoop;
        OnEnable(); // 重新初始化
    }
    
    
    #if UNITY_EDITOR
    private double _lastEditorTime;

    
    void OnDisable()
    {
        EditorApplication.update -= EditorTick;
    }

    private void EditorTick()          // 仅在编辑模式下执行
    {
        if (Application.isPlaying) return;   // 运行时由 Update() 负责

        double now = EditorApplication.timeSinceStartup;
        float dt  = (float)(now - _lastEditorTime);
        _lastEditorTime = now;

        if (!AutoPlayInEditMode || frames.Length == 0) return;

        Tick(dt);

        // ① 强制下一帧 PlayerLoop —— 让 ExecuteInEditMode 脚本得以再次更新
        EditorApplication.QueuePlayerLoopUpdate();   //contentReference[oaicite:1]{index=1}

        // ② 立即重绘 Scene / Game 视图 —— 让你肉眼可见
        SceneView.RepaintAll();                      //contentReference[oaicite:2]{index=2}
        InternalEditorUtility.RepaintAllViews();
    }
#endif
}