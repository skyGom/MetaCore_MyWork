using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using minigame;
using System.Linq;

namespace Dasverse.Aleo
{
    /// <summary>
    /// Mini Game MineHero packet을 처리하기 위한 Processor
    /// </summary>
    public class MineHeroPacketProcessor : MiniGamePacketProcessor
    {
        /// <summary>
        /// Mini Game MineHero packet을 처리하기 위한 Processor. 공용 패킷 처리는 상위 클래스인 MiniGamePacketProcessor에서 처리한다.
        /// </summary>
        /// <param name="header">packet header</param>
        /// <param name="data">packet header + body를 포함하는 json string 원본</param>
        /// <returns>처리 완료 여부. 정의되지 않은 header가 들어와 처리가 불가능한 경우 false를 반환한다.</returns>
        public override bool Process(string header, string data)
        {
            switch (header)
            {
                // 전용 패킷이거나, 공용 패킷이지만 게임 종류에 따라 별도 처리가 필요한 패킷들은 여기서 진행한다
                case "BattleStart": battleStartResponse(JsonConvert.DeserializeObject<BattleStartResponse>(data)); break;
                case "BattlePoint": battlePointResponse(JsonConvert.DeserializeObject<BattlePointResponse>(data)); break;
                case "BattleExit": battleExitResponse(JsonConvert.DeserializeObject<BattleExitResponse>(data)); break;
                case "BattleEnd": battleEndResponse(JsonConvert.DeserializeObject<BattleEndResponse>(data)); break;
                case "BattleBonusTime": battleBonusTimeResponse(JsonConvert.DeserializeObject<BattleBonusTimeResponse>(data)); break;
                case "BattleItem": battleItemResponse(JsonConvert.DeserializeObject<minigame.MineHero.BattleItemResponse>(data)); break;
                case "BattleItem_Emp" : battleEmpResponse(JsonConvert.DeserializeObject<minigame.MineHero.BattleItemEmpResponse>(data)); break;
                default:
                    // 공용 패킷 처리는 상위 클래스에서 진행
                    return base.Process(header, data);
            }
            return true;
        }

        /// <summary>
        /// 게임 시작 알림
        /// </summary>
        /// <param name="res">result 0:성공, 1:방없음, 2:베팅금액설정오류, 3:이미게임중. res.map에는 출현 정보가 포함</param>
        private void battleStartResponse(BattleStartResponse res)
        {
            if (res.result == 0)
            {
                MineHeroGameMain.Instance.MainGameStartAsync(
                    CrystalSpawner.Instance.GetEmergenceNumList(res.map),
                    (int)res.availableItem.Where(x => x.type == 14).FirstOrDefault().count,
                    (int)res.availableItem.Where(x => x.type == 15).FirstOrDefault().count
                    ).Forget();
            }
            else if (res.result == 3)
            {
                BattleServerManager.Instance.IsSendBattleStart = true;
                MineHeroGameMain.Instance.GameStateNum = 2;
            }
            else
            {
                Debug.LogFormat("error result : {0}", res.result);
            }
        }

        /// <summary>
        /// 실시간 점수 체크
        /// </summary>
        /// <param name="res">result 0:성공, 1:실패.</param>
        private void battlePointResponse(BattlePointResponse res)
        {
            if (res.result == 0)
            {
                if (BattleServerManager.Instance.SessionId == res.sessionId)
                {
                    if (res.objectTableId == 117 || res.objectTableId == 118 || res.objectTableId == 119)
                    {
                        MineHeroPointManager.Instance.TotalBonus += res.getObjectPoint;
                    }
                    else
                    {
                        MineHeroPointManager.Instance.TotalHitPoint += res.getObjectPoint + res.getComboPoint + res.getMultiComboPoint;
                    }

                    MineHeroPointManager.Instance.TotalPoint += res.getObjectPoint + res.getComboPoint + res.getMultiComboPoint;
                    Debug.Log($"<color=blue>ServerRecordTotalPoint : {MineHeroPointManager.Instance.TotalPoint}</color>");
                }
                else
                {
                    MineHeroGameMain.Instance.EnemyTotalPoint += res.getObjectPoint + res.getComboPoint + res.getMultiComboPoint;
                }
            }
            else
            {
                Debug.LogFormat("error result : {0}", res.result);
            }
        }

