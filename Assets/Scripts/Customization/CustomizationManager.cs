using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Dasverse.Aleo;
using Dasverse.Aleo.InGame;
using Dasverse.Framework;
using Rukha93.ModularAnimeCharacter.Customization.Utils;
using UniRx;
using UnityEngine;
using Debug = Dasverse.Debug;

public class CustomizationManager : MonoBehaviour
{   
    public static CustomizationManager Instance;

    /// <summary>
    /// 착용한 코스튬 정보를 담는 클래스
    /// </summary>
    public class EquipedItem{

        /// <summary>
        /// 착용한 코스튬의 인스턴스화된 오브젝트들, 한번에 한 여러 장비일 경우도 있으므로 리스트로 정의
        /// </summary>
        public List<GameObject> instantiatedObjects;
        public CostumeAssetData assetReference;

        public SkinnedMeshRenderer renderers;
    }

    [SerializeField]
    private CostumeAssetDataContainer costumeAssetDataContainer;

    private List<CostumeType> costumeTypeList = new List<CostumeType>
    {
        CostumeType.Body,
        CostumeType.Head,
        CostumeType.Hair,
        CostumeType.Top,
        CostumeType.Bottom,
        CostumeType.Shoes
    };

    /// <summary>
    /// 플레이어 케릭터
    /// </summary>
    private Animator character;

    /// <summary>
    /// 메인 적용될 케릭터의 바디 렌더러
    /// </summary>
    private SkinnedMeshRenderer referenceMesh;

    /// <summary>
    /// 플레이어의 바디파츠 테그 정보를 담는 변수
    /// </summary>
    private Dictionary<BodyPartType, BodyPartTag> bodyParts;

    /// <summary>
    /// 플레이어가 착용한 코스튬들의 정보를 담는 변수
    /// </summary>
    private Dictionary<CostumeType, EquipedItem> equiped;

    /// <summary>
    /// 남자 플레이어 디폴트 코스튬 옵션
    /// </summary>
    private Dictionary<string, int> defaultMaleCostumeOptions;
    /// <summary>
    /// 여자 플레이어 디폴트 코스튬 옵션
    /// </summary>
    private Dictionary<string, int> defaultFemaleCostumeOptions;

    public Dictionary<CostumeType, int> MyCostumeSeqDic { get; private set; }

    private RotateOnDrag rotateOnDrag;

    private LerpOnScroll lerpOnScroll;

    private bool isCharacterSetProcess = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        isCharacterSetProcess = false;
        bodyParts = new Dictionary<BodyPartType, BodyPartTag>();
        equiped = new Dictionary<CostumeType, EquipedItem>();
        rotateOnDrag = GetComponent<RotateOnDrag>();
        rotateOnDrag.enabled = false;
        lerpOnScroll = GetComponent<LerpOnScroll>();
        lerpOnScroll.enabled = false;
        
        defaultMaleCostumeOptions = new Dictionary<string, int>();
        foreach (var type in costumeTypeList)
        {
            if(type == CostumeType.Top){
                defaultMaleCostumeOptions.Add(type.ToString(), 102);
                continue;
            }

            if(type == CostumeType.Shoes){
                defaultMaleCostumeOptions.Add(type.ToString(), 104);
                continue;
            }

            defaultMaleCostumeOptions.Add(type.ToString(), 100);
        }

        defaultFemaleCostumeOptions = new Dictionary<string, int>();
        foreach (var type in costumeTypeList)
        {   
            if (type == CostumeType.Hair){
                defaultFemaleCostumeOptions.Add(type.ToString(), 107);
                continue;
            }

            if(type == CostumeType.Head){
                defaultFemaleCostumeOptions.Add(type.ToString(), 200);
                continue;
            }

            if(type == CostumeType.Top){
                defaultFemaleCostumeOptions.Add(type.ToString(), 202);
                continue;
            }

            if(type == CostumeType.Bottom){
                defaultFemaleCostumeOptions.Add(type.ToString(), 204);
                continue;
            }

            if(type == CostumeType.Shoes){
                defaultFemaleCostumeOptions.Add(type.ToString(), 104);
                continue;
            }

            defaultFemaleCostumeOptions.Add(type.ToString(), 100);
        }
    }

