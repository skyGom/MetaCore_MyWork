using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dasverse.Aleo.InGame;
using Dasverse.Aleo.System;
using Dasverse.Aleo.UI;
using Dasverse.Framework;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Dasverse.Aleo
{   
    /// <summary>
    /// 커스터마이징 테스트용 메인 클래스 에셋 매니저를 생성하고 로드를 시작한다.
    /// </summary>
    public class CustomizationTestMain : MonoBehaviour, ILoaderListener
    {   
        /// <summary>
        /// 에셋 매니저 로딩 확인용 UI
        /// </summary>
        [SerializeField]
        private GameObject loadingUI;
        /// <summary>
        /// 실사용 커스터마이징 UI
        /// </summary>
        [SerializeField]
        private CharacterSettingView characterSettingView;

        /// <summary>
        /// 에셋 매니저 로딩용
        /// </summary>
        private List<ILoader> loaderList;

        [SerializeField]
        private AleoAssetsInputs aleoAssetsInputs;

        private Dictionary<long, OtherPlayer> playerDic = new Dictionary<long, OtherPlayer>();
        private GameObject character;

        private MyPlayer myPlayer;

        void Awake()
        {   
            Application.targetFrameRate = 30;

            // Btn.gameObject.SetActive(false);
            // countText.gameObject.SetActive(false);

            AleoSODataManager.GetInstance();
            loadingUI.SetActive(true);
            loaderList = new List<ILoader>();

            AssetManager.Create(LoaderId.Asset, this);

            StartLoad();
        }

        public void RegisterLoader(ILoader loader)
        {
            loaderList.Add(loader);
        }

        public void StartLoad()
        {
            foreach (var loader in loaderList)
            {
                loader.StartLoading();
            }
        }

        public void OnLoadingProgress(float v)
        {

        }

        int createNum = 0;

        public void OnCompletedLoading(LoaderId id)
        {
            if (id == LoaderId.Asset)
            {
                loadingUI.SetActive(false);
                //characterLoad().Forget();
                //characterSettingView.InitCharacterSettingUI();

                // createMyPlayer(new MUserInfo()
                // {
                //     sessionId = 3,
                //     nickName = "testismine",
                //     charType = 6,
                //     pos = new MVec3() { x = 0, y = 0, z = 0 }
                // }, 0).Forget();

                // Btn.onClick.AddListener(GenerateOtherPlayer);
                // Btn.gameObject.SetActive(true);
                // countText.gameObject.SetActive(true);

                // this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.Alpha1)).Subscribe(_ =>
                // {
                //     createOtherPlayer(new MUserInfo()
                //     {
                //         sessionId = createNum,
                //         nickName = "test_" + createNum,
                //         charType = 7,
                //         pos = new MVec3() { x = createNum, y = 0, z = createNum }
                //     }).Forget();
                //     createNum++;
                // });
            }
        }

        // public TextMeshProUGUI countText;
        // public Button Btn;

        // void Update()
        // {
        //     countText.text = "Player Count : " + playerDic.Count;
        //     aleoAssetsInputs.LookInput(new Vector2(-10, 0));
        // }

        // public void GenerateOtherPlayer()
        // {   
        //     Debug.Log("GenerateOtherPlayer");
        //     createOtherPlayer(new MUserInfo()
        //     {
        //         sessionId = createNum,
        //         nickName = "test_" + createNum,
        //         charType = 7,
        //         pos = new MVec3() { x = createNum, y = 0, z = createNum }
        //     }).Forget();
        //     createNum++;
        // }

        // private async UniTaskVoid characterLoad(){
        //     character = await CustomizationManager.Instance.SetCharacter();
        // }

        // private async UniTaskVoid createOtherPlayer(MUserInfo user)
        // {
        //     Vector3 pos = new Vector3(user.pos.x, user.pos.y, user.pos.z);
        //     if (pos == Vector3.zero)
        //     {
        //         pos = AleoSODataManager.DictionaryData.SceneSpwanPosDic.GetValueOrDefault(SceneId.Lobby);
        //     }

        //     var playerObj = await AssetManager.Instance.GetWorldObjectPool(WorldObjectPoolId.OtherPlayer).GetObjAsync(null, pos);

        //     playerObj.name = user.nickName;
        //     playerObj.transform.parent = ViewEnvironment.Instance == null ?  transform.Find("World") : ViewEnvironment.Instance.WorldContainer;

        //     var player = playerObj.GetComponent<OtherPlayer>();
        //     player.Init(new PlayerInfo(user.sessionId, user.nickName));
        //     //player.ResetPlayerUI();

        //     // 이 부분에 데이터를 넣어주어야 함
        //     player.CustomizationOptions = new Dictionary<string, int>(){
        //             {"Body", 200},
        //             {"Head", 100},
        //             {"Hair", 106},
        //             {"Top", 200},
        //             {"Bottom", 201},
        //             {"Shoes", 100}
        //         };

        //     GameObject character = await CustomizationManager.Instance.SetCharacter(player);
        //     player.SetAnimator(character.GetComponent<Animator>());
        //     // 추후 데이터화 필요
        //     player.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        //     player.gameObject.SetActive(true);
        //     playerDic.Add(user.sessionId, player);
        // }

        // private async UniTaskVoid createMyPlayer(MUserInfo user, int SceneID)
        // {
        //     if (myPlayer == null)
        //     {
        //         var player = await AssetManager.Instance.GetWorldObjectPool(WorldObjectPoolId.MyPlayer).GetObjAsync();

        //         player.transform.parent = ViewEnvironment.Instance == null ?  transform.Find("World") : ViewEnvironment.Instance.WorldContainer;
        //         player.transform.position = AleoSODataManager.DictionaryData.SceneSpwanPosDic.GetValueOrDefault((SceneId)SceneID);
        //         player.transform.rotation = Quaternion.Euler(AleoSODataManager.DictionaryData.SceneSpwanRotDic.GetValueOrDefault((SceneId)SceneID));
        //         player.name = user.nickName;

        //         myPlayer = player.GetComponent<MyPlayer>();
        //         myPlayer.Init(new PlayerInfo(user.sessionId, user.nickName));
        //         myPlayer.SetCameraRot(AleoSODataManager.DictionaryData.SceneSpwanRotDic.GetValueOrDefault((SceneId)SceneID));

        //         // 이 부분에 데이터를 넣어주어야 함
        //         myPlayer.CustomizationOptions = new Dictionary<string, int>(){
        //             {"Body", 100},
        //             {"Head", 100},
        //             {"Hair", 100},
        //             {"Top", 101},
        //             {"Bottom", 102},
        //             {"Shoes", 100}
        //         };

        //         GameObject character = await CustomizationManager.Instance.SetCharacter(myPlayer);
        //         myPlayer.SetAnimator(character.GetComponent<Animator>());
        //         // 추후 데이터화 필요
        //         myPlayer.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        //         myPlayer.gameObject.SetActive(true);

        //         myPlayer.SetInputs(aleoAssetsInputs);
        //         //listener.OnSpawned(myPlayer);
        //     }
        //     else
        //     {
        //         if (GameCore.PrevSceneId == SceneId.MineHero)
        //         {
        //             myPlayer.transform.position = GameCore.PreMyPosition;
        //             myPlayer.SendStop();
        //         }
        //         else
        //         {
        //             myPlayer.transform.position = AleoSODataManager.DictionaryData.SceneSpwanPosDic.GetValueOrDefault((SceneId)SceneID);
        //             myPlayer.transform.rotation = Quaternion.Euler(AleoSODataManager.DictionaryData.SceneSpwanRotDic.GetValueOrDefault((SceneId)SceneID));
        //         }
        //         myPlayer.SetCameraRot(AleoSODataManager.DictionaryData.SceneSpwanRotDic.GetValueOrDefault((SceneId)SceneID));
        //     }
        // }

        private T getCharacterId<T>(int type) where T : Enum
        {
            if (type == 1)
            {
                return (T)Enum.Parse(typeof(T), "CharacterMale");
            }
            else
            {
                return (T)Enum.Parse(typeof(T), "CharacterFemale");
            }
        }
    }
}