        /// <summary>
        /// 게임 종료
        /// </summary>
        /// <param name="res">result 0:성공, 1:실패. 성공시 게임 결과 정보를 포함한다.</param>
        protected override void battleEndResponse(BattleEndResponse res)
        {
            if (res.result == 0)
            {
                if (MineHeroGameMain.Instance != null)
                {
                    MineHeroGameMain.Instance.SetResponsePointList(res);
                    MineHeroGameMain.Instance.MainGameEndAsync().Forget();
                }
                else
                {
                    gameEndAsync(res).Forget();
                }
            }
            else
            {
                Debug.LogFormat("error result : {0}", res.result);
            }
            base.battleEndResponse(res);
        }

        private async UniTaskVoid gameEndAsync(BattleEndResponse res)
        {
            await UniTask.WaitUntil(() => MineHeroGameMain.Instance != null, cancellationToken: GameCore.UIWorkCts.Token);

            MineHeroGameMain.Instance.SetResponsePointList(res);
            MineHeroGameMain.Instance.MainGameEndAsync().Forget();
        }

        /// <summary>
        /// 사용자 이탈 알림
        /// </summary>
        /// <param name="res">result 0:성공, 1:실패. sessionId: 이탈한 사용자의 session id.</param>
        protected override void battleExitResponse(BattleExitResponse res)
        {
            if (res.result == 0)
            {
                MineHeroGameMain.Instance.ShowGiveUpPopup();
            }
            else
            {
                Debug.LogFormat("error result : {0}", res.result);
            }
        }

        /// <summary>
        /// 10초 보너스 시간 알림
        /// </summary>
        /// <param name="res">result 0:성공, 1:실패</param>
        private void battleBonusTimeResponse(BattleBonusTimeResponse res)
        {
            if (res.result == 0)
            {
                MineHeroGameMain.Instance.FinishModeStartAsync().Forget();
            }
            else
            {
                Debug.LogFormat("error result : {0}", res.result);
            }
        }

        /// <summary>
        /// 아이템 사용 알림
        /// </summary>
        /// <param name="res">result 0:성공, 1:실패. res.itemId : 14 = Emp, 15 = Ignore </param>
        private void battleItemResponse(minigame.MineHero.BattleItemResponse res)
        {
            if (res.result == 0)
            {
                if(res.itemId == 14)
                {
                    MineHeroGameMain.Instance.EmpShockItem();
                }
                else if(res.itemId == 15)
                {
                    MineHeroGameMain.Instance.IgnoreBombItem();
                }
                else
                {
                    Debug.LogFormat("error itemId : {0}", res.itemId);
                }
            }
            else
            {
                Debug.LogFormat("error result : {0}", res.result);
            }
        }

        /// <summary>
        /// EMP 아이템 사용 결과
        /// </summary>
        /// <param name="res"></param>
        private void battleEmpResponse(minigame.MineHero.BattleItemEmpResponse res)
        {
            if (res.result == 0)
            {
                if (res.destroyedObjectList.Where(x => x.objectId == 117 || x.objectId == 118 || x.objectId == 119).Count() > 0)
                {
                    MineHeroPointManager.Instance.TotalBonus += res.getObjectPoint;
                }
                else
                {
                    MineHeroPointManager.Instance.TotalHitPoint += res.getObjectPoint + res.getComboPoint + res.getMultiComboPoint;
                }

                MineHeroPointManager.Instance.TotalPoint += res.getObjectPoint + res.getComboPoint + res.getMultiComboPoint;
                Debug.Log($"<color=blue>ServerRecordTotalPoint : {MineHeroPointManager.Instance.TotalPoint}</color>");
            }
            else
            {
                Debug.LogFormat("error result : {0}", res.result);
            }
        }
    }
}