#region 캐릭터 셋팅뷰 카메라 관련 함수

    public void CamRotate(){
        rotateOnDrag.enabled = true;
        lerpOnScroll.enabled = true;
    }

    public void CamStop(){
        rotateOnDrag.enabled = false;
        lerpOnScroll.enabled = false;
    }

    public void DestroyCharacter(){
        if(character != null){
            Destroy(character.gameObject);
        }
    }
    #endregion

    /// <summary>
    /// 코스튬의 정보를 SOData기반으로 가져오기
    /// </summary>
    /// <param name="costumeType">필요한 파츠 타입</param>
    /// <param name="index">파츠의 index</param>
    /// <returns></returns>
    private CostumeAssetData getCostumeAssetData(CostumeType costumeType, int index, int gender)
    {
        ItemGroup itemGroup;
        // 헤어, 헤드, 신발은 같은 오브젝트 그룹을 사용 
        if (costumeType == CostumeType.Hair || costumeType == CostumeType.Shoes)
        {
            itemGroup = gender == 1 ? costumeAssetDataContainer.MaleItems : costumeAssetDataContainer.FemaleItems;
        }
        else
        {
            itemGroup = index < 200 ? costumeAssetDataContainer.MaleItems : costumeAssetDataContainer.FemaleItems;
        }

        switch (costumeType)
        {
            case CostumeType.Body:
                return itemGroup.Bodies.Where(x => x.ID == index).FirstOrDefault();
            case CostumeType.Head:
                return itemGroup.Heads.Where(x => x.ID == index).FirstOrDefault();
            case CostumeType.Hair:
                return itemGroup.HairStyles.Where(x => x.ID == index).FirstOrDefault();
            case CostumeType.Top:
                return itemGroup.Tops.Where(x => x.ID == index).FirstOrDefault();
            case CostumeType.Bottom:
                return itemGroup.Bottoms.Where(x => x.ID == index).FirstOrDefault();
            case CostumeType.Shoes:
                return itemGroup.Shoes.Where(x => x.ID == index).FirstOrDefault();
            default:
                return null;
        }
    }

    /// <summary>
    /// 코스튬 내장 리스트 가져오기
    /// </summary>
    /// <param name="costumeType">코스튬 타입</param>
    /// <param name="gender">0 = none, 1 = Male, 2 = Female</param>
    /// <returns></returns>
    public List<int> GetBuiltInCostumeList(CostumeType costumeType, int gender){
        ItemGroup itemGroup = gender == 1 ? costumeAssetDataContainer.MaleItems : costumeAssetDataContainer.FemaleItems;

        switch (costumeType)
        {   
            case CostumeType.Body:
                return itemGroup.Bodies.Where(x => x.CostumeServerData.Builtin == 1).Select(x => x.ID).ToList();
            case CostumeType.Head:
                return itemGroup.Heads.Where(x => x.CostumeServerData.Builtin == 1).Select(x => x.ID).ToList();
            case CostumeType.Hair:
                return itemGroup.HairStyles.Where(x => x.CostumeServerData.Builtin == 1).Select(x => x.ID).ToList();
            case CostumeType.Top:
                return itemGroup.Tops.Where(x => x.CostumeServerData.Builtin == 1).Select(x => x.ID).ToList();
            case CostumeType.Bottom:
                return itemGroup.Bottoms.Where(x => x.CostumeServerData.Builtin == 1).Select(x => x.ID).ToList();
            case CostumeType.Shoes:
                return itemGroup.Shoes.Where(x => x.CostumeServerData.Builtin == 1).Select(x => x.ID).ToList();
            default:
                return null;
        }
    }

    /// <summary>
    /// 코스튬 서버 시퀀스로 코스튬의 클라이언트상의 아이디 가져오기
    /// </summary>a
    /// <param name="costumeType">코스튬 타입</param>
    /// <param name="gender">0 = none, 1 = Male, 2 = Female</param>
    /// <param name="serverSeq">서버상의 아이탬 데이터</param>
    /// <returns></returns>
    public int GetCostumeID(CostumeType costumeType, int gender, int serverSeq)
    {
        ItemGroup itemGroup = gender == 1 ? costumeAssetDataContainer.MaleItems : costumeAssetDataContainer.FemaleItems;

        switch (costumeType)
        {
            case CostumeType.Body:
                return itemGroup.Bodies.Where(x => x.CostumeServerData.Sep == serverSeq).FirstOrDefault().ID;
            case CostumeType.Head:
                return itemGroup.Heads.Where(x => x.CostumeServerData.Sep == serverSeq).FirstOrDefault().ID;
            case CostumeType.Hair:
                return itemGroup.HairStyles.Where(x => x.CostumeServerData.Sep == serverSeq).FirstOrDefault().ID;
            case CostumeType.Top:
                return itemGroup.Tops.Where(x => x.CostumeServerData.Sep == serverSeq).FirstOrDefault().ID;
            case CostumeType.Bottom:
                return itemGroup.Bottoms.Where(x => x.CostumeServerData.Sep == serverSeq).FirstOrDefault().ID;
            case CostumeType.Shoes:
                return itemGroup.Shoes.Where(x => x.CostumeServerData.Sep == serverSeq).FirstOrDefault().ID;
            default:
                return 0;
        }
    }

    /// <summary>
    /// 서버상의 코스튬 아이디로 클라이언트 상의 아이디 가져오기
    /// </summary>
    /// <param name="equipment">서버 데이터</param>
    /// <param name="gender">0 = none, 1 = Male, 2 = Female</param>
    /// <returns></returns>
    private Dictionary<string, int> getPlayerCostumeOptions(Equipment equipment, int gender)
    {   
        Dictionary<string, int> options = new Dictionary<string, int>();

        options.Add(CostumeType.Body.ToString(), GetCostumeID(CostumeType.Body, gender, equipment.Skin));
        options.Add(CostumeType.Head.ToString(), GetCostumeID(CostumeType.Head, gender, equipment.Head));
        options.Add(CostumeType.Hair.ToString(), GetCostumeID(CostumeType.Hair, gender, equipment.Hair));
        options.Add(CostumeType.Top.ToString(), GetCostumeID(CostumeType.Top, gender, equipment.Top));
        options.Add(CostumeType.Bottom.ToString(), GetCostumeID(CostumeType.Bottom, gender, equipment.Bottom));
        options.Add(CostumeType.Shoes.ToString(), GetCostumeID(CostumeType.Shoes, gender, equipment.Shoes));

        Debug.Log("@@@@@options : Body || " + options[CostumeType.Body.ToString()] + " Head || " + options[CostumeType.Head.ToString()] + " Hair || " + options[CostumeType.Hair.ToString()] + " Top || " + options[CostumeType.Top.ToString()] + " Bottom || " + options[CostumeType.Bottom.ToString()] + " Shoes || " + options[CostumeType.Shoes.ToString()]);
        return options;
    }

    /// <summary>
    /// 클라이언트 상의 아이디로 서버상의 코스튬 시퀀스 가져오기
    /// </summary>
    /// <param name="costumeType"></param>
    /// <param name="gender"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public int GetServerSeq(CostumeType costumeType, int gender, int id){
        ItemGroup itemGroup = gender == 1 ? costumeAssetDataContainer.MaleItems : costumeAssetDataContainer.FemaleItems;

        switch (costumeType)
        {
            case CostumeType.Body:
                return itemGroup.Bodies.Where(x => x.ID == id).FirstOrDefault().CostumeServerData.Sep;
            case CostumeType.Head:
                return itemGroup.Heads.Where(x => x.ID == id).FirstOrDefault().CostumeServerData.Sep;
            case CostumeType.Hair:
                return itemGroup.HairStyles.Where(x => x.ID == id).FirstOrDefault().CostumeServerData.Sep;
            case CostumeType.Top:
                return itemGroup.Tops.Where(x => x.ID == id).FirstOrDefault().CostumeServerData.Sep;
            case CostumeType.Bottom:
                return itemGroup.Bottoms.Where(x => x.ID == id).FirstOrDefault().CostumeServerData.Sep;
            case CostumeType.Shoes:
                return itemGroup.Shoes.Where(x => x.ID == id).FirstOrDefault().CostumeServerData.Sep;
            default:
                return 0;
        }
    }

    /// <summary>
    /// 최근 생성한 케릭터의 코스튬 시퀀스 딕셔너리 가져오기
    /// </summary>
    /// <returns></returns>
    public Dictionary<CostumeType, int> GetCurrentCostumeSeqDic(){

        Dictionary<CostumeType, int> seqDic = new Dictionary<CostumeType, int>();

        foreach(var item in equiped){
            seqDic[item.Key] = item.Value.assetReference.CostumeServerData.Sep;
        }

        return seqDic;
    }

    /// <summary>
    /// 플레이어 정보를 바탕으로 케릭터를 생성하는 비동기 함수
    /// </summary>
    /// <param name="player">유저 코스튬 정보를 받아 올수 있는 것이면 무엇이든 변경</param>
    public async UniTask<GameObject> SetCharacter(Player player = null)
    {
        await UniTask.WaitUntil(() => isCharacterSetProcess == false);
        isCharacterSetProcess = true;

        GameObject characterObj = null;

        // 먼저 캐릭터가 생성되어 있다면 삭제
        if (character != null)
        {
            character = null;
            equiped.Clear();
            bodyParts.Clear();
        }

        // 처음 캐릭터를 생성할 때
        if (player == null)
        {
            setBody((int)BodyPart.M_Body_001_white);
            foreach (var type in costumeTypeList)
            {
                if (type == CostumeType.Body)
                    continue;

                if (defaultMaleCostumeOptions.ContainsKey(type.ToString()))
                {
                    Equip(type, defaultMaleCostumeOptions[type.ToString()], 1);
                }
            }

            characterObj = character.gameObject;
        }
        else // 나 혹은 타인의 캐릭터를 생성할때
        {
            character = null;
            
            characterObj = await characterSetProcess(getPlayerCostumeOptions(player.Equipment, player.Gender));

            List<GameObject> _equiped = new List<GameObject>();
            _equiped = characterObj.GetComponentsInChildren<SkinnedMeshRenderer>().Select(x => x.gameObject).Where(x => x.activeSelf == true).ToList();

            await UniTask.Yield();

            player.MeshCombiner.CombineMesh(_equiped);

            character = characterObj.GetComponent<Animator>();
            player.SetAnimator(character);

            await UniTask.Yield();

            // 베이킹한 메쉬가 임의의 위치에 있는 현상을 방지하기 위해 위치를 초기화
            Transform bakedMeshTrasform = player.transform.Find("CombinedMesh").transform;
            bakedMeshTrasform.localPosition = Vector3.zero;
            bakedMeshTrasform.localRotation = Quaternion.identity;
            bakedMeshTrasform.GetChild(0).localPosition = Vector3.zero;
            bakedMeshTrasform.GetChild(0).localRotation = Quaternion.identity;

            characterObj.transform.parent = player.transform.Find("Body");
            characterObj.transform.localPosition = Vector3.zero;
            characterObj.transform.localRotation = Quaternion.identity;
            if(player.CharacterController != null){
                player.CharacterController.center = this.equiped.ContainsKey(CostumeType.Shoes) ? new Vector3(0, 0.86f, 0) : Vector3.zero;
            }
            referenceMesh = player.GetComponentInChildren<SkinnedMeshRenderer>();
        }

        isCharacterSetProcess = false;
        return characterObj;
    }

    public void ChangeOutfit(int gender, int index){
        ItemGroup itemGroup = gender == 1 ? costumeAssetDataContainer.MaleItems : costumeAssetDataContainer.FemaleItems;
        Vector3[] outfits = itemGroup.Outfits;
        List<int> outfitList = new List<int>(){(int)outfits[index].x, (int)outfits[index].y, (int)outfits[index].z};
        Equip(CostumeType.Top, outfitList[0], gender, true);
        Equip(CostumeType.Bottom, outfitList[1], gender, true);
        //Equip(CostumeType.Shoes, outfitList[2], gender);
    }

    /// <summary>
    /// 최근 생성한 플레이어 케릭터의 코스튬 시퀀스 딕셔너리 저장
    /// </summary>
    public void savePlayerCostumeSeqDic(){
        MyCostumeSeqDic = GetCurrentCostumeSeqDic();
    }

    /// <summary>
    /// 선택한 성별의 바디로 변경
    /// </summary>
    public void ChangeGenderBody(int index)
    {
        // 먼저 생성되어 있는 캐릭터 삭제
        if (character != null)
        {
            Destroy(character.gameObject);

            bodyParts.Clear();
        }

        // 장비되었던 부분의 바디 부분만 삭제
        equiped[CostumeType.Body] = null;
        setBody(index);
        int gender = index < 200 ? 1 : 2;
        Dictionary<string, int> defaultCostumeOptions = index < 200 ? defaultMaleCostumeOptions : defaultFemaleCostumeOptions;
        
        foreach (var type in costumeTypeList)
        {
            if (type == CostumeType.Body)
                continue;

            if (defaultCostumeOptions.ContainsKey(type.ToString()))
            {
                Equip(type, defaultCostumeOptions[type.ToString()], gender);
            }
        }
    }

    /// <summary>
    /// 플레이어 케릭터를 생성하는 비동기 함수
    /// </summary>
    /// <param name="player">유저 코스튬 정보를 받아 올수 있는 것이면 무엇이든 변경</param>
    /// <returns>메쉬 콤바인이 완료된 유저 케릭터</returns>
    private async UniTask<GameObject> characterSetProcess(Dictionary<string, int> keyValues)
    { 
        int gender = keyValues[CostumeType.Body.ToString()] < 200 ? 1 : 2;
        setBody(keyValues[CostumeType.Body.ToString()]);

        await UniTask.Yield();
        character.gameObject.SetActive(false);
        
        // 플레이어 케릭터의 코스튬 정보를 가져와서 코스튬을 입힌다.
        foreach (var type in costumeTypeList)
        {   
            if (type == CostumeType.Body)
                continue;

            Equip(type, keyValues[type.ToString()], gender, false);

            await UniTask.Yield();
        }
        
        UpdateBodyRenderers();
        return character.gameObject;
    }

    /// <summary>
    /// 일반 바디 생성
    /// </summary>
    /// <param name="prefab">생성 할 바디</param>
    private void setBody(int index)
    {   
        // 바디 프리팹을 인스턴스화
        character = Instantiate(AssetManager.Instance.GetBody(index), this.transform).GetComponent<Animator>();

        // 바디의 스킨드메쉬렌더러중 하나를 가져오기
        var meshes = character.GetComponentsInChildren<SkinnedMeshRenderer>();
        referenceMesh = meshes[meshes.Length / 2];

        // 바디의 바디파트 태그를 가져오기
        var _bodyParts = character.GetComponentsInChildren<BodyPartTag>();
        foreach (var part in _bodyParts)
        {
            bodyParts[part.Type] = part;
        }

        // 착용한 바디 코스튬 정보 초기화
        var equip = new EquipedItem()
        {
            assetReference = getCostumeAssetData(CostumeType.Body, index, index < 200 ? 1 : 2),
            instantiatedObjects = new List<GameObject>() { character.gameObject },
            renderers = referenceMesh
        };

        character.gameObject.SetActive(true);
        equiped[CostumeType.Body] = equip;
    }

    /// <summary>
    /// 스킨 변경의 경우 같은 바디에서 메테리얼만 변화가 생기니 바디의 메테리얼만 변경(일단 이걸로 해보고 문제가 생기면 바디를 새로 생성하는 방식으로 변경)
    /// </summary>
    /// <param name="bodyPartID">변경하고자 하는 바디 파츠 id</param>
    public void ChangeBodyMat(int bodyPartID){
        if(character == null || referenceMesh == null){
            Debug.LogError("캐릭터의 바디가 생성되지 않았습니다.");
            return;
        }

        var meshes = AssetManager.Instance.GetBody((int)bodyPartID).GetComponentsInChildren<SkinnedMeshRenderer>();
        referenceMesh.sharedMaterials = meshes[meshes.Length / 2].sharedMaterials;

        var charactorMeshes = character.GetComponentsInChildren<Transform>().Where(x => x.GetComponent<BodyPartTag>() != null).ToArray();
        foreach (var mesh in charactorMeshes)
        {
            mesh.GetComponent<SkinnedMeshRenderer>().sharedMaterials = referenceMesh.sharedMaterials;
        }
        // 헤드 부분도 해줘야함
        if(equiped.ContainsKey(CostumeType.Head)){
            var headMesh = equiped[CostumeType.Head].instantiatedObjects[0].GetComponent<SkinnedMeshRenderer>().materials;

            for(int i = 0; i < 2; i++){
                headMesh[i].SetColor("_Color_A_1", referenceMesh.sharedMaterials[0].GetColor("_Color_A_1"));
                headMesh[i].SetColor("_Color_A_2", referenceMesh.sharedMaterials[0].GetColor("_Color_A_2"));
            }
        }

        equiped[CostumeType.Body].assetReference = getCostumeAssetData(CostumeType.Body, bodyPartID, bodyPartID < 200 ? 1 : 2);
    }

    /// <summary>
    /// 필요한 바디 파츠만 활성화
    /// </summary>
    public void UpdateBodyRenderers()
    {
        List<BodyPartType> disabled = new List<BodyPartType>();

        //모든 장비한 파츠들의 데이터를 가져와서
        foreach (var equip in equiped.Values)
        {
            if (equip.assetReference == null){
                Debug.Log("@@@@@assetReference is null");
                continue;
            }

            foreach (var part in equip.assetReference.DisableBodyPartTypes){
                if (!disabled.Contains(part)){
                    disabled.Add(part);
                }
            }
        }

        // 설정한 키에 있다면 비활성화
        foreach (var part in bodyParts){
            part.Value.gameObject.SetActive(!disabled.Contains(part.Key));
        }
    }

    /// <summary>
    /// 코스튬을 입히는 함수
    /// </summary>
    /// <param name="costumeType">교체할 코스튬 타입</param>
    /// <param name="index">코스튬의 인덱스</param>
    /// <param name="updateRenderers">바디 파츠의 렌더러를 업데이트 할 것인지</param>
    public void Equip(CostumeType costumeType, int index,int gender, bool updateRenderers = true)
    {
        //이전 코스튬을 먼저 벗는다
        Unequip(costumeType, updateRenderers);

        // 코스튬 정보를 가져온다 성별 설정 맞춰야함
        CostumeAssetData asset = getCostumeAssetData(costumeType, index, gender);

        // 새로 착용할 코스튬을 정의하고
        EquipedItem equip = new EquipedItem()
        {
            assetReference = asset,
            instantiatedObjects = new List<GameObject>()
        };
        equiped[costumeType] = equip;

        // 매쉬를 새로 인스턴스화 하고 렌더러를 추가
        GameObject go = null;
        SkinnedMeshRenderer skinnedMesh = null;

        if(costumeType == CostumeType.Hair){
            // 헤어가 여러개 조합이 가능해서 기존의 에셋의 스크립트와 같은 다른 방법을 고려 해봐야 함(추후 장비도 가능)
            go = Instantiate(AssetManager.Instance.GetHairPart(index), character.GetBoneTransform(asset.TargetBone));
            equip.instantiatedObjects.Add(go);
            go.SetActive(true);
        }
        else{
            // 코스튬을 불러와서 붙이는 작업
            go = new GameObject(AssetManager.Instance.GetCostumeMesh(costumeType, index).name);
            go.transform.SetParent(character.transform, false);
            equiped[costumeType].instantiatedObjects.Add(go);

            skinnedMesh = go.AddComponent<SkinnedMeshRenderer>();
            skinnedMesh.rootBone = referenceMesh.rootBone;
            skinnedMesh.bones = referenceMesh.bones;
            skinnedMesh.bounds = referenceMesh.bounds;
            skinnedMesh.sharedMesh = AssetManager.Instance.GetCostumeMesh(costumeType, index).sharedMesh;
            skinnedMesh.sharedMaterials = AssetManager.Instance.GetCostumeMesh(costumeType, index).sharedMaterials;
            if(costumeType == CostumeType.Head && equiped.ContainsKey(CostumeType.Body)){
                for(int i= 0; i < 2 ; i++){
                    skinnedMesh.materials[i].SetColor("_Color_A_1", referenceMesh.sharedMaterials[0].GetColor("_Color_A_1"));
                    skinnedMesh.materials[i].SetColor("_Color_A_2", referenceMesh.sharedMaterials[0].GetColor("_Color_A_2"));
                }
            }

            equiped[costumeType].renderers = skinnedMesh;

            // 머리의 경우 바디의 색상을 따라가야함
        }

        // 필요한 바디 파츠만 활성화, 최초 플레이어 케릭터 생성이 아닌 일반적인 케릭터 생성 과정에서는 맨 마지막에만 한번 하면 됨
        if(updateRenderers)
            UpdateBodyRenderers();

        // 코스튬 파츠 변경 완료
    }

    /// <summary>
    /// 코스튬을 벗는 함수
    /// </summary>
    /// <param name="category">입혀져 있는 코스튬</param>
    /// <param name="updateRenderers">바디 파츠의 렌더러를 업데이트 할 것인지</param>
    public void Unequip(CostumeType category, bool updateRenderers = true)
    {
        if (equiped.ContainsKey(category) == false)
            return;

        var item = equiped[category];
        equiped.Remove(category);

        foreach (var go in item.instantiatedObjects)
            Destroy(go);

        if (updateRenderers)
            UpdateBodyRenderers();
    }

    public void SetCamCloseUp(){
        lerpOnScroll.CamPositionSetCloseUp();
    }

    public void SetCamFullBody(){
        lerpOnScroll.CamPositionSetDefault();
    }

    public float GetCamDistance(){
        return lerpOnScroll.GetDistanceNomal();
    }
}
