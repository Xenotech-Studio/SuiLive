using System.Collections;
using DataSystem;
using TMPro;
using UnityEngine;

namespace SuiLive
{
    public class CountdownTimer : MonoBehaviour
    {
        public TMP_Text countdownText;
        public TMP_Text millisText;
        public TMP_Text extraText;
        public TMP_Text extraMillisText;
        public TMP_Text buttonText;
        public float countdownSeconds = 10f;
        public float BasicSize = 1f;
        public float SizeSensitivity = 1f;
        public float PoseXSensitivity = 1f;
        public float PoseYSensitivity = 1f;
        public bool applyConfigOnStart = true;
        public bool autoDisableOnFinish = false;
        public Color flashColor = Color.yellow;
        public Color normalColor = Color.white;
        public float flashInterval = 0.15f;
        public int flashPairs = 2; // 黄-白 为一对，配置成对数。例如 2 对 = 黄-白-黄-白
        public string startLabel = "开始倒计时";
        public string resetLabel = "重置倒计时";
        public string cancelLabel = "取消";

        private Coroutine runningCoroutine;
        private float remainingSeconds;

        private void Awake()
        {
            if (countdownText == null)
            {
                countdownText = GetComponentInChildren<TMP_Text>(true);
            }
            if (millisText == null)
            {
                // 寻找除主文本外的另一个 TMP_Text 作为毫秒文本（若存在）
                TMP_Text[] all = GetComponentsInChildren<TMP_Text>(true);
                foreach (var t in all)
                {
                    if (t != countdownText)
                    {
                        millisText = t;
                        break;
                    }
                }
            }
        }

        private void Start()
        {
            if (applyConfigOnStart)
            {
                ApplyConfigValuesIfAvailable();
            }

            remainingSeconds = Mathf.Max(0f, countdownSeconds);
            UpdateText(countdownSeconds);
            UpdateButtonLabel();
        }

        private void Update()
        {
            ApplyVisualFromConfigEveryFrame();
            // 当未在计时中时，输入框（配置）修改总时长应实时反映到文本
            if (runningCoroutine == null)
            {
                ConfigData config = ConfigManager.SavedConfig;
                if (config != null && config.CountdownConfig != null)
                {
                    float newSeconds = config.CountdownConfig.CountdownSeconds;
                    if (!Mathf.Approximately(newSeconds, countdownSeconds))
                    {
                        countdownSeconds = Mathf.Max(0f, newSeconds);
                        UpdateText(countdownSeconds);
                    }
                }
            }
        }

        public void StartCountdown()
        {
            // 运行中：点击视为“取消”，停止并回满，不再继续走
            if (runningCoroutine != null)
            {
                StopCoroutine(runningCoroutine);
                runningCoroutine = null;
                remainingSeconds = countdownSeconds;
                UpdateText(remainingSeconds);
                UpdateButtonLabel();
                return;
            }

            // 未运行且已结束：点击视为“重置”，回满但不启动
            if (remainingSeconds <= 0f)
            {
                remainingSeconds = countdownSeconds;
                UpdateText(remainingSeconds);
                UpdateButtonLabel();
                return;
            }

            // 未运行且未结束：点击“开始计时”
            runningCoroutine = StartCoroutine(RunCountdown());
            UpdateButtonLabel();
        }

        public void StopCountdown()
        {
            if (runningCoroutine != null)
            {
                StopCoroutine(runningCoroutine);
                runningCoroutine = null;
            }
            UpdateButtonLabel();
        }

        public void ResetCountdown(float seconds)
        {
            // 停止当前计时并将剩余时间重置为新时长，等待再次开始
            StopCountdown();
            countdownSeconds = Mathf.Max(0f, seconds);
            remainingSeconds = countdownSeconds;
            UpdateText(countdownSeconds);
            UpdateButtonLabel();
        }

        private IEnumerator RunCountdown()
        {
            remainingSeconds = Mathf.Max(0f, countdownSeconds);
            while (remainingSeconds > 0f)
            {
                remainingSeconds -= Time.deltaTime;
                UpdateText(Mathf.Max(0f, remainingSeconds));
                yield return null;
            }

            runningCoroutine = null;
            OnCountdownFinished();
        }

