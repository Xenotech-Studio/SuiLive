using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DefaultNamespace;
using OpenBLive.Runtime.Data;
using SuiLive;
using UnityEngine;
using UnityEngine.UI;

public class InfoHelper : MonoBehaviour
{
    public RectTransform Guard1EnterRoomTemplate;
    public RectTransform Guard2EnterRoomTemplate;
    public RectTransform Guard3EnterRoomTemplate;
    public RectTransform NormalUserEnterRoomTemplate;
    public RectTransform ListContentParent;
    public Scrollbar VerticalScrollBar;
    public int counter;
    
    public bool Reverse { get => _reverse; set => _reverse = value; }
    public bool _reverse;
    private bool _reverseLastFrame;

    private Coroutine _scrollToBottomCoroutine;

    private void Awake()
    {
        Guard1EnterRoomTemplate.gameObject.SetActive(false);
        Guard2EnterRoomTemplate.gameObject.SetActive(false);
        Guard3EnterRoomTemplate.gameObject.SetActive(false);
        NormalUserEnterRoomTemplate.gameObject.SetActive(false);
    }

    private void Update()
    {
        ListContentParent.GetComponent<VerticalLayoutGroup>().reverseArrangement = Reverse;
        if (_reverse != _reverseLastFrame)
        {
            _reverseLastFrame = _reverse;
            StartCoroutine(ScrollToBottom(0.5f));
        }
    }

    // Start is called before the first frame update
    public void ReceiveDamaku(Dm dm)
    {
        /*
        if(gameObject.activeInHierarchy == false) gameObject.SetActive(true);
        
        RectTransform newItem = Instantiate(Template, ListContentParent);
        newItem.gameObject.SetActive(true);
        newItem.name = $"DamakuItem ({counter}) - {dm.userName} - {dm.timestamp}"; 
        counter += 1;
        
        TextFieldIndexing textFieldIndexing = newItem.GetComponentInChildren<TextFieldIndexing>();
        textFieldIndexing.TextFields["msg"].text = dm.msg;
        textFieldIndexing.TextFields["userName"].text = dm.userName;
        textFieldIndexing.TextFields["fansMedalLevel"].text = dm.fansMedalLevel.ToString();
        textFieldIndexing.TextFields["fansMedalName"].text = dm.fansMedalName;
        
        if (String.IsNullOrEmpty(dm.fansMedalName)) textFieldIndexing.TextFields["fansMedalName"].gameObject.SetActive(false);

        newItem.GetChild(1).GetComponent<ContentSizeFitter>().enabled = true;
        newItem.GetComponent<ContentSizeFitter>().enabled = true;
        
        if(_scrollToBottomCoroutine!=null) StopCoroutine(_scrollToBottomCoroutine);
        _scrollToBottomCoroutine = StartCoroutine(ScrollToBottom(0.5f));
        StartCoroutine(FadeIn(newItem.GetComponent<CanvasGroup>(), 0.5f));
        */
    }
    
    public void ReceiveGift(SendGift gift)
    {
        /*
        RectTransform newItem = Instantiate(Template, ListContentParent);
        newItem.gameObject.SetActive(true);
        newItem.name = $"DamakuItem ({counter}) - {gift.userName} - {gift.timestamp}"; 
        counter += 1;
        
        TextFieldIndexing textFieldIndexing = newItem.GetComponentInChildren<TextFieldIndexing>();
        textFieldIndexing.TextFields["msg"].text = "投喂了" + gift.giftNum + "个" + gift.giftName + "(" + gift.price/10 + "电池)";
        textFieldIndexing.TextFields["userName"].text = gift.userName;
        textFieldIndexing.TextFields["fansMedalLevel"].text = gift.fansMedalLevel.ToString();
        textFieldIndexing.TextFields["fansMedalName"].text = gift.fansMedalName;
        
        if (String.IsNullOrEmpty(gift.fansMedalName)) textFieldIndexing.TextFields["fansMedalName"].gameObject.SetActive(false);

        newItem.GetChild(1).GetComponent<ContentSizeFitter>().enabled = true;
        newItem.GetComponent<ContentSizeFitter>().enabled = true;
        
        if(_scrollToBottomCoroutine!=null) StopCoroutine(_scrollToBottomCoroutine);
        _scrollToBottomCoroutine = StartCoroutine(ScrollToBottom(0.5f));
        StartCoroutine(FadeIn(newItem.GetComponent<CanvasGroup>(), 0.5f));
        */
    }
    
