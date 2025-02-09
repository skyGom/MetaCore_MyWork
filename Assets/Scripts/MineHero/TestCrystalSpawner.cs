using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dasverse.Aleo
{
    public class TestCrystalSpawner : MonoBehaviour
    {
        public static TestCrystalSpawner Instance;

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

        /// <summary>
        /// 그룹번호
        /// </summary>
        [SerializeField]
        private int groupNum;
        /// <summary>
        /// 타임라인 0 ~ 5000
        /// </summary>
        [SerializeField]
        private List<int> timelines;
        /// <summary>
        /// 오브젝트 번호
        /// </summary>
        [SerializeField]
        private List<int> objectNums;
        /// <summary>
        /// 포지션 번호 3 ~ 11
        /// </summary>
        [SerializeField]
        private List<int> positionNums;
        /// <summary>
        /// 발사 힘 1 ~ 3
        /// </summary>
        [SerializeField]
        private List<int> powers;
        /// <summary>
        /// 랜덤생성할 테이블 내의 수
        /// </summary>
        [SerializeField]
        private int randomCount;

        [HideInInspector]
        public bool isDataChecked;
        [HideInInspector]
        public EmergenceTableData emergenceTableData;

        private bool isTestStart;
        private bool isGroupChecked;
        private bool isTimeChecked;
        private bool isObjectChecked;
        private bool isPositionChecked;
        private bool isPowerChecked;

        public void Init()
        {
            isTestStart = false;
            isDataChecked = false;
            isGroupChecked = false;
            isObjectChecked = false;
            isPositionChecked = false;
            isTimeChecked = false;
            isPowerChecked = false;
        }

        private void setEmergenceTableData()
        {
            emergenceTableData = new EmergenceTableData();

            emergenceTableData.GroupNum = groupNum;
            emergenceTableData.Timelines = timelines;
            emergenceTableData.ObjectNums = objectNums;
            emergenceTableData.PositionNums = positionNums;
            emergenceTableData.Powers = powers;
        }

        public void SpawnTestStart()
        {
            Debug.Log("Spawn Test Start");

            if (isTestStart)
            {
                Debug.Log("<color=red>이미 테스트가 시작되었습니다.</color>");
                return;
            }
            else
            {
                if (timelines.Count == 0 || objectNums.Count == 0 || positionNums.Count == 0 || powers.Count == 0)
                {
                    Debug.Log("<color=red>출현 테이블이 비어있습니다. 출현 테이블을 먼저 생성해주세요!</color>");
                    return;
                }
                else
                {
                    setEmergenceTableData();
                    CrystalSpawner.Instance.StartEmergenceDataTableTest(emergenceTableData);
                    MineHeroGameMain.Instance.GameStartBtn.gameObject.SetActive(false);
                }
            }
        }

        public void SpawnTestStop()
        {
            Debug.Log("Spawn Test Stop");

            if (!isTestStart)
            {
                Debug.Log("<color=red>테스트가 시작하지 않았습니다.</color>");
                MineHeroGameMain.Instance.CancelAllEvents();
                MineHeroGameMain.Instance.GameReset();
                MineHeroGameMain.Instance.GameStartBtn.gameObject.SetActive(true);
                return;
            }
            else
            {
                isTestStart = false;
                MineHeroGameMain.Instance.CancelAllEvents();
                MineHeroGameMain.Instance.GameReset();
                MineHeroGameMain.Instance.GameStartBtn.gameObject.SetActive(true);
            }
        }

        public void SetRandomEmergenceData()
        {
            Debug.Log("Set Random EmergenceData");

            timelines = generateNonOverlappingInt(0, 5000, randomCount);
            objectNums = generateOverlappingInt(1, 16, randomCount);
            positionNums = generateOverlappingInt(3, 11, randomCount);
            powers = generateOverlappingInt(1, 3, randomCount);

            timelines.Sort();
        }

        public void SetBonusEmergenceData()
        {
            Debug.Log("Set Bonus EmergenceData");

            timelines = generateNonOverlappingInt(0, 5000, randomCount);
            objectNums = generateOverlappingInt(17, 19, randomCount);
            positionNums = generateOverlappingInt(3, 11, randomCount);
            powers = generateOverlappingInt(1, 3, randomCount);

            timelines.Sort();
        }

        private List<int> generateNonOverlappingInt(int start, int end, int count)
        {
            List<int> result = new List<int>();
            HashSet<int> usedNumbers = new HashSet<int>();

            for (int i = 0; i < count; i++)
            {
                int newNumber = UnityEngine.Random.Range(start, end + 1);

                while (usedNumbers.Contains(newNumber))
                {
                    newNumber = UnityEngine.Random.Range(start, end + 1);
                }

                usedNumbers.Add(newNumber);
                result.Add(newNumber);
            }

            return result;
        }

        private List<int> generateOverlappingInt(int start, int end, int count)
        {
            List<int> result = new List<int>();

            for (int i = 0; i < count; i++)
            {
                int newNumber = UnityEngine.Random.Range(start, end + 1);
                result.Add(newNumber);
            }

            return result;
        }

        public void EmergenceDataCheck()
        {
            Debug.Log("EmergenceData Check");

            if (isDataChecked)
            {
                Debug.Log("이미 체크를 완료 하셨습니다. CreateEmergenceData를 눌러 데이터를 생성하거나 Reset해주세요");
                return;
            }
            else
            {
                if (groupNum < 1000 || groupNum > 4000)
                {
                    Debug.Log("<color=red>GroupNum를 확인해주세요!</color>");
                    return;
                }
                else
                {
                    isGroupChecked = true;
                }

                if (timelines.Where(x => x < 0 || x > 5000).ToList().Count > 0)
                {
                    Debug.Log("<color=red>TimeLines가 맞지 않습니다. 확인해주세요! 테이블당 기본 할당 할 수 있는 시간은 5초(5000ms) 이내입니다.</color>");
                    return;
                }
                else
                {
                    isTimeChecked = true;
                }

                if (objectNums.Where(x => x < 1 || x > 19).ToList().Count > 0)
                {
                    Debug.Log("<color=red>ObjectNums가 맞지 않습니다. 확인해주세요! 테이블에 정해진 번호에 맞는 정수를 입력해주세요.</color>");
                    return;
                }
                else
                {
                    isObjectChecked = true;
                }

                if (positionNums.Where(x => x < 3 || x > 11).ToList().Count > 0)
                {
                    Debug.Log("<color=red>PositionNums가 맞지 않습니다. 확인해주세요! 1번 2번은 기획상 제외된 출현 지점입니다. 3번 부터 11번 사이의 출현 지점 정수를 입력해주세요.</color>");
                    return;
                }
                else
                {
                    isPositionChecked = true;
                }

                if (powers.Where(x => x < 1 || x > 3).ToList().Count > 0)
                {
                    Debug.Log("<color=red>Powers가 맞지 않습니다. 확인해주세요! 정해진 파워는 1 부터 3 까지의 정수입니다.</color>");
                    return;
                }
                else
                {
                    isPowerChecked = true;
                }

                if (isGroupChecked && isObjectChecked && isPositionChecked && isPowerChecked && isTimeChecked)
                {
                    setEmergenceTableData();
                    isDataChecked = true;
                    Debug.Log("<color=green>출현 테이블 체크가 완료되었습니다. 출현 테이블을 생성하셔도 괜찮습니다.</color>");
                }
            }
        }

        public void ResetCheck()
        {
            Debug.Log("ResetCheck");
            isDataChecked = false;
        }

        public void ResetEmergenceData()
        {
            Debug.Log("<color=red>ResetEmergenceData</color>");

            groupNum = 0;
            timelines.Clear();
            objectNums.Clear();
            positionNums.Clear();
            powers.Clear();

            isDataChecked = false;
        }

        public void CreateEmergenceData()
        {
            Debug.Log("<color=green>Create EmergenceData</color>");

            if (!isDataChecked)
            {
                Debug.Log("<color=green>EmergenceDataCheck를 한번 실행 해 주세요</color>");
                return;
            }
        }
    }
}
