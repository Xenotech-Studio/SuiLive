using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EditorUtils;
using OpenBLive._Demo.LoginSample.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Versee.Scripts.Utils;

public class SuiLiveLoginManager : MonoBehaviour
{

    [Header("记住身份码")]
    public string IsSaveCodeSaveKey = "IsSaveIdentity";
    public bool IsSaveIdCode;
    public Toggle SaveIdCodeToggle;

    [Header("预设身份")]
    public string SelectedIdentityKey = "SuiLiveSelectedIdentity";
    public string SelectedIdentity = "";
    
    [HideInInspector] [SerializeField]
    public SerializableDictionary<string, IdentityData> IdentityDataDic = new SerializableDictionary<string, IdentityData>();

    [Header("开启玩法")]
    public Button StartToPlayButton;

    [Header("Others")]
    public TMP_Dropdown IdentityDropdown;
    private int hideHash;
    public Animator AnimationController;

    [Header("Events")]
    public UnityEvent LinkSuccessEvent;//连接成功时触发
    public UnityEvent LinkFailedEvent;//连接失败时触发

    public virtual void Initial()
    {
        //config read and init
        IsSaveIdCode = BilibiliPlayerPrefs.GetBool(IsSaveCodeSaveKey);
        SaveIdCodeToggle.isOn = IsSaveIdCode;
        if (SaveIdCodeToggle)
        {
            SelectedIdentity = BilibiliPlayerPrefs.GetString(SelectedIdentityKey);
        }
        else
        {
            SelectedIdentity = IdentityDataDic.Keys.ToArray()[0];
            BilibiliPlayerPrefs.SetString(SelectedIdentityKey, SelectedIdentity);
        }
        // let dropdown select to the right index
        IdentityDropdown.value = IdentityDataDic.Keys.ToList().IndexOf(SelectedIdentity);

        //add ui listener
        IdentityDropdown.onValueChanged.AddListener(ChangeSelection);
        SaveIdCodeToggle.onValueChanged.AddListener(ChangeIsSaveIdCode);
        StartToPlayButton.onClick.AddListener(StartToPlay);
        
        //init aniamtion hash
        hideHash = Animator.StringToHash("Hide");


        //init openblive sdk
        if (ConnectViaCode.Instance != null)
        {
            ConnectViaCode.Instance.ConnectSuccess += LinkSuccess;
            ConnectViaCode.Instance.ConnectFailure += LinkFailed;
        }
    }

    /// <summary>
    /// 修改Id Code
    /// </summary>
    /// <param name="code"></param>
    public virtual void ChangeSelection(int selectedIndex)
    {
        SelectedIdentity = IdentityDataDic.Keys.ToArray()[selectedIndex];
        Debug.Log("Selected Identity is changed to " + SelectedIdentity);
    }

    /// <summary>
    /// 修改是否保存Id Code
    /// </summary>
    /// <param name="isOn"></param>
    public virtual void ChangeIsSaveIdCode(bool isOn)
    {
        IsSaveIdCode = isOn;
        BilibiliPlayerPrefs.SetBool(IsSaveCodeSaveKey, IsSaveIdCode);

        if (!IsSaveIdCode)
        {
            BilibiliPlayerPrefs.SetString(SelectedIdentityKey, string.Empty);
        }
    }

    /// <summary>
    /// 点击开启玩法时触发
    /// </summary>
    public virtual void StartToPlay()
    {
        if (ConnectViaCode.Instance)
        {
            ConnectViaCode.Instance.appId = IdentityDataDic[SelectedIdentity].AppId;
            ConnectViaCode.Instance.accessKeyId = IdentityDataDic[SelectedIdentity].AccessKeyId;
            ConnectViaCode.Instance.accessKeySecret = IdentityDataDic[SelectedIdentity].AccessKeySecret;
            ConnectViaCode.Instance.LinkStart(IdentityDataDic[SelectedIdentity].IdCode);
            
            ConnectViaCode.Instance.WebRoomLinkStart();
        }
    }

    /// <summary>
    /// 连接成功时触发
    /// </summary>
    protected virtual void LinkSuccess()
    {
        if (IsSaveIdCode)
        {
            BilibiliPlayerPrefs.SetString(SelectedIdentityKey, SelectedIdentity);
        }
        Debug.Log("连接成功");
        Hide();
        LinkSuccessEvent?.Invoke();
    }

    /// <summary>
    /// 连接失败时触发
    /// </summary>
    protected virtual void LinkFailed()
    {
        LinkFailedEvent?.Invoke();
        Debug.LogError("连接失败", this);
    }
    
    /// <summary>
    /// 关闭UI
    /// </summary>
    protected virtual void Hide()
    {
        if (AnimationController != null)
        {
            AnimationController.Play(hideHash);
        }
        Debug.Log("关闭身份码弹框");
    }

    #region Unity Func
    protected virtual void Start()
    {
        Initial();
    }
    #endregion

}


#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(SuiLiveLoginManager))]
public class SuiLiveLoginManagerEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        serializedObject.Update();
        DictionaryUtils.RenderDic("可选身份", serializedObject.FindProperty("IdentityDataDic"));
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
