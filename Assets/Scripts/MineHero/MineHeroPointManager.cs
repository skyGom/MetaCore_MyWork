// Licensed to the .NET Foundation under one or more agreements.

using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Dasverse.Framework;
using DG.Tweening;
using minigame;
using minigame.MineHero;
using TMPro;
using UniRx;
using UnityEngine;

namespace Dasverse.Aleo
{
    public class MineHeroPointManager : MonoBehaviour
    {
        public static MineHeroPointManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public int ClientHitPoint;
        public int ClientBonusPoint;

        [HideInInspector]
        public int TotalPoint;
        [HideInInspector]
        public int TotalHitPoint;
        [HideInInspector]
        public int TotalBonus;
        [HideInInspector]
        public bool IsDoubleChance;

        public float MaxComboTime;
        public RectTransform NomalPointTargetRect;
        public RectTransform BonusPointTargetRect;

        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private Canvas popupCanvas;
        [SerializeField]
        private Camera mineHeroCamera;

        [Header("FontMaterials")]
        [SerializeField]
        private Material blackMat;
        [SerializeField]
        private Material yellowMat;
        [SerializeField]
        private Material redMat;

        private GameObjectPool earnedPointTextPool;
        private GameObjectPool pointParticlePool;

        private int standardHitPoint = 1;
        private int criticalHitPoint = 10;

        private int multiComboCount;
        private int roomId;
        private float currentComboTime;
        private bool isShowBonus;
        private Vector3 preHitPosition;
        private Queue<int> hitQue;
        private TextMeshProUGUI bonusText;
        private Vector3 nomalPointTartgetPos;
        private Vector3 bonusPointTartgetPos;

        private ReactiveProperty<bool> isHitRp;

        public int ServerRecordTotalPoint;

        public void Init(int roomId = 0)
        {
            this.roomId = roomId;

            Vector2 nomalPointScreenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, NomalPointTargetRect.position);
            RectTransformUtility.ScreenPointToWorldPointInRectangle(NomalPointTargetRect, nomalPointScreenPos, canvas.worldCamera, out nomalPointTartgetPos);

            Vector2 bonusPointScreenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, BonusPointTargetRect.position);
            RectTransformUtility.ScreenPointToWorldPointInRectangle(BonusPointTargetRect, bonusPointScreenPos, canvas.worldCamera, out bonusPointTartgetPos);

            ClientHitPoint = 0;
            ClientBonusPoint = 0;
            TotalPoint = 0;
            currentComboTime = 0;
            TotalBonus = 0;

            hitQue = new Queue<int>();
            isHitRp = new ReactiveProperty<bool>();
            isHitRp.Subscribe(x =>
            {
                if (!x)
                {
                    int hitQueCount = hitQue.Count;

                    if (hitQueCount >= 3)
                    {
                        multiComboCount++;

                        int getPoint = hitQueCount * hitQueCount;

                        NomalPointUp(getPoint);
                        Debug.Log($"<color=green>Combo Point Up : {getPoint}, Current Time : {MineHeroTimeManager.Instance.TotalMilliSeconds}</color>");
                        //showEarnedPointTextAsync(preHitPosition, $"<b><size=45><color=#ffff00>COMBO POINT +{getPoint}</b></size></color>.").Forget();
                        showEarnedPointTextAsync(preHitPosition, $"<b><size=90>COMBO POINT +{getPoint}</b></size>", fontMaterialIndex: 1).Forget();

                        if (multiComboCount >= 2)
                        {
                            ClientHitPoint += getPoint;  // 멀티콤보 포인트는 2x 적용을 하지 않습니다.
                            Debug.Log($"<color=green>Multi Combo Point Up : {getPoint}, Current Time : {MineHeroTimeManager.Instance.TotalMilliSeconds}</color>");
                            //showEarnedPointTextAsync(Vector3.up * 2, $"<b><size=49><color=#29a856>MULTI COMBO +100%</b></size></color>.").Forget();
                            showEarnedPointTextAsync(Vector3.up * 2, $"<b><size=100>MULTI COMBO +100%</b></size>", fontMaterialIndex: 2).Forget();
                        }
                    }
                    else
                    {
                        multiComboCount = 0;
                    }

                    MineHeroGameMain.Instance.MineHeroUIMainView?.ShowHistory(GetHitHistoryList(hitQue), hitQueCount);
                    hitQue = new Queue<int>();
                }
            });

