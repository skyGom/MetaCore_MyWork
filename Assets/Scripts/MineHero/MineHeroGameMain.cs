using System;
using System.Collections.Generic;
using System.Linq;
using Cyan;
using Cysharp.Threading.Tasks;
using Dasverse.Aleo.System;
using Dasverse.Aleo.UI;
using Dasverse.Framework;
using DG.Tweening;
using minigame.MineHero;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Dasverse.Aleo
{
    public class MineHeroGameMain : MonoBehaviour, InitializerListener
    {
        public static MineHeroGameMain Instance;

        public bool IsIgnoreBomb => isIgnoreBomb;
        public int AbleEmpShockNum => ableEmpShockNum;
        public int AbleIgnoreBombNum => ableIgnoreBombNum;

        public float MaxGameTime;
        public float MaxFinishGameTime;

        public Button GameStartBtn;

        [SerializeField]
        private GameObject barrierCol;
        [SerializeField]
        private LoaderUI loaderUI;
        [SerializeField]
        private TextMeshProUGUI headerText;
        [SerializeField]
        private AleoSODataManager aleoSO;
        [SerializeField]
        private ViewEnvironment viewEnvironment;
        [SerializeField]
        private UniversalRendererData[] universalRendererDatas;
        [SerializeField]
        private ParticleSystem empShockEffect;

        [HideInInspector]
        public bool IsTest;
        [HideInInspector]
        public MineHeroView MineHeroUIMainView;
        [HideInInspector]
        public AlertPopup AlertPopup;
        [HideInInspector]
        public int RewardBonus;
        [HideInInspector]
        public int RoomID;
        [HideInInspector]
        public int EnemyTotalPoint;
        /// <summary>
        /// 0 = 정상 진행, 1 = 상대의 기권, 2 = 나의 기권
        /// </summary>
        [HideInInspector]
        public int GameStateNum;
        [HideInInspector]
        public bool IsFinishMode;
        [SerializeField]
        private Blade blade;
        [SerializeField]
        private GameObject testBlade;

        private bool isMultiMode;
        private string timeText;
        private float currentGameTime;
        private bool isBarrierOn;
        private List<int> gamePointList;
        private int getTierPoint;
        private int getRankPoint;
        private int ableEmpShockNum;
        private int ableIgnoreBombNum;
        private bool isIgnoreBomb;

        private bool isShowAlertPopup;

        private Blit blueBarrierRenderFeature;
        private Blit goldBarrierRenderFeature;

        private IDisposable battleStartSendDispoable;
        private IDisposable interstitialAdSubscription = null;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

#if ENABLE_DEV
                if (Spawner.Instance != null && Spawner.Instance.AutoPlayData.Values[(int)GameType.MineHero] > 0)
                {
                    Debug.LogFormat("Attemp Enable TestBlade ... {0}", Spawner.Instance.AutoPlayData.Values[(int)GameType.MineHero]);
                    testBlade.SetActive(true);
                    CircleCollider2D col = testBlade.GetComponent<CircleCollider2D>();
                    if (col != null)
                    {
                        col.radius = Spawner.Instance.AutoPlayData.Values[(int)GameType.MineHero];
                        Debug.LogFormat("Enable Test Blade : {0}", col.radius);
                    }
                    else
                    {
                        Debug.Log("Not Found TestBlade Collider ...");
                    }
                }
                else
                {
                    Debug.Log("Disabled TestBlade ...");
                }
#endif

                setResolution();

                if (AssetManager.Instance != null)
                {
                    GameStartBtn.gameObject.SetActive(false);
                    Destroy(aleoSO.gameObject);
                    Destroy(TestCrystalSpawner.Instance.gameObject);

                    AssetManager.Instance.TakeMineHeroPoolItems();
                    setMineHeroView(BattleServerManager.Instance.IsMultiMode);
                }
                else
                {
                    IsTest = true;
                    Application.targetFrameRate = 30;
                    AleoSODataManager.GetInstance();
                    Initializer.ISO = LanguageId.KR.ToString();
                    Initializer.Start(this);
                    loaderUI.Show(SceneId.MineHero);
                    AssetManager.Instance.StartLoadingSceneObjects(SceneId.MineHero);

                    GameStartBtn.onClick.AddListener(() => MainGameStartAsync(true, 3, 3).Forget());
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            gamePointList = new List<int>();
            List<ScriptableRendererFeature> rendererFeatures = universalRendererDatas[QualitySettings.GetQualityLevel()].rendererFeatures;

            blueBarrierRenderFeature = (Blit)rendererFeatures.Where(x => x.name == "BlitBarrierBlue").FirstOrDefault();
            goldBarrierRenderFeature = (Blit)rendererFeatures.Where(x => x.name == "BlitBarrierGold").FirstOrDefault();

            if (!IsTest)
            {
                MineHeroTimeManager.Instance.Init();
                MineHeroPointManager.Instance.Init(BattleServerManager.Instance.RoomId);
                CrystalSpawner.Instance.Init();
                battleStartSendDispoable = GameCore.OnSceneEnteredRp.Where(x => x == true)
                    .Subscribe(_ => BattleServerManager.Instance?.StartBattle(CrystalSpawner.Instance.SetEmergenceNumList()).Forget());
            }

#if UNITY_EDITOR    // 테스트용 키
            if (IsTest)
            {
                this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.Q))
                    .Subscribe(_ => GameEndSceneMove());

                this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.Alpha1))
                    .Subscribe(_ => BarrierActive());

                this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.Alpha2))
                    .Subscribe(_ => CrystalSpawner.Instance.SpacialSpawnStart());

                this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.Alpha3))
                    .Subscribe(_ => MineHeroPointManager.Instance.DoubleChace());

                this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.Alpha4))
                    .Subscribe(_ => CrystalSpawner.Instance.ReturnAllCrystalAsync().Forget());

                this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.Alpha5))
                    .Subscribe(_ => EmpShockItem());

                this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.Alpha6))
                    .Subscribe(_ => IgnoreBombItem());
            }
