using UnityEngine;

namespace Dasverse.Aleo
{
    public class MultiPosLaunch : MonoBehaviour
    {
        [SerializeField]
        private LaunchPositionDatas launchPositionTableDatas;

        public void SetMultiPosLaunch(GameObject crystal, int positionNum)
        {
            Vector3 pos = launchPositionTableDatas.LaunchPositionPoolDic[(SpawnPosition)positionNum];
            Vector3 rot = launchPositionTableDatas.LaunchRotationPoolDic[(SpawnRotation)positionNum];

            if (positionNum != 2 || positionNum != 3 || positionNum != 9 || positionNum != 10)
            {
                rot = Random.value < 0.5f ? rot : new Vector3(rot.x, rot.y, rot.z - 10);
            }

            crystal.gameObject.transform.SetLocalPositionAndRotation(pos, Quaternion.Euler(rot));
        }
    }
}

