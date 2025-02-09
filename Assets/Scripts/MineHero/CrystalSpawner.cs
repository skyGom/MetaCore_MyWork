using Cysharp.Threading.Tasks;
using Dasverse.Framework;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Dasverse.Aleo
{
    public class CrystalSpawner : MonoBehaviour
    {
        public static CrystalSpawner Instance;

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

        private readonly float criticalChance = 0.05f;

        [SerializeField]
        private PhaseTableDatas spawnTimeTableDatas;
        [SerializeField]
        private MultiPosLaunch multiLaunchPoints;
        [SerializeField]
        private RawImage penaltyEffectImg;

        private bool isSpawnStart;
        private List<Crystal> spawnedCrystalList;
        private List<MineHeroObjectId> crystalKeysList;
        private Dictionary<MineHeroObjectId, GameObjectPool> crystalPoolDic;
        private GameObjectPool crystalEffectPool;
        private GameObjectPool trapEffectPool;

        /// <summary>
        /// 0 = Timelines, 1 = ObjectNums, 2 = PositionNums, 3 = Powers
        /// </summary>
        private List<List<int>> sortedSpawnList;
        private List<int> emergenceNumList;
        private EmergenceTableData specialEmergenceData;
        private ReactiveProperty<int> spawnCount;
        private ReactiveProperty<int> specialSpawnCount;
        private Color invisibleColor;
        private bool isSpecialSpawn;

        private CrystalPropertyRawContainer crystalPropertyContainer;
        private bool isFinishedSpawn;

        public void Init()
        {
            isSpawnStart = false;
            isSpecialSpawn = false;
            emergenceNumList = new List<int>();

            DataTableManager<Datas>.GetData(Datas.CrystalPropertyRawContainer, out crystalPropertyContainer);

            if (crystalPoolDic == null && crystalKeysList == null)
            {
                crystalPoolDic = new Dictionary<MineHeroObjectId, GameObjectPool>();
                crystalKeysList = new List<MineHeroObjectId>();

                for (int i = 0; i < 19; i++)
                {
                    crystalKeysList.Add((MineHeroObjectId)i);
                    crystalPoolDic.Add(crystalKeysList[i], AssetManager.Instance.GetMineHeroObjectPool((MineHeroObjectId)i));

                    int length = crystalPoolDic[crystalKeysList[i]].ObjNum;

                    for (int j = 0; j < length; j++)
                    {
                        crystalPoolDic[crystalKeysList[i]].GetObj().GetComponent<Crystal>().SetCrystalProperty(crystalPropertyContainer.items[i]);
                    }

                    crystalPoolDic[crystalKeysList[i]].ReturnAll();
                }
            }
            else
            {
                if (spawnedCrystalList != null)
                    ReturnAllCrystal(false);

                for (int i = 0; i < 19; i++)
                {
                    crystalPoolDic[crystalKeysList[i]].ReturnAll();
                }
            }

            sortedSpawnList = new List<List<int>>();
            spawnedCrystalList = new List<Crystal>();
            spawnCount = new ReactiveProperty<int>();
            specialSpawnCount = new ReactiveProperty<int>();

            crystalEffectPool = AssetManager.Instance.GetMineHeroObjectPool(MineHeroObjectId.Effect_Gem);
            trapEffectPool = AssetManager.Instance.GetMineHeroObjectPool(MineHeroObjectId.Effect_Trap);

            penaltyEffectImg.gameObject.SetActive(false);
            invisibleColor = new Color(penaltyEffectImg.color.r, penaltyEffectImg.color.g, penaltyEffectImg.color.b, 0);
            penaltyEffectImg.color = invisibleColor;

            if (MineHeroGameMain.Instance.IsTest)
            {
                emergenceNumList = SetEmergenceNumList();
                sortNomalSpawnTimelineAsync().Forget();
            }
        }

        private void Update()
        {
            if (isSpawnStart)
            {
                if (!isFinishedSpawn && sortedSpawnList[spawnCount.Value][0] <= MineHeroTimeManager.Instance.TotalMilliSeconds)
                {
                    if (UnityEngine.Random.Range(0, 1f) <= criticalChance)
                    {
                        spawn(crystalPoolDic[crystalKeysList[sortedSpawnList[spawnCount.Value][1] - 1]].GetObj().GetComponent<Crystal>()
                            , sortedSpawnList[spawnCount.Value][2], sortedSpawnList[spawnCount.Value][3], true);
                    }
                    else
                    {
                        spawn(crystalPoolDic[crystalKeysList[sortedSpawnList[spawnCount.Value][1] - 1]].GetObj().GetComponent<Crystal>()
                            , sortedSpawnList[spawnCount.Value][2], sortedSpawnList[spawnCount.Value][3]);
                    }
                }
                else if (isFinishedSpawn && sortedSpawnList[spawnCount.Value][0] <= MineHeroTimeManager.Instance.FinishTime)
                {
                    spawn(crystalPoolDic[crystalKeysList[sortedSpawnList[spawnCount.Value][1] - 1]].GetObj().GetComponent<Crystal>()
                        , sortedSpawnList[spawnCount.Value][2], sortedSpawnList[spawnCount.Value][3]);
                }

                if (isSpecialSpawn && specialEmergenceData.Timelines[specialSpawnCount.Value] <= MineHeroTimeManager.Instance.SpecialTime)
                {
                    spawn(crystalPoolDic[crystalKeysList[specialEmergenceData.ObjectNums[specialSpawnCount.Value] - 1]].GetObj().GetComponent<Crystal>()
                                                , specialEmergenceData.PositionNums[specialSpawnCount.Value], specialEmergenceData.Powers[specialSpawnCount.Value], false, true);
                }
            }
        }

        public void TotalEmergenceStop()
        {
            EndSpawn();
            SpecialSpawnEnd();
        }

        /// <summary>
        /// 60초 부터 0초(총 12회) + 보너스 타임 10초?(총 2회)
        /// 1페이지 = 2회
        /// 2페이지 = 4회
        /// 3페이지 = 4회
        /// 4페이지 = 2회
        /// </summary>
        /// <returns></returns>
        public List<int> SetEmergenceNumList()
        {
            // 60 ~ 50초 2회
            for (int i = 0; i < 2; i++)
            {
                pickEmergenceNum(0);
            }

            // 50 ~ 31초 4회
            for (int j = 0; j < 4; j++)
            {
                pickEmergenceNum(1);
            }

            // 30 ~ 11초 4회
            for (int k = 0; k < 4; k++)
            {
                pickEmergenceNum(2);
            }

            // 10 ~ 0초 2회
            for (int l = 0; l < 2; l++)
            {
                pickEmergenceNum(3);
            }

            // 보너스 10초 2회(하지만 보너스는 5초 1회만 실행)
            for (int m = 0; m < 2; m++)
            {
                pickEmergenceNum(5);
            }

            return emergenceNumList;
        }

        private void pickEmergenceNum(int index)
        {
            int randomNum = UnityEngine.Random.Range(0, spawnTimeTableDatas.PhaseTables[index].EmergenceDatas.Length);
            emergenceNumList.Add(spawnTimeTableDatas.PhaseTables[index].EmergenceDatas[randomNum].GroupNum);
        }

        public bool GetEmergenceNumList(List<int> emergenceNumList)
        {
            bool isAvailableStart = emergenceNumList != null && emergenceNumList.Count > 0 ? true : false;

            this.emergenceNumList = new List<int>();
            this.emergenceNumList = emergenceNumList;

            sortNomalSpawnTimelineAsync().Forget();

            return isAvailableStart;
        }

        private async UniTask sortNomalSpawnTimelineAsync()
        {
            int phaseIndex = 0;

            for (int i = 0; i < emergenceNumList.Count; i++)
            {
                PhaseTableData phaseTableData = spawnTimeTableDatas.PhaseTables[phaseIndex];
                EmergenceTableData tableData = phaseTableData.EmergenceDatas.Where(x => x.GroupNum == emergenceNumList[i]).FirstOrDefault();

                if (tableData != null)
                {
                    int lastSortedTime = sortedSpawnList.Count != 0 ? (int)(Math.Ceiling(sortedSpawnList[^1][0] * 0.001) * 1000) : 0;

                    for (int j = 0; j < tableData.Timelines.Count; j++)
                    {
                        List<int> emergenceTable = new List<int>();

                        if (i == 0)
                        {
                            emergenceTable.Add(tableData.Timelines[j]);
                            emergenceTable.Add(tableData.ObjectNums[j]);
                            emergenceTable.Add(tableData.PositionNums[j]);
                            emergenceTable.Add(tableData.Powers[j]);

                            sortedSpawnList.Add(emergenceTable);
                        }
                        else if (i >= 1 && j == 0)
                        {
                            lastSortedTime += tableData.Timelines[j];

                            emergenceTable.Add(lastSortedTime);
                            emergenceTable.Add(tableData.ObjectNums[j]);
                            emergenceTable.Add(tableData.PositionNums[j]);
                            emergenceTable.Add(tableData.Powers[j]);

                            sortedSpawnList.Add(emergenceTable);
                        }
                        else if (i >= 1 && j >= 1)
                        {
                            lastSortedTime += tableData.Timelines[j] - tableData.Timelines[j - 1];

                            emergenceTable.Add(lastSortedTime);
                            emergenceTable.Add(tableData.ObjectNums[j]);
                            emergenceTable.Add(tableData.PositionNums[j]);
                            emergenceTable.Add(tableData.Powers[j]);

                            sortedSpawnList.Add(emergenceTable);
                        }
                    }

                    // emergenceNumList의 페이즈 정렬 기준에 따라 index값 변화
                    if (i == 1) // 0, 1
                    {
                        phaseIndex = 1;
                    }
                    else if (i == 5)    // 2, 3, 4, 5
                    {
                        phaseIndex = 2;
                    }
                    else if (i == 9)    // 6, 7, 8, 9
                    {
                        phaseIndex = 3;
                    }
                    else if (i == 11)   // 10, 11
                    {
                        break;
                    }
                }
            }

            await UniTask.Yield();
        }

        public async UniTaskVoid SpawnCountIndexingAsync(int currentTime)
        {
            int limitindex = spawnCount.Value;

            for (int i = sortedSpawnList.Count - 1; i >= limitindex; i--)
            {
                if (sortedSpawnList[i][0] < currentTime)
                    spawnCount.Value++;
            }

            if (spawnCount.Value >= sortedSpawnList.Count)
                spawnCount.Value = sortedSpawnList.Count;

            await UniTask.Yield();

            MineHeroTimeManager.Instance.IsGameStart = true;
            isSpawnStart = true;
        }

        public void StartSpawn()
        {
            spawnCount = new ReactiveProperty<int>();
            spawnCount.Where(x => x >= sortedSpawnList.Count).Subscribe(x => isSpawnStart = false);
            isSpawnStart = true;
        }

        public void EndSpawn()
        {
            isSpawnStart = false;
        }

        public void SpacialSpawnStart()
        {
            if (!isSpecialSpawn)
            {
                int randomNum = UnityEngine.Random.Range(0, spawnTimeTableDatas.PhaseTables[4].EmergenceDatas.Length);
                specialEmergenceData = spawnTimeTableDatas.PhaseTables[4].EmergenceDatas[randomNum];
                specialSpawnCount = new ReactiveProperty<int>();
                specialSpawnCount.Where(x => x >= specialEmergenceData.Timelines.Count).Subscribe(x => SpecialSpawnEnd());

                isSpecialSpawn = true;
                MineHeroTimeManager.Instance.SpecialTimeStart();

                MineHeroGameMain.Instance.EnableGoldBarrierEffect();
                MineHeroGameMain.Instance.SpecialSoundAllStop();
                AudioManager<MineHeroBgm>.PlayAndFadeIn(MineHeroBgm.Minehero_Feverbg, 0.5f, true);
            }
            else
            {
                SpecialSpawnEnd();
                SpacialSpawnStart();
            }
        }

        public void SpecialSpawnEnd()
        {
            Debug.Log("SpecialSpawnEnd");
            isSpecialSpawn = false;
            MineHeroTimeManager.Instance.SpecialTimeEnd();

            MineHeroGameMain.Instance.DisableGoldBarrierEffect();
            AudioManager<MineHeroBgm>.StopAndFadeOut(MineHeroBgm.Minehero_Feverbg, 0.5f);
        }

        public async UniTaskVoid SpecialSpawnCountIndexingAsync(int currentTime)
        {
            int limitindex = specialSpawnCount.Value;

            for (int i = specialEmergenceData.Timelines.Count - 1; i >= limitindex; i--)
            {
                if (specialEmergenceData.Timelines[i] < currentTime)
                    specialSpawnCount.Value++;
            }

            if (specialSpawnCount.Value >= specialEmergenceData.Timelines.Count)
            {
                specialSpawnCount.Value = specialEmergenceData.Timelines.Count;
                SpecialSpawnEnd();
            }
            else
            {
                await UniTask.Yield();

                MineHeroTimeManager.Instance.IsSpecialSpawn = true;
                isSpecialSpawn = true;
            }

        }

        public void StartFinishSpawn()
        {
            if (isFinishedSpawn)
                return;

            EmergenceTableData tableData = spawnTimeTableDatas.PhaseTables[5].EmergenceDatas.Where(x => x.GroupNum == emergenceNumList[^2]).FirstOrDefault();
            sortedSpawnList = new List<List<int>>();

            for (int j = 0; j < tableData.Timelines.Count; j++)
            {
                List<int> emergenceTable = new List<int>
                {
                    tableData.Timelines[j],
                    tableData.ObjectNums[j],
                    tableData.PositionNums[j],
                    tableData.Powers[j]
                };

                sortedSpawnList.Add(emergenceTable);
            }

            isFinishedSpawn = true;
            Debug.Log($"StartFinishSpawn || sortedSpawnList.Count = {sortedSpawnList.Count}");
            StartSpawn();
        }

        public void StartEmergenceDataTableTest(EmergenceTableData tableData)
        {
            sortedSpawnList = new List<List<int>>();

            for (int j = 0; j < tableData.Timelines.Count; j++)
            {
                List<int> emergenceTable = new List<int>
                {
                    tableData.Timelines[j],
                    tableData.ObjectNums[j],
                    tableData.PositionNums[j],
                    tableData.Powers[j]
                };

                sortedSpawnList.Add(emergenceTable);
            }

            spawnCount = new ReactiveProperty<int>();
            spawnCount.Where(x => x >= sortedSpawnList.Count).Subscribe(x =>
            {
                isSpawnStart = false;
                MineHeroTimeManager.Instance.GameEnd();
                MineHeroTimeManager.Instance.Init();
            });
            isSpawnStart = true;
            MineHeroTimeManager.Instance.GameStart();
        }

        private void spawn(Crystal Crystal, int positionNum, int power, bool isCritical = false, bool isSpecial = false)
        {
            multiLaunchPoints.SetMultiPosLaunch(Crystal.gameObject, positionNum - 1);
            Crystal.LaunchCrystal(power, isCritical);

            spawnedCrystalList.Add(Crystal);

            if (!isSpecial)
            {
                spawnCount.Value++;
            }
            else
            {
                specialSpawnCount.Value++;
            }
        }

        public void ReturnPool(Crystal Crystal, bool isExplosion = false)
        {
            if (spawnedCrystalList != null && spawnedCrystalList.Contains(Crystal))
                spawnedCrystalList.Remove(spawnedCrystalList.Where(x => x == Crystal).FirstOrDefault());

            if (crystalPoolDic != null && crystalPoolDic.ContainsKey(crystalKeysList[Crystal.CrystalID - 101]))
            {
                crystalPoolDic[crystalKeysList[Crystal.CrystalID - 101]].Return(Crystal.gameObject);
                Crystal.gameObject.SetActive(false);

                Rigidbody2D rb = Crystal.gameObject.GetComponent<Rigidbody2D>();
                rb.velocity = Vector3.zero;
                rb.angularVelocity = 0;
            }

            if (isExplosion)
                explosionEffectAsync(Crystal).Forget();
        }

        public async UniTaskVoid ReturnAllCrystalAsync()
        {
            MineHeroGameMain.Instance.SpecialSoundAllStop();
            penaltyEffectImg.gameObject.SetActive(true);
            penaltyEffectImg.DOFade(0.5f, 0.5f).SetEase(Ease.OutQuad);

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            AudioManager<MineHeroSFX>.Play(MineHeroSFX.Minehero_emp);

            penaltyEffectImg.gameObject.SetActive(false);
            penaltyEffectImg.color = invisibleColor;

            ReturnAllCrystal();
        }

        public void ReturnAllCrystal(bool isOnEffect = true)
        {
            var listCopy = spawnedCrystalList?.ToList();
            spawnedCrystalList = new List<Crystal>();

            if (listCopy != null)
            {
                ReturnCrystal(listCopy, isOnEffect);
            }
        }

        /// <summary>
        /// 스폰된 크리스탈 리스트 만을 반환 후 초기화
        /// </summary>
        /// <param name="list"></param>
        public List<Crystal> GetSpawnedCrystalList()
        {   
            var crystals = spawnedCrystalList?.Where(x => x.CrystalID != 116).ToList(); // 116번은 트랩 크리스탈 제외
            spawnedCrystalList = spawnedCrystalList?.Where(x => x.CrystalID == 116).ToList();

            return crystals;
        }
        
        /// <summary>
        /// 리스트의 크리스탈들을 풀에 반환
        /// </summary>
        /// <param name="crystalList">반환할 크리스탈 리스트</param>
        /// <param name="isOnEffect">폭발 이펙트 여부</param>
        public void ReturnCrystal(List<Crystal> crystalList, bool isOnEffect = true){
            foreach (var item in crystalList)
            {
                ReturnPool(item, isOnEffect);
            }
        }

        private async UniTaskVoid explosionEffectAsync(Crystal Crystal)
        {
            GameObject explosion;
            ParticleSystem effect;
            Vector3 explosionPos = Crystal.gameObject.transform.position;
            Quaternion rot = Crystal.gameObject.transform.rotation;

            if (Crystal.CrystalID != 116)
            {
                explosion = crystalEffectPool.GetObj(null, explosionPos);
            }
            else
            {
                explosion = trapEffectPool.GetObj(null, explosionPos);
            }

            explosion.transform.rotation = Quaternion.Euler(new Vector3(90, rot.y, rot.z));
            explosion.transform.localScale = new Vector3(Crystal.Size, Crystal.Size, Crystal.Size);
            explosion.SetActive(true);
            effect = explosion.GetComponent<ParticleSystem>();
            effect.Play();

            if (effect != null)
                await UniTask.WaitUntil(() => !effect.isPlaying);

            explosion.SetActive(false);
            effect.Stop();

            if (Crystal.CrystalID != 116)
            {
                crystalEffectPool.Return(explosion);
            }
            else
            {
                trapEffectPool.Return(explosion);
            }
        }
    }
}