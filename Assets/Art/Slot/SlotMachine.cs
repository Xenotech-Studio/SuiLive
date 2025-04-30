using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class SlotMachine : MonoBehaviour
{
    /* ---------- 视觉 ---------- */
    [Header("=== 视觉 ===")]
    [Range(0, 1)] public float SlotProgress = 0f;
    public float StaticScrollSpeed = 1f;
    public Material SlotMaterial;

    /* ---------- 运行开关 ---------- */
    [Header("=== 运行开关 ===")]
    public bool PlayInEditMode = true;
    public bool SlotRunning    = false;

    /* ---------- 初速 & 阻力 ---------- */
    [Header("=== 初速 & 阻力 ===")]
    public Vector2 RunningInitialSpeedRange = new(0.5f, 1f);
    public float   CurrentSpeed   = 0.5f;
    public float   StoppingSpeed  = 0.3f;
    public float   Friction       = 0.5f;

    /* ---------- 弹簧对齐 ---------- */
    [Header("=== 弹簧对齐 ===")]
    public float AligningSpring   = 0.8f;
    public float AligningFriction = 0.6f;

    /* ---------- 精度 ---------- */
    [Header("=== 精度 & 性能 ===")]
    [Min(1)]  public int   SimulationStepsPerFrame = 1;
    public float MinFinalizeSpeed = 0.01f;
    public float MinFinalizeDelta = 1e-3f;

    /* ---------- 结果展示 ---------- */
    [Header("=== 结果展示 ===")]
    public float ResultHoldSeconds = 10f;

    /* ---------- 奖品数据 ---------- */
    [Header("=== 奖品数据 ===")]
    public List<string> SlotItems            = new();
    public List<float>  PrizeStartProgresses = new();   // 升序 ∈[0,1)
    [Tooltip("与 SlotItems 等长；每项 > 0；值越大越容易抽到")]
    public List<float> PrizeWeights = new();

    /* ---------- 调参模式 ---------- */
    [Header("=== 调参模式 ===")]
    public bool EnableTuningMode = false;
    public int  TunePrizeIndex   = 0;

    /* ---------- 私有状态 ---------- */
    [Header("=== 私有状态 (调试) ===")]
    public  bool  _aligning      = false;
    public  bool  _showingResult = false;
    public  int   _prizeIndex    = -1;
    public  bool  speedLow, deltaLow;

    private float _rawProgress   = 0f;
    private float _targetRaw     = 0f;
    private float _resultTimer   = 0f;
    private float _alignEntrySpeed = 0f;       // ★ 进入 Aligning 时的绝对速度

    public UnityEvent OnEnterNormal;
    public UnityEvent OnEnterRunning;
    public UnityEvent<string> OnEnterShowingResult;

    /* ---------- 生命周期 ---------- */
    private void OnValidate()
    {
        if (PrizeStartProgresses.Count != SlotItems.Count)
            Debug.LogWarning($"{name}: PrizeStartProgresses 必须与 SlotItems 等长");
    }

    /* ---------- 公共接口 ---------- */
    public void StartOneGame()
    {
        if (EnableTuningMode || SlotItems.Count == 0) return;

        SlotRunning    = true;
        _aligning      = false;
        _showingResult = false;
        _prizeIndex    = -1;

        _rawProgress   = SlotProgress;
        CurrentSpeed   = Random.Range(RunningInitialSpeedRange.x,
                                      RunningInitialSpeedRange.y);
        
        OnEnterRunning?.Invoke();
    }

    /* ---------- MonoBehaviour ---------- */
    private void Update()
    {
#if UNITY_EDITOR
        if (!PlayInEditMode && !Application.isPlaying) return;
#endif
        int   steps  = Mathf.Max(1, SimulationStepsPerFrame);
        float dtStep = Time.deltaTime / steps;

        for (int i = 0; i < steps; ++i)
        {
            if (EnableTuningMode)           TuningUpdate(dtStep);
            else if (_showingResult)        ResultHoldUpdate(dtStep);
            else if (SlotRunning)           RunningUpdate(dtStep);
            else                            _rawProgress += dtStep * StaticScrollSpeed;
        }

        SlotProgress = Mathf.Repeat(_rawProgress, 1f);
        if (SlotMaterial != null)
            SlotMaterial.SetFloat("_Offset", SlotProgress);
    }

    /* ---------- 模式分支 ---------- */
    private void ResultHoldUpdate(float dt)
    {
        _resultTimer -= dt;
        if (_resultTimer <= 0f)
        {
            _showingResult = false;
            OnEnterNormal?.Invoke();
        }
    }

    private void TuningUpdate(float dt)
    {
        if (PrizeStartProgresses.Count == 0) return;

        int   n     = PrizeStartProgresses.Count;
        int   idx   = Mathf.Clamp(TunePrizeIndex, 0, n - 1);
        float mid   = PrizeMidPoint(idx);
        int   loops = Mathf.RoundToInt(_rawProgress - mid);
        _targetRaw  = loops + mid;

        AligningStep(dt);
    }

    private void RunningUpdate(float dt)
    {
        if (!_aligning)  InertiaPhase(dt);
        else             AligningStep(dt);
    }

    /* ---------- 运动阶段 ---------- */
    private void InertiaPhase(float dt)
    {
        _rawProgress += CurrentSpeed * dt;
        CurrentSpeed  = Mathf.Max(0f, CurrentSpeed - Friction * dt);

        if (CurrentSpeed <= StoppingSpeed) DecidePrizeAndAlign_Random();
    }
    
    /* ---------- NEW: 加权随机抽奖 ---------- */
    private int WeightedRandomIndex()
    {
        float total = 0f;
        foreach (var w in PrizeWeights) total += w;

        float r = Random.Range(0f, total);
        float acc = 0f;
        for (int i = 0; i < PrizeWeights.Count; ++i)
        {
            acc += PrizeWeights[i];
            if (r <= acc) return i;
        }
        return PrizeWeights.Count - 1;   // 理论到不了这里
    }

    /* ---------- MOD: 选奖并开始对齐 ---------- */
    private void DecidePrizeAndAlign_Random()
    {
        _prizeIndex = WeightedRandomIndex();          // ★ 随机抽奖

        float mid  = PrizeMidPoint(_prizeIndex);

        // 计算最近的整数圈数，保持滚动连贯
        int loops  = Mathf.RoundToInt(_rawProgress - mid);
        _targetRaw = loops + mid;

        _aligning        = true;
        _alignEntrySpeed = Mathf.Abs(CurrentSpeed);
    }

    /* ---------- 弹簧一步 (含可变阻尼) ---------- */
    private void AligningStep(float dt)
    {
        float delta   = _targetRaw - _rawProgress;

        /* ★ 根据当前速度调整阻尼 */
        float effectiveFriction = AligningFriction;
        if (_alignEntrySpeed > 0f && Mathf.Abs(CurrentSpeed) < 0.5f * _alignEntrySpeed)
            effectiveFriction *= 2f;

        float accel   = -AligningSpring * delta - effectiveFriction * CurrentSpeed;

        CurrentSpeed += accel * dt;
        _rawProgress += CurrentSpeed * dt;

        speedLow = Mathf.Abs(CurrentSpeed) < MinFinalizeSpeed;
        deltaLow = Mathf.Abs(delta)       < MinFinalizeDelta;

        if (speedLow && deltaLow)
        {
            _rawProgress = _targetRaw;
            CurrentSpeed = 0f;

            if (EnableTuningMode)
            {
                _aligning = false;
                return;
            }

            if (_aligning)
            {
                _aligning       = false;
                SlotRunning     = false;
                _showingResult  = true;
                _resultTimer    = ResultHoldSeconds;
                
                OnEnterShowingResult?.Invoke(SlotItems[_prizeIndex]);
                
                Debug.Log($"🎉 Prize: {_prizeIndex} ({SlotItems[_prizeIndex]})");
            }
        }
    }

    /* ---------- 工具 ---------- */
    private int FindPrizeIndex(float pos01)
    {
        int n = PrizeStartProgresses.Count;
        for (int i = 0; i < n - 1; ++i)
            if (pos01 >= PrizeStartProgresses[i] && pos01 < PrizeStartProgresses[i + 1])
                return i;
        return n - 1;
    }

    private float PrizeMidPoint(int idx)
    {
        int n = PrizeStartProgresses.Count;
        float s = PrizeStartProgresses[idx];
        float e = (idx == n - 1) ? PrizeStartProgresses[0] + 1f
                                 : PrizeStartProgresses[idx + 1];
        return Mathf.Repeat((s + e) * 0.5f, 1f);
    }
}

/* ---------- 自定义 Inspector ---------- */
#if UNITY_EDITOR
[CustomEditor(typeof(SlotMachine))]
public class SlotMachineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var sm = (SlotMachine)target;
        EditorGUILayout.Space(4);

        if (sm.EnableTuningMode)
        {
            EditorGUILayout.LabelField("Tuning Mode Active", EditorStyles.boldLabel);
            sm.TunePrizeIndex = EditorGUILayout.IntField("Tune Prize Index", sm.TunePrizeIndex);
        }
        else
        {
            if (GUILayout.Button("Start One Game"))
                sm.StartOneGame();
        }
    }
}
#endif
