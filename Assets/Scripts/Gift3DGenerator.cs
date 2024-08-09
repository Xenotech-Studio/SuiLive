using System.Collections;
using System.Collections.Generic;
using EditorUtils;
using OpenBLive.Runtime;
using OpenBLive.Runtime.Data;
using UnityEngine;
using UnityEngine.SocialPlatforms;

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
    
    // Start is called before the first frame update
    void Start()
    {
    }

    public void EnterRoomDrop(EnterRoom enterRoom) => ReceiveGift(new SendGift { giftId = 31036, giftNum = 1 });

    public void ReceiveGift(SendGift gift)
    {
        // try to add gift.giftNum to _giftNums[gift.giftId]
        if (_giftNums.ContainsKey(gift.giftId)) _giftNums[gift.giftId] += gift.giftNum;
        else _giftNums[gift.giftId] = gift.giftNum;
        
        // if some coroutine is not running to generate the gift, run it.
        if (! _giftCoroutines.ContainsKey(gift.giftId)) StartCoroutine(ReceiveGiftCoroutine(gift.giftId));
    }

    public IEnumerator ReceiveGiftCoroutine(long giftId)
    {
        if (Gift3DPrefabs.ContainsKey(giftId))
        {
            while (_giftNums[giftId] > 0)
            {
                _giftNums[giftId] -= 1;
                
                GameObject gift3D = Instantiate(Gift3DPrefabs[giftId], this.transform);
                gift3D.transform.position = new Vector3(
                    x: GiftSource.transform.position.x + (PossibilityCurve.Evaluate(Random.Range(0f, 1f)) - 0.5f) * GiftStartPositionXRange * 100 ,
                    y: GiftSource.transform.position.y,
                    z: GiftSource.transform.position.z + (PossibilityCurve.Evaluate(Random.Range(0f, 1f)) - 0.5f) * 20);
                
                yield return new WaitForSeconds(GiftMinTimeGap);
            }
        }
        
        _giftCoroutines.Remove(giftId);
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
