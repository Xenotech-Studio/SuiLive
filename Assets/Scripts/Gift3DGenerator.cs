using System.Collections;
using System.Collections.Generic;
using DataSystem;
using DefaultNamespace;
using EditorUtils;
using OpenBLive.Runtime;
using OpenBLive.Runtime.Data;
using SuiLive;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Gift3DGenerator : MonoBehaviour
{
    public Transform AvatarCollider;
    public Transform GiftSource;
    
    [Range(0.5f, 2)]
    public float GiftStartPositionXRange = 1f;
    public AnimationCurve PossibilityCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    
    [HideInInspector] [SerializeField]
    public Versee.Scripts.Utils.SerializableDictionary<long, GameObject> Gift3DPrefabs = new Versee.Scripts.Utils.SerializableDictionary<long, GameObject>();
    
    private Dictionary<long, Coroutine> _giftCoroutines = new Dictionary<long, Coroutine>();
    private Dictionary<long, long> _giftNums = new Dictionary<long, long>();
    public float GiftMinTimeGap = 0.1f;
    
    public float GiftExistanceTime = 10;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    public void EnterRoomDrop(EnterRoom enterRoom)
    {
        if (EnterRoomManager.CheckCoolDown(enterRoom.uid, ConfigManager.Config.EnterRoomDrop.DropCoolDown * 60f))
        {
            bool drop = enterRoom.guardLevel == 0 ? ConfigManager.Config.EnterRoomDrop.DropNormalUser :
                enterRoom.guardLevel == 3 ? ConfigManager.Config.EnterRoomDrop.DropGuard3 :
                enterRoom.guardLevel == 2 ? ConfigManager.Config.EnterRoomDrop.DropGuard2 :
                enterRoom.guardLevel == 1 && ConfigManager.Config.EnterRoomDrop.DropGuard1;
            
            int mul = enterRoom.guardLevel == 0 ? ConfigManager.Config.EnterRoomDrop.DropCountNormalUser :
                enterRoom.guardLevel == 3 ? ConfigManager.Config.EnterRoomDrop.DropCountGuard3 :
                enterRoom.guardLevel == 2 ? ConfigManager.Config.EnterRoomDrop.DropCountGuard2 :
                enterRoom.guardLevel == 1 ? ConfigManager.Config.EnterRoomDrop.DropCountGuard1 : 0;
            
            if (mul>0 && drop)
            {
                int count = EnterRoomManager.GetWeekAttendance(ConnectViaCode.Instance.RoomId, enterRoom.uid);
                Debug.Log($"进场掉落！UID:{enterRoom.uid}, 昵称:{enterRoom.userName}, 连续签到天数:{count}x航海等级乘数{mul}");
                ReceiveGift(new SendGift
                {
                    giftId = enterRoom.guardLevel == 0 ? 10004 :
                        enterRoom.guardLevel == 3 ? 10003 :
                        enterRoom.guardLevel == 2 ? 10002 :
                        enterRoom.guardLevel == 1 ? 10001 : 10000,
                    giftNum = count * mul,
                    extraImageUrl = enterRoom.userFace, userName = enterRoom.userName
                });
            }
        }
    }

    
    

    public void ReceiveGift(SendGift gift)
    {
        if (gift.giftNum==0) return;
        
        //if (gift.giftId <= 10000 || gift.giftId > 10004) return;
        
        // try to add gift.giftNum to _giftNums[gift.giftId]
        if (_giftNums.ContainsKey(gift.giftId)) _giftNums[gift.giftId] += gift.giftNum;
        else _giftNums[gift.giftId] = gift.giftNum;
        
        // if some coroutine is not running to generate the gift, run it.
        if (! _giftCoroutines.ContainsKey(gift.giftId) && gameObject.activeSelf) StartCoroutine(ReceiveGiftCoroutine(gift.giftId, gift.userName, gift.extraImageUrl));
    }

    public IEnumerator ReceiveGiftCoroutine(long giftId, string senderName, string extraImageUrl="")
    {
        if (Gift3DPrefabs.ContainsKey(giftId))
        {
            while (_giftNums[giftId] > 0)
            {
                _giftNums[giftId] -= 1;
                
                StartCoroutine(GenerateSingleGift(giftId, senderName, extraImageUrl));
                
                yield return new WaitForSeconds(GiftMinTimeGap);
            }
        }
        
        _giftCoroutines.Remove(giftId);
    }

    public IEnumerator GenerateSingleGift(long giftId, string senderName, string extraImageUrl="")
    {
        GameObject gift3D = Instantiate(Gift3DPrefabs[giftId], this.transform);
        gift3D.transform.position = new Vector3(
            x: GiftSource.transform.position.x + (PossibilityCurve.Evaluate(Random.Range(0f, 1f)) - 0.5f) * GiftStartPositionXRange * 100 ,
            y: GiftSource.transform.position.y,
            z: GiftSource.transform.position.z + (PossibilityCurve.Evaluate(Random.Range(0f, 1f)) - 0.5f) * 20);

        gift3D.name += " - " + senderName;
        
        StartCoroutine(DestorySingleGiftAfterSeconds(gift3D, GiftExistanceTime));
        
        // if has extraImageUrl, and if there is a gameObject with tag "AvatarImage" and has an Image component,
        // then set the Image component's sprite to the extraImageUrl.
        if (!string.IsNullOrEmpty(extraImageUrl))
        {
            //GameObject avatarImage = gift3D.FindChildWithTag("AvatarImage");
            GameObject avatarImage = null;
            foreach (Image i in gift3D.GetComponentsInChildren<Image>())
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
                    gift3D.gameObject.SetActive(false);
                    avatarImage.name = extraImageUrl;
                    WebImageUtil.GetTexture2DByUrl(extraImageUrl, (tex) =>
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
                        gift3D.gameObject.SetActive(true);
                    });
                    yield return new WaitUntil(() => isDone||Time.time-startTime>100);
                }
            }
        }
    }
    
    public IEnumerator DestorySingleGiftAfterSeconds(GameObject gift3D, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gift3D);
    }

    // Update is called once per frame
    void Update()
    {
        GiftSource.transform.position = new Vector3(
            x: AvatarCollider.transform.position.x,
            y: GiftSource.transform.position.y,
            z: AvatarCollider.transform.position.z);

        GiftSource.transform.localScale = new Vector3( GiftStartPositionXRange, 0.024f, 1 );
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(Gift3DGenerator))]
public class Gift3DGeneratorEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        serializedObject.Update();
        DictionaryUtils.RenderDic("礼物预制体", serializedObject.FindProperty("Gift3DPrefabs"), oneLine:true);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