        private void UpdateText(float seconds)
        {
            if (countdownText == null)
            {
                return;
            }

            if (seconds < 0f) seconds = 0f;

            int minutes = (int)(seconds / 60f);
            float secFraction = seconds - minutes * 60f;
            int secs = (int)secFraction;
            int millis = (int)((secFraction - secs) * 1000f + 0.5f);

            // 处理进位：例如 59.9995s 四舍五入为 60.000 → 进位到分钟
            if (millis >= 1000)
            {
                millis -= 1000;
                secs += 1;
            }
            if (secs >= 60)
            {
                secs -= 60;
                minutes += 1;
            }

            // 主文本：始终显示至少一位分钟与两位秒
            countdownText.text = $"{minutes}:{secs:00}";
            // 子文本：仅显示毫秒，带前导点，三位
            if (millisText != null)
            {
                millisText.text = $".{millis:000}";
            }

            // 额外显示（不控制其位置/尺寸），仅同步文本
            if (extraText != null)
            {
                extraText.text = $"{minutes}:{secs:00}";
            }
            if (extraMillisText != null)
            {
                extraMillisText.text = $".{millis:000}";
            }
        }

        private void OnCountdownFinished()
        {
            UpdateText(0f);
            if (countdownText != null)
            {
                StartCoroutine(FlashThenFinish());
            }
            else
            {
                if (autoDisableOnFinish)
                {
                    gameObject.SetActive(false);
                }
                UpdateButtonLabel();
            }
        }

        private IEnumerator FlashThenFinish()
        {
            // 按成对次数进行 黄-白 交替闪烁：每对包含两步（黄、白）
            int pairs = Mathf.Max(0, flashPairs);
            int totalSteps = pairs * 2;
            for (int i = 0; i < totalSteps; i++)
            {
                Color c = (i % 2 == 0) ? flashColor : normalColor;
                countdownText.color = c;
                if (millisText != null) millisText.color = c;
                yield return new WaitForSeconds(flashInterval);
            }

            // 恢复成常规颜色
            countdownText.color = normalColor;
            if (millisText != null) millisText.color = normalColor;

            if (autoDisableOnFinish)
            {
                gameObject.SetActive(false);
            }
            UpdateButtonLabel();
        }

        private void UpdateButtonLabel()
        {
            if (buttonText == null)
            {
                return;
            }
            if (runningCoroutine != null)
            {
                buttonText.text = cancelLabel; // 运行中显示“取消”
                return;
            }

            // 非运行中，若已结束则“重置倒计时”，否则“开始倒计时”
            buttonText.text = remainingSeconds <= 0f ? resetLabel : startLabel;
        }

        public void OnButtonClick()
        {
            // 运行中：点击即“取消”——停止并重置为满值
            if (runningCoroutine != null)
            {
                StopCountdown();
                remainingSeconds = countdownSeconds;
                UpdateText(remainingSeconds);
                UpdateButtonLabel();
                return;
            }

            // 未运行：若已结束则“重置倒计时”，否则“开始倒计时”
            if (remainingSeconds <= 0f)
            {
                ResetCountdown(countdownSeconds);
            }
            else
            {
                StartCountdown();
            }
        }

        private void ApplyConfigValuesIfAvailable()
        {
            // 读取配置（若存在）并应用到计时与显示的 Transform
            ConfigData config = ConfigManager.SavedConfig;
            if (config == null || config.CountdownConfig == null)
            {
                return;
            }

            countdownSeconds = config.CountdownConfig.CountdownSeconds;

            if (countdownText != null)
            {
                RectTransform textRect = countdownText.rectTransform;
                textRect.anchoredPosition = new Vector2(
                    config.CountdownConfig.CountdownPositionX * PoseXSensitivity,
                    config.CountdownConfig.CountdownPositionY * PoseYSensitivity
                );

                float size = Mathf.Max(0.01f, config.CountdownConfig.CountdownSize * SizeSensitivity + BasicSize);
                textRect.localScale = new Vector3(size, size, 1f);

                countdownText.gameObject.SetActive(config.CountdownConfig.CountdownEnabled);
            }
        }

        private void ApplyVisualFromConfigEveryFrame()
        {
            // 仅每帧同步 位置 / 尺寸 / 启用状态，不改动倒计时秒数
            if (countdownText == null)
            {
                return;
            }

            ConfigData config = ConfigManager.SavedConfig;
            if (config == null || config.CountdownConfig == null)
            {
                return;
            }

            RectTransform textRect = countdownText.rectTransform;
            textRect.anchoredPosition = new Vector2(
                config.CountdownConfig.CountdownPositionX * PoseXSensitivity,
                config.CountdownConfig.CountdownPositionY * PoseYSensitivity
            );

            float size = Mathf.Max(0.01f, config.CountdownConfig.CountdownSize * SizeSensitivity + BasicSize);
            textRect.localScale = new Vector3(size, size, 1f);

            if (countdownText.gameObject.activeSelf != config.CountdownConfig.CountdownEnabled)
            {
                countdownText.gameObject.SetActive(config.CountdownConfig.CountdownEnabled);
            }
        }
    }
}