#endif
            blade.gameObject.SetActive(true);
            GameStateNum = 0;
            IsFinishMode = false;
            isShowAlertPopup = false;
        }

        public void CallbackByInteractionLimitDuration()
        {
        }

        public void OnCompletedInit()
        {
            ViewManager.CreateManager(AleoSODataManager.GetViewManagerData(UIViewManagerId.BaseView));
            PopupManager.CreateManager(AleoSODataManager.GetViewManagerData(UIViewManagerId.PopupView));
            setMineHeroView();

            MineHeroTimeManager.Instance.Init();
            MineHeroPointManager.Instance.Init();
            CrystalSpawner.Instance.Init();
            TestCrystalSpawner.Instance.Init();

            loaderUI.transform.gameObject.SetActive(false);
            GameStartBtn.transform.SetAsLastSibling();
            GameStartBtn.gameObject.SetActive(true);
        }

        private int deviceWidth, deviceHeight;
        private Rect rectCameraMain;
        private void setResolution()
        {
            int setWidth = 2048;
            int setHeight = 1024;

            deviceWidth = Screen.width;
            deviceHeight = Screen.height;
            rectCameraMain = Camera.main.rect;

            Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true);

            if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight)
            {
                float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight);
                Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);
            }
            else
            {
                float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight);
                Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);
            }
        }

        private void OnDestroy()
        {
            Screen.SetResolution(deviceWidth, deviceHeight, true);
            Camera.main.rect = rectCameraMain;
        }

        private void setMineHeroView(bool isMulti = false)
        {
            isMultiMode = isMulti;
            ViewEnvironmentInfo info =
                new ViewEnvironmentInfo(viewEnvironment.MiniGameWorldContainer,
                                        viewEnvironment.MiniGameUIContainer,
                                        viewEnvironment.MiniGamePopupContainer,
                                        null,
                                        Camera.main);
            ViewEnvironment.Instance.SetMiniGameInfo(info);
            ViewManager.Instance.SetContainer(ViewType.MiniGameView, false);
            MineHeroUIMainView = ViewManager.Instance.GetView<MineHeroView>(UIViews.MineHeroView);
            MineHeroUIMainView.InitializeSetting(isMulti);
            ViewManager.Instance.ShowView(UIViews.MineHeroView);

            if (isMulti)
                MineHeroUIMainView.SetEnemyNickname(BattleServerManager.Instance.EnemyNickName);

            MineHeroUIMainView.gameObject.SetActive(true);
            MineHeroUIMainView.DisplayReadyTween();

            PopupManager.Instance.SetContainer(ViewType.MiniGamePopup, false);
            AlertPopup = (AlertPopup)PopupManager.Instance.GetView(UIViews.AlertPopup);
        }

        public void SetLoadingProgress(float progress)
        {
            loaderUI.SetProgressBar(progress);
            progress = (float)Math.Truncate(progress * 100);
            loaderUI.SetLoadText($"{progress} %");
        }

        void Update()
        {
            if (MineHeroTimeManager.Instance.IsGameStart)
            {
                currentGameTime = MaxGameTime - (MineHeroTimeManager.Instance.TotalMilliSeconds / 1000f);

                if (currentGameTime < 0.1)
                    currentGameTime = 0;

                timeText = string.Format("{0:N1}", currentGameTime);
                MineHeroUIMainView.SetTimerText(timeText);
                MineHeroUIMainView.SetMyScore(MineHeroPointManager.Instance.TotalPoint);

                if (!IsTest && BattleServerManager.Instance.IsMultiMode)
                    MineHeroUIMainView.SetEnemyScore(EnemyTotalPoint);

                if (IsTest && currentGameTime <= 0)
                {
                    FinishModeStartAsync().Forget();
                }
            }

            if (IsFinishMode)
            {
                currentGameTime = MaxFinishGameTime - MineHeroTimeManager.Instance.FinishTime / 1000f;
                MineHeroUIMainView.SetTimerText("0");

#if UNITY_EDITOR
                // 테스트할 때 단일씬 실행용
                if (currentGameTime <= -3000 && IsTest)
                {
                    MainGameEndAsync().Forget();
                }
#endif
            }
        }

        public async UniTaskVoid MainGameStartAsync(bool isAvailable, int ableEmpNum, int ableIgnoreNum)
        {
            if (!isAvailable)
            {
                Debug.Log("출현 테이블 정보를 받지 못했습니다.");
                await UniTask.Yield();
                GameEndSceneMove();
                return;
            }

            if (IsTest)
                GameStartBtn.gameObject.SetActive(false);
            
            isIgnoreBomb = false;
            currentGameTime = MaxGameTime;
            ableEmpShockNum = ableEmpNum;
            ableIgnoreBombNum = ableIgnoreNum;
            MineHeroUIMainView.InitPayItemBtn(ableEmpShockNum, ableIgnoreBombNum);
            MineHeroTimeManager.Instance.GameStart();
            CrystalSpawner.Instance.StartSpawn();
            MineHeroUIMainView.DisplayStartTween();
            AudioManager<MineHeroBgm>.PlayAndFadeIn(MineHeroBgm.Minehero_Normalbg, 1f, true);
            AudioManager<MineHeroSFX>.Play(MineHeroSFX.Minehero_Start);
        }

        public void CancelAllEvents()
        {
            SpecialSoundAllStop();
            MineHeroUIMainView.HideItemUI();
            CrystalSpawner.Instance.TotalEmergenceStop();
            MineHeroPointManager.Instance.CancelDoubleChance();
            CancelBarrier();
        }

        public void UseItem(int itemId)
        {
            BattleItemRequest request = new BattleItemRequest();
            request.itemId = itemId;
            request.useCount = 1;

            BattleServerManager.Instance.SendItemRequest(request);
        }

        /// <summary>
        /// EMP 아이템 사용시 사운드 실행 및 현재 스폰된 크리스탈 모두 제거 점수 서버 전달
        /// </summary>
        public void EmpShockItem(){
            if(ableEmpShockNum == 0)
                return;
                
            ableEmpShockNum--;
            AudioManager<MineHeroSFX>.Play(MineHeroSFX.Minehero_emp);
            empShockEffect.Play();
            List<Crystal> crystalList = new List<Crystal>();
            crystalList = CrystalSpawner.Instance.GetSpawnedCrystalList();

            if(crystalList.Count == 0)
                return;

            MineHeroPointManager.Instance.SettlementPoints(crystalList).Forget();
            CrystalSpawner.Instance.ReturnCrystal(crystalList);

            foreach(var crystal in crystalList){
                if(crystal.CrystalID == 113){
                    BarrierActive();
                    continue;
                }

                if(crystal.CrystalID == 114){
                    MineHeroPointManager.Instance.DoubleChace();
                    continue;
                }

                if(crystal.CrystalID == 115){
                    CrystalSpawner.Instance.SpacialSpawnStart();
                    continue;
                }
            }
        }

        /// <summary>
        /// 폭탄 무시 아이템 사용시 폭탄 무시 상태로 변경
        /// </summary>
        public void IgnoreBombItem(){
            if(ableIgnoreBombNum == 0)
                return;

            ableIgnoreBombNum--;
            MineHeroTimeManager.Instance.IgnoreTimeStart();
            MineHeroTimeManager.Instance.IgnoreTimeRp.Where(x => x >= 5000).Subscribe(x => isIgnoreBomb = false);
            isIgnoreBomb = true;
        }

        /// <summary>
        /// 피니쉬 모드 실행
        /// </summary>
        /// <returns></returns>
        public async UniTaskVoid FinishModeStartAsync()
        {
            // 테스트할 때 단일씬 실행용(서버에 의한 중복실행 방지)
            if (IsFinishMode && IsTest)
                return;

            IsFinishMode = true;
            MineHeroUIMainView.EnableItemBtnInteraction(false);
            CancelAllEvents();

            headerText.text = $"<color=red>Finish Time!</color>";
            headerText.gameObject.SetActive(true);
            headerText.rectTransform.DOLocalMoveY(100, 1).From(true);

            AudioManager<MineHeroBgm>.StopAndFadeOut(MineHeroBgm.Minehero_Normalbg, 1f);

            await UniTask.Delay(TimeSpan.FromMilliseconds(1000));

            AudioManager<MineHeroBgm>.PlayAndFadeIn(MineHeroBgm.Minehero_Bonusbg, 1f, true);

            headerText.gameObject.SetActive(false);
            MineHeroTimeManager.Instance.FinishTimeStart();
            CrystalSpawner.Instance.StartFinishSpawn();
        }

        /// <summary>
        /// 상대방 기권했을 때 안내 팝업 띄움
        /// </summary>
        public void ShowGiveUpPopup()
        {
            isShowAlertPopup = true;
            GameStateNum = 1;
            IsFinishMode = false;
            CrystalSpawner.Instance.TotalEmergenceStop();
            MineHeroPointManager.Instance.HideBonusPointText();
            AudioManager<MineHeroBgm>.StopAndFadeOut(MineHeroBgm.Minehero_Bonusbg, 1f);
            MineHeroTimeManager.Instance.GameEnd();

            string giveUpText = ScriptDisplayUtil.GetText(LanguageScripts.MiniGame, ScriptConstants.MiniGame.OpponentAbstained.ToString());
            AlertPopup.Instance.ShowAlertUI(giveUpText, () =>
            {
                isShowAlertPopup = false;
            }, null, AlertPopupButtonState.CancelHide);
        }

        /// <summary>
        /// 미니게임 결과값 세팅<para></para>
        /// singleResult => 0: gamePoint, 1: bonusPoint, 2: rewardBonus, 3: totalPoint, 4: singleReward<para></para>
        /// multiResult => 0: myPoint, 1: enemyPoint , 2:rankPoint, 3: multiReward, 4: win(0) / Draw(1) / Lose(2)
        /// </summary>
        /// <param name="res"></param>
        public void SetResponsePointList(minigame.BattleEndResponse res)
        {
            MUserInfo2 myInfo = res.userInfos.Where(x => x.sessionId == BattleServerManager.Instance.SessionId).FirstOrDefault();
            int winStatus = 0;
            getTierPoint = myInfo.getTierPoint;
            getRankPoint = myInfo.getRankPoint;

            if (!BattleServerManager.Instance.IsMultiMode)
            {
                gamePointList = new List<int>
                {
                    MineHeroPointManager.Instance.TotalHitPoint,
                    MineHeroPointManager.Instance.TotalBonus,
                    BattleServerManager.Instance.RewardCnt,
                    myInfo.getGamePoint,
                    (int)res.rewardItem.amount
                };
            }
            else
            {
                MUserInfo2 enemyInfo = res.userInfos.Where(x => x.sessionId != BattleServerManager.Instance.SessionId).FirstOrDefault();

                winStatus = myInfo.winStatus;
                if (enemyInfo != null)
                {
                    if (winStatus == 0)
                    {
                        if (myInfo.getRankPoint > enemyInfo.getRankPoint)
                        {
                            winStatus = 0;
                        }
                        else if (myInfo.getRankPoint < enemyInfo.getRankPoint)
                        {
                            winStatus = 2;
                        }
                        else
                        {
                            winStatus = 1;
                        }
                    }

                    gamePointList = new List<int>
                    {
                        myInfo.getGamePoint,
                        enemyInfo.getGamePoint,
                        myInfo.getRankPoint,
                        (int)res.rewardItem.amount,
                        winStatus,
                        (int)res.eventRewardItem.amount
                    };
                }
                else
                {
                    gamePointList = new List<int>
                    {
                        myInfo.getGamePoint,
                        0,
                        myInfo.getRankPoint,
                        (int)res.rewardItem.amount,
                        winStatus,
                        (int)res.eventRewardItem.amount
                    };
                }
            }
        }

        /// <summary>
        /// 미니게임 종료
        /// </summary>
        /// <param name="gameState">0: 정상종료, 1: 상대방 기권</param>
        /// <param name="isPacket"></param>
        /// <returns></returns>
        public async UniTaskVoid MainGameEndAsync(bool isPacket = true)
        {
#if UNITY_EDITOR
            // 테스트할 때 단일씬 실행용
            if (IsFinishMode == false && IsTest)
                return;
#endif
            IsFinishMode = false;
            CrystalSpawner.Instance.TotalEmergenceStop();
            MineHeroPointManager.Instance.HideBonusPointText();
            AudioManager<MineHeroBgm>.StopAndFadeOut(MineHeroBgm.Minehero_Bonusbg, 1f);
            MineHeroTimeManager.Instance.GameEnd();

            if (isPacket)
            {
                await UniTask.WaitUntil(() => gamePointList.Count >= 5 && !isShowAlertPopup);
            }
            else
            {
                gamePointList = MineHeroPointManager.Instance.GetPointList();
            }
            blade.gameObject.SetActive(false);
            // #731 전면 광고 노출 제거
            // if (DataManager.Instance.UserInfo.lastPurchasedTimeSec == 0)
            // {
            //     interstitialAdSubscription = GameCore.ADMonetizationManager.ShowInterstitialAd().Subscribe(step =>
            //     {
            //     }, exception =>
            //     {
            //         Debug.Log("MineHeroGameMain.MiniGameEndAsync() ShowInterstitialAd() exception : " + exception.Message);
            //         MineHeroUIMainView.SetResultData(gamePointList, GameStateNum);
            //     }, () =>
            //     {
            //         MineHeroUIMainView.SetResultData(gamePointList, GameStateNum);
            //     });
            // }
            // else
            {
                MineHeroUIMainView.SetResultData(gamePointList, GameStateNum);
            }
        }

        public void GameReset()
        {
            MineHeroTimeManager.Instance.GameRestartInit();
            CrystalSpawner.Instance.Init();
            MineHeroPointManager.Instance.Init();
            MineHeroUIMainView.InitializeSetting(false);
        }

        private async UniTaskVoid gameRestartAsync()
        {
            GameReset();

            await UniTask.Yield();

            MainGameStartAsync(true, 3, 3).Forget();
        }

        /// <summary>
        /// 게임 종료하고 원래 있던 씬으로 이동
        /// </summary>
        public void GameEndSceneMove()
        {
            Camera.main.rect = new Rect(0, 0, 1, 1);
            DisableBlueBarrierEffect();
            DisableGoldBarrierEffect();

            if (IsTest)
            {
                gameRestartAsync().Forget();
                return;
            }
            GameCore.SetLoadView(loaderUI);
            //AssetManager.Instance.TakeMineHeroPoolItems();
            loaderUI.Show();

            ViewEnvironment.Instance.SetActiveUIRoot(true);
            MineHeroUIMainView.Hide();

            AudioManager<MineHeroBgm>.StopAll();
            AudioManager<MineHeroSFX>.StopAll();

            BattleServerManager.Instance.IsSendBattleStart = false;

            battleStartSendDispoable.Dispose();
            if (interstitialAdSubscription != null)
            {
                interstitialAdSubscription.Dispose();
                interstitialAdSubscription = null;
            }

            Camera.main.rect = rectCameraMain;

            Destroy(MineHeroTimeManager.Instance);
            Destroy(MineHeroPointManager.Instance);
            Destroy(CrystalSpawner.Instance);
            Destroy(Instance);

            if (BattleServerManager.Instance.IsReservedKick)
            {
                AlertPopup.Instance.ShowAlertUI(ScriptDisplayUtil.GetText(LanguageScripts.Message, ScriptConstants.Message.SystemKickAlert.ToString()), GameCore.RestartApp, null, AlertPopupButtonState.CancelHide);
                return;
            }

            ViewManager.Instance.SetContainer(ViewType.MiniGameView, true);
            PopupManager.Instance.SetContainer(ViewType.MiniGamePopup, true);
            GameCore.StartLoadingScene(GameCore.PrevSceneId, false);

            //AudioManager<InGameBgm>.Play(InGameBgm.Aleoplaza_BGM, true);

            ViewManager.Instance.GetView(UIViews.InGameUIMainView).Show();

            // 게임결과 저장해서 이전 씬으로 돌아갔을 때 포인트 및 재화 획득 연출에 사용됨
            PlayRoomUIView playRoomUIView = (PlayRoomUIView)ViewManager.Instance.GetView(UIViews.PlayRoomUIView);
            playRoomUIView.SetGameResult(getTierPoint, getRankPoint, isMultiMode);

            //BattleServerManager.Instance.WebTcp2CancelRun();
        }

        private void OnApplicationQuit()
        {
            CancelAllEvents();
        }

        public void BarrierActive()
        {
            if (!isBarrierOn)
            {
                isBarrierOn = true;
                MineHeroTimeManager.Instance.BarrierTimeStart();
                MineHeroTimeManager.Instance.BarrierTimeRp.Where(x => x >= 5000).Subscribe(x => CancelBarrier());

                EnableBlueBarrierEffect();
                SpecialSoundAllStop();
                AudioManager<MineHeroBgm>.PlayAndFadeIn(MineHeroBgm.Minehero_Shieldbg, 0.5f, true);
            }
            else
            {
                CancelBarrier();
                BarrierActive();
            }
        }

        public void CancelBarrier()
        {
            if (isBarrierOn)
            {
                AudioManager<MineHeroBgm>.StopAndFadeOut(MineHeroBgm.Minehero_Shieldbg, 0.5f);
                barrierCol.SetActive(false);
                DisableBlueBarrierEffect();
                SpecialSoundAllStop();

                MineHeroTimeManager.Instance.BarrierTimeEnd();
                isBarrierOn = false;
            }
        }

        public void SpecialSoundAllStop()
        {
            AudioManager<MineHeroSFX>.StopAndFadeOut(MineHeroSFX.Minehero_emp, 0.5f);
            AudioManager<MineHeroBgm>.StopAndFadeOut(MineHeroBgm.Minehero_Shieldbg, 0.5f);
            AudioManager<MineHeroBgm>.StopAndFadeOut(MineHeroBgm.Minehero_x2bg, 0.5f);
            AudioManager<MineHeroBgm>.StopAndFadeOut(MineHeroBgm.Minehero_Feverbg, 0.5f);
        }

        public void EnableBlueBarrierEffect()
        {
            barrierCol.SetActive(true);
            MineHeroUIMainView.ShowShieldUI();
            blueBarrierRenderFeature.SetActive(true);
            blueBarrierRenderFeature.blitPass.blitMaterial.DOFloat(0.262f, "_FullscreenIntensity", 0.8f);
        }

        public void DisableBlueBarrierEffect()
        {
            barrierCol.SetActive(false);
            MineHeroUIMainView.HideShieldUI();
            blueBarrierRenderFeature.SetActive(false);
            blueBarrierRenderFeature.blitPass.blitMaterial.DOFloat(0f, "_FullscreenIntensity", 0.1f);
        }

        public void EnableGoldBarrierEffect()
        {
            MineHeroUIMainView.ShowGoldMineUI();
            goldBarrierRenderFeature.SetActive(true);
            goldBarrierRenderFeature.blitPass.blitMaterial.DOFloat(0.3f, "_FullscreenIntensity", 0.8f);
        }

        public void DisableGoldBarrierEffect()
        {
            MineHeroUIMainView.HideGoldMineUI();
            goldBarrierRenderFeature.SetActive(false);
            goldBarrierRenderFeature.blitPass.blitMaterial.DOFloat(0f, "_FullscreenIntensity", 0.1f);
        }
    }
}