    public void EnterRoom(EnterRoom er)
    {
        if (EnterRoomManager.CheckCoolDown2(er.uid, ConfigManager.Config.EnterRoomDrop.DropCoolDown))
        {
            int continueAttendance = EnterRoomManager.GetContinueAttendance(ConnectViaCode.Instance.RoomId, er.uid);
            int weekAttendance = EnterRoomManager.GetContinueAttendance(ConnectViaCode.Instance.RoomId, er.uid);

            RectTransform enterRoomTemplate = null;
            switch (er.guardLevel)
            {
                case 1:　enterRoomTemplate = Guard1EnterRoomTemplate;　break;
                case 2:　enterRoomTemplate = Guard2EnterRoomTemplate;　break;
                case 3:　enterRoomTemplate = Guard3EnterRoomTemplate;　break;
                default:　enterRoomTemplate = NormalUserEnterRoomTemplate;　break;
            }

            string guardLevelStr = "";
            switch (er.guardLevel)
            {
                case 1: guardLevelStr = "【总督】"; break;
                case 2: guardLevelStr = "【提督】"; break;
                case 3: guardLevelStr = "【舰长】"; break;
                default: guardLevelStr = ""; break;
            }
            
            
            if (er.guardLevel == 0 && er.wealthLevel < 25) return;
            
            RectTransform newItem = Instantiate(enterRoomTemplate, ListContentParent);
            newItem.gameObject.SetActive(true);
            newItem.name = $"[{counter}]进房消息 - {er.userName}";
            counter += 1;

            TextFieldIndexing textFieldIndexing = newItem.GetComponentInChildren<TextFieldIndexing>();
            StringBuilder sb = new StringBuilder();

            // 来啦：猪猪痴人, 
            // 粉团(太岁爷)13级, 财富20级
            // 周5天, 总114天, 航海100天(20天后到期)

            sb.Append($"进房：{er.userName} {guardLevelStr}\n");

            if (er.fansMedalLevel > 0) sb.Append($"粉团({er.fansMedalLevel})：{er.fansMedalLevel}, ");
            else sb.Append("无粉团牌, ");

            if (er.wealthLevel > 0) sb.Append($"财富等级：{er.wealthLevel}\n");
            else sb.Append("无财富等级\n");

            sb.Append($"周{weekAttendance}天, ");
            sb.Append($"总{continueAttendance}天, ");

            if (er.guardLevel > 0)
            {
                sb.Append($"航海{"X"}天");
                sb.Append($"({er.daysBeforeGuardExpired}天后到期)");
            }

            textFieldIndexing.TextFields["INFO"].text = sb.ToString();

            StartCoroutine(LoadAvatarAndContinue(er.userFace, newItem, () =>
            {
                if (_scrollToBottomCoroutine != null) StopCoroutine(_scrollToBottomCoroutine);
                _scrollToBottomCoroutine = StartCoroutine(ScrollToBottom(0.5f));
                StartCoroutine(FadeIn(newItem.GetComponent<CanvasGroup>(), 0.6f));
                StartCoroutine(FadeOut(newItem.GetComponent<CanvasGroup>(), 60f, 10f));
            }));
        }
    }

    IEnumerator LoadAvatarAndContinue(string userFace, RectTransform newItem, Action next)
    {
        // if has extraImageUrl, and if there is a gameObject with tag "AvatarImage" and has an Image component,
        // then set the Image component's sprite to the extraImageUrl.
        if (!string.IsNullOrEmpty(userFace))
        {
            //GameObject avatarImage = gift3D.FindChildWithTag("AvatarImage");
            GameObject avatarImage = null;
            foreach (Image i in newItem.GetComponentsInChildren<Image>())
            {
                if (i.CompareTag("WebImage"))
                {
                    avatarImage = i.gameObject;
                    break;
                }
            }
            
            if (avatarImage != null)
            {
                UnityEngine.UI.Image image = avatarImage.GetComponent<UnityEngine.UI.Image>();
                if (image != null)
                {
                    bool isDone = false;
                    float startTime = Time.time;
                    newItem.gameObject.SetActive(false);
                    avatarImage.name = userFace;
                    WebImageUtil.GetTexture2DByUrl(userFace, (tex) =>
                    {
                        if (tex != null)
                        {
                            Debug.Log("sprite got");
                            image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                        }
                        else
                        {
                            Debug.Log("sprite got null");
                        }
                        isDone = true;
                        newItem.gameObject.SetActive(true);
                        next?.Invoke();
                    });
                    yield return new WaitUntil(() => isDone||Time.time-startTime>100);
                }
            }
        }
    }
    
    
    
    IEnumerator ScrollToBottom(float time)
    {
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            VerticalScrollBar.value = Reverse ? 1f : 0f;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator FadeIn(CanvasGroup cg, float time)
    {
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            cg.alpha = Mathf.Lerp(0, 1, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    
    IEnumerator FadeOut(CanvasGroup cg, float delay, float time)
    {
        yield return new WaitForSeconds(delay);
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            cg.alpha = Mathf.Lerp(1, 0, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // destroy the gameObject
        Destroy(cg.gameObject);
    }
}