            bonusText?.gameObject.SetActive(false);
            bonusText = null;

            earnedPointTextPool ??= AssetManager.Instance.GetMineHeroObjectPool(MineHeroObjectId.EarnedPoint);
            earnedPointTextPool.ReturnAll();
            pointParticlePool ??= AssetManager.Instance.GetMineHeroObjectPool(MineHeroObjectId.PointTrail);
        }

        private void initComboProperty()
        {
            currentComboTime = 0;
            isHitRp.SetValueAndForceNotify(false);
        }

        private void Update()
        {
            if (isHitRp != null && isHitRp.Value)
            {
                if (MineHeroGameMain.Instance.IsFinishMode)
                {
                    initComboProperty();
                    return;
                }

                currentComboTime += Time.deltaTime;

                if (currentComboTime >= MaxComboTime)
                {
                    initComboProperty();
                    return;
                }
            }
        }

        private void sendPoint(int objectId, int objedctTableId, int combo, long time)
        {
            BattlePointRequest battlePointRequest = new BattlePointRequest();
            battlePointRequest.roomId = roomId;
            battlePointRequest.objectId = objectId;
            battlePointRequest.objectTableId = objedctTableId;
            battlePointRequest.combo = combo;
            battlePointRequest.time = time;

            BattleServerManager.Instance?.SendPointRequest(battlePointRequest);
        }

        public void NomalPointUp(int enarnedPoint)
        {
            if (!IsDoubleChance)
            {
                ClientHitPoint += enarnedPoint;
                Debug.Log($"<color=green>Point Up : {enarnedPoint}, Current Time : {MineHeroTimeManager.Instance.TotalMilliSeconds}</color>");
            }
            else
            {
                ClientHitPoint += enarnedPoint * 2;
                Debug.Log($"<color=green>Point Up : {enarnedPoint * 2}, Current Time : {MineHeroTimeManager.Instance.TotalMilliSeconds}</color>");
            }

            Debug.Log($"<color=red>ClientRecordTotalPoint : {TotalPoint}</color>");
        }

        public void BonusPointUp(int enarnedPoint)
        {
            ClientBonusPoint += enarnedPoint;
        }

        public void LostPoint(Vector3 pos, int objectTableId)
        {
            sendPoint(objectTableId, objectTableId, 0, MineHeroTimeManager.Instance.TotalMilliSeconds);

            isHitRp.SetValueAndForceNotify(false);

            currentComboTime = 0;
            ClientHitPoint -= 10;
            Debug.Log($"<color=green>Lost Point : {-10}, Current Time : {MineHeroTimeManager.Instance.TotalMilliSeconds}</color>");
            //showEarnedPointTextAsync(pos, $"<size=50><color=#ff0000>-{10}</size></color>").Forget();
            showEarnedPointTextAsync(pos, $"<size=105>-{10}</size>", fontMaterialIndex: 5).Forget();
            MineHeroGameMain.Instance.MineHeroUIMainView?.ShowHistory(objectTableId - 101, -10);

            multiComboCount = 0;
        }

        public void Hit(Transform transform, int objectID, int objectTableID, bool isCritical = false)
        {
            AudioManager<MineHeroSFX>.Play(MineHeroSFX.Minehero_Objectboom);

            if (currentComboTime == 0 && objectTableID <= 115)
            {
                isHitRp.SetValueAndForceNotify(true);
            }

            if (isHitRp.Value)
                hitQue.Enqueue(objectID - 101);

            preHitPosition = transform.position;

            int sendHitCount = hitQue.Count > 2 ? hitQue.Count : 0;

            switch (objectTableID, isCritical)
            {
                // 일반 점수
                case (int n, bool isCri) when n >= 101 && n <= 112 && isCri == false:
                    NomalPointUp(standardHitPoint);
                    sendPoint(objectID, objectTableID, sendHitCount, MineHeroTimeManager.Instance.TotalMilliSeconds);
                    //showEarnedPointTextAsync(preHitPosition, $"<size=36><color=#FFFFFF>+{standardHitPoint}</size></color>").Forget();
                    showEarnedPointTextAsync(preHitPosition, $"<size=90>+{standardHitPoint}</size>", fontMaterialIndex: 0).Forget();
                    getPointEffectAsync(transform, true).Forget();
                    break;
                // 일반 크리티컬 점수
                case (int n, bool isCri) when n >= 101 && n <= 112 && isCri == true:
                    NomalPointUp(criticalHitPoint);
                    sendPoint(objectID, objectTableID + 100, sendHitCount, MineHeroTimeManager.Instance.TotalMilliSeconds);
                    //showEarnedPointTextAsync(preHitPosition, $"<size=38><color=#ff0000>CRITICAL +{criticalHitPoint}</size></color>").Forget();
                    showEarnedPointTextAsync(preHitPosition, $"<size=90>CRITICAL +{criticalHitPoint}</size>", fontMaterialIndex: 3).Forget();
                    getPointEffectAsync(transform, true).Forget();
                    break;
                // 아이탬
                case (int n, bool isCri) when n >= 113 && n <= 115 && isCri == false:
                    Debug.Log($"<color=green>Item : {objectTableID}, Current Time : {MineHeroTimeManager.Instance.TotalMilliSeconds}</color>");
                    sendPoint(objectID, objectTableID, sendHitCount, MineHeroTimeManager.Instance.TotalMilliSeconds);
                    getPointEffectAsync(transform, true).Forget();
                    break;
                // 보너스 1 점수
                case (int n, bool isCri) when n == 117 && isCri == false:
                    BonusPointUp(1);
                    sendPoint(objectID, objectTableID, 0, MineHeroTimeManager.Instance.TotalMilliSeconds);
                    //showEarnedPointTextAsync(preHitPosition, $"<size=36><color=#FFFFFF>+{1}</size></color>").Forget();
                    showEarnedPointTextAsync(preHitPosition, $"<size=90>+{1}</size>", fontMaterialIndex: 4).Forget();
                    showTotalBonusPointAsync();
                    getPointEffectAsync(transform, false).Forget();
                    break;
                // 보너스 2 점수
                case (int n, bool isCri) when n == 118 && isCri == false:
                    BonusPointUp(2);
                    sendPoint(objectID, objectTableID, 0, MineHeroTimeManager.Instance.TotalMilliSeconds);
                    //showEarnedPointTextAsync(preHitPosition, $"<size=36><color=#FFFFFF>+{2}</size></color>").Forget();
                    showEarnedPointTextAsync(preHitPosition, $"<size=90>+{2}</size>", fontMaterialIndex: 4).Forget();
                    showTotalBonusPointAsync();
                    getPointEffectAsync(transform, false).Forget();
                    break;
                // 보너스 3 점수
                case (int n, bool isCri) when n == 119 && isCri == false:
                    BonusPointUp(3);
                    sendPoint(objectID, objectTableID, 0, MineHeroTimeManager.Instance.TotalMilliSeconds);
                    //showEarnedPointTextAsync(preHitPosition, $"<size=36><color=#FFFFFF>+{3}</size></color>").Forget();
                    showEarnedPointTextAsync(preHitPosition, $"<size=90>+{3}</size>", fontMaterialIndex: 4).Forget();
                    showTotalBonusPointAsync();
                    getPointEffectAsync(transform, false).Forget();
                    break;
            }
        }

        /// <summary>
        /// 이전의 콤보를 결산하고 부숴진 크리스탈들을 서버에 한번에 리퀘스트합니다.
        /// </summary>
        public async UniTaskVoid SettlementPoints(List<Crystal> crystalList)
        {
            initComboProperty();

            await UniTask.WaitUntil(() => hitQue.Count == 0);   // 콤보가 결산이 될때 까지 대기합니다.

            isHitRp.SetValueAndForceNotify(true);
            List<MObjectInfo> destroyeObjdectList = new List<MObjectInfo>();

            foreach (var crystal in crystalList)
            {
                if (isHitRp.Value)
                    hitQue.Enqueue(crystal.CrystalID - 101);

                preHitPosition = crystal.transform.position;

                switch (crystal.CrystalID, crystal.IsCritical)
                {
                    // 일반 점수
                    case (int n, bool isCri) when n >= 101 && n <= 112 && isCri == false:
                        NomalPointUp(standardHitPoint);
                        //showEarnedPointTextAsync(preHitPosition, $"<size=36><color=#FFFFFF>+{standardHitPoint}</size></color>").Forget();
                        showEarnedPointTextAsync(preHitPosition, $"<size=90>+{standardHitPoint}</size>", fontMaterialIndex: 0).Forget();
                        getPointEffectAsync(crystal.transform, true).Forget();
                        break;
                    // 일반 크리티컬 점수
                    case (int n, bool isCri) when n >= 101 && n <= 112 && isCri == true:
                        NomalPointUp(criticalHitPoint);
                        //showEarnedPointTextAsync(preHitPosition, $"<size=38><color=#ff0000>CRITICAL +{criticalHitPoint}</size></color>").Forget();
                        showEarnedPointTextAsync(preHitPosition, $"<size=95>CRITICAL +{criticalHitPoint}</size>", fontMaterialIndex: 3).Forget();
                        getPointEffectAsync(crystal.transform, true).Forget();
                        break;
                    // 아이탬
                    case (int n, bool isCri) when n >= 113 && n <= 115 && isCri == false:
                        Debug.Log($"<color=green>Item : {crystal.CrystalID}, Current Time : {MineHeroTimeManager.Instance.TotalMilliSeconds}</color>");
                        getPointEffectAsync(crystal.transform, true).Forget();
                        break;
                    // 보너스 1 점수
                    case (int n, bool isCri) when n == 117 && isCri == false:
                        BonusPointUp(1);
                        //showEarnedPointTextAsync(preHitPosition, $"<size=36><color=#FFFFFF>+{1}</size></color>").Forget();
                        showEarnedPointTextAsync(preHitPosition, $"<size=90>+{1}</size>", fontMaterialIndex: 4).Forget();
                        showTotalBonusPointAsync();
                        getPointEffectAsync(crystal.transform, false).Forget();
                        break;
                    // 보너스 2 점수
                    case (int n, bool isCri) when n == 118 && isCri == false:
                        BonusPointUp(2);
                        //showEarnedPointTextAsync(preHitPosition, $"<size=36><color=#FFFFFF>+{2}</size></color>").Forget();
                        showEarnedPointTextAsync(preHitPosition, $"<size=90>+{2}</size>", fontMaterialIndex: 4).Forget();
                        showTotalBonusPointAsync();
                        getPointEffectAsync(crystal.transform, false).Forget();
                        break;
                    // 보너스 3 점수
                    case (int n, bool isCri) when n == 119 && isCri == false:
                        BonusPointUp(3);
                        //showEarnedPointTextAsync(preHitPosition, $"<size=36><color=#FFFFFF>+{3}</size></color>").Forget();
                        showEarnedPointTextAsync(preHitPosition, $"<size=90>+{3}</size>", fontMaterialIndex: 4).Forget();
                        showTotalBonusPointAsync();
                        getPointEffectAsync(crystal.transform, false).Forget();
                        break;
                }

                if (destroyeObjdectList.Where(x => x.objectId == crystal.CrystalID).Count() == 0)
                {
                    destroyeObjdectList.Add(new MObjectInfo { objectId = crystal.CrystalID, count = 1 });
                }
                else
                {
                    destroyeObjdectList.Where(x => x.objectId == crystal.CrystalID).First().count++;
                }
            }

            BattleItemEmpRequest battleItemEmpRequest = new BattleItemEmpRequest
            {
                sessionId = roomId,
                destroyeObjdectList = destroyeObjdectList,
                nowTime = MineHeroTimeManager.Instance.TotalMilliSeconds
            };

            BattleServerManager.Instance?.SendEmpRequest(battleItemEmpRequest);
            initComboProperty();
        }

        /// <summary>
        /// 점수획득 텍스트 세팅
        /// </summary>
        /// <param name="position"></param>
        /// <param name="enarnedPointText"></param>
        /// <param name="isLost"></param>
        /// <param name="fontMaterialIndex">
        /// 0: 흰색 폰트 + 검은색 스트록(일반포인트 / 보너스포인트)
        /// 1: 노란색 폰트, 검은색 스트록(일반 콤보)
        /// 2: 보라색 폰트, 노란색 스트록(멀티 콤보)
        /// 3: 붉은색 폰트, 노란색 스트록(크리티컬)
        /// 4: 노란색 폰트, 검은색 스트록(보너스)
        /// 5: 흰색 폰트, 붉은색 스트록(폭탄)
        /// <returns></returns>
        private async UniTaskVoid showEarnedPointTextAsync(Vector3 position, string enarnedPointText, bool isLost = false, int fontMaterialIndex = 0)
        {
            GameObject textObj = GetTextObjectToPool(position);

            TextMeshProUGUI text = textObj.GetComponent<TextMeshProUGUI>();

            VertexGradient gradient = new VertexGradient();
            Color color;

            // 폰트 메테리얼 교체q
            switch (fontMaterialIndex)
            {
                default:
                case 0: // 흰색 폰트 + 검은색 스트록(일반포인트 / 보너스포인트)
                    text.fontMaterial = blackMat;
                    gradient.topLeft = Color.white;
                    gradient.topRight = Color.white;
                    gradient.bottomLeft = Color.white;
                    gradient.bottomRight = Color.white;
                    break;
                case 1: // 노란색 폰트, 검은색 스트록(일반 콤보)
                    text.fontMaterial = blackMat;
                    ColorUtility.TryParseHtmlString("#FF8800", out color);
                    gradient.topLeft = color;
                    gradient.bottomLeft = color;
                    ColorUtility.TryParseHtmlString("#D64B0F", out color);
                    gradient.topRight = color;
                    gradient.bottomRight = color;
                    break;
                case 2: //보라색 폰트, 노란색 스트록(멀티 콤보)
                    text.fontMaterial = yellowMat;
                    ColorUtility.TryParseHtmlString("#AD00FF", out color);
                    gradient.topLeft = color;
                    gradient.bottomLeft = color;
                    ColorUtility.TryParseHtmlString("#D60FC2", out color);
                    gradient.topRight = color;
                    gradient.bottomRight = color;
                    break;
                case 3: //붉은색 폰트, 노란색 스트록(크리티컬)
                    text.fontMaterial = yellowMat;
                    ColorUtility.TryParseHtmlString("#FF0000", out color);
                    gradient.topLeft = color;
                    gradient.bottomLeft = color;
                    ColorUtility.TryParseHtmlString("#A50E0E", out color);
                    gradient.topRight = color;
                    gradient.bottomRight = color;
                    break;
                case 4: //노란색 폰트, 검은색 스트록(보너스)
                    text.fontMaterial = blackMat;
                    ColorUtility.TryParseHtmlString("#FFB800", out color);
                    gradient.topLeft = color;
                    gradient.bottomLeft = color;
                    ColorUtility.TryParseHtmlString("#D64B0F", out color);
                    gradient.topRight = color;
                    gradient.bottomRight = color;
                    break;
                case 5: //흰색 폰트, 붉은색 스트록(폭탄)
                    text.fontMaterial = redMat;
                    ColorUtility.TryParseHtmlString("#FFFFFF", out color);
                    gradient.topLeft = color;
                    gradient.bottomLeft = color;
                    gradient.topRight = color;
                    gradient.bottomRight = color;
                    break;
            }
            text.colorGradient = gradient;


            if (!isLost)
            {
                text.rectTransform.DOLocalMoveY(-200, 2).From(true);
            }
            else
            {
                text.rectTransform.DOLocalMoveY(200, 2).From(true);
            }

            text.DOFade(0, 2f);
            text.text = $"{enarnedPointText}";
            textObj.SetActive(true);

            await UniTask.Delay(TimeSpan.FromMilliseconds(2000));

            textObj.SetActive(false);
            text.alpha = 1.0f;
            earnedPointTextPool.Return(textObj);
        }

        private void showTotalBonusPointAsync()
        {
            if (!isShowBonus)
            {
                isShowBonus = true;
                bonusText = GetTextObjectToPool(BonusPointTargetRect.transform.position, true).GetComponent<TextMeshProUGUI>();
                bonusText.alpha = 1.0f;
                bonusText.gameObject.SetActive(true);
            }

            bonusText.text = $"<b><size=75><color=red>BONUS {TotalBonus}</b></size></color>";
        }

        public void HideBonusPointText()
        {
            bonusText?.gameObject.SetActive(false);
        }

        private GameObject GetTextObjectToPool(Vector3 position, bool isBonus = false)
        {
            GameObject textObj = earnedPointTextPool.GetObj(popupCanvas.transform);
            Vector3 cameraPoint = mineHeroCamera.WorldToScreenPoint(position);
            Vector3 pos = !isBonus ? new Vector3(cameraPoint.x, cameraPoint.y + 200, cameraPoint.z) : cameraPoint;

            RectTransform rect = textObj.transform.GetComponent<RectTransform>();
            rect.position = pos;
            rect.rotation = Quaternion.identity;
            rect.localScale = Vector3.one;
            return textObj;
        }

        private async UniTaskVoid getPointEffectAsync(Transform objPos, bool isNomal)
        {
            GameObject particleObj = pointParticlePool.GetObj(null, objPos.position);
            particleObj.SetActive(true);

            if (isNomal)
            {
                particleObj.transform.DOMove(nomalPointTartgetPos, 0.5f).SetEase(Ease.InOutQuad);
            }
            else
            {
                particleObj.transform.DOMove(bonusPointTartgetPos, 0.5f).SetEase(Ease.InOutQuad);
            }

            ParticleSystem particle = particleObj.GetComponent<ParticleSystem>();
            particle.Play();

            await UniTask.Delay(TimeSpan.FromMilliseconds(500));

            particle.Stop();
            particleObj.SetActive(false);
            pointParticlePool.Return(particleObj);
        }

        public void DoubleChace()
        {
            if (!IsDoubleChance)
            {
                IsDoubleChance = true;
                MineHeroTimeManager.Instance.DoubleTimeStart();
                MineHeroTimeManager.Instance.DoubleTimeRp.Where(x => x >= 5000).Subscribe(x => CancelDoubleChance());

                MineHeroGameMain.Instance.MineHeroUIMainView.ShowDoubleUI();
                MineHeroGameMain.Instance.SpecialSoundAllStop();
                AudioManager<MineHeroBgm>.PlayAndFadeIn(MineHeroBgm.Minehero_x2bg, 0.5f, true);
            }
            else
            {
                CancelDoubleChance();
                DoubleChace();
            }
        }

        public void CancelDoubleChance()
        {
            if (IsDoubleChance)
            {
                MineHeroGameMain.Instance.MineHeroUIMainView.HideDoubleUI();
                AudioManager<MineHeroBgm>.StopAndFadeOut(MineHeroBgm.Minehero_x2bg, 0.5f);

                MineHeroTimeManager.Instance.DoubleTimeEnd();
                IsDoubleChance = false;
            }
        }

        public List<int> GetHitHistoryList(Queue<int> hitQue)
        {
            int index = 0;
            List<int> historyList = new List<int>();

            while (index < 3)
            {
                if (hitQue != null && hitQue.Count > 0)
                {
                    index++;
                    historyList.Add(hitQue.Dequeue());
                }
                else
                {
                    break;
                }
            }

            return historyList;
        }

        public List<int> GetPointList()
        {
            List<int> list = new List<int>
            {
                ClientHitPoint,
                TotalBonus,
                1,
                ClientHitPoint + ClientBonusPoint,
                0
            };

            return list;
        }
    }
}
