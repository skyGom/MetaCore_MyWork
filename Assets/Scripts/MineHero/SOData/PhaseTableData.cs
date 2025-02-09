using UnityEngine;

namespace Dasverse.Aleo
{
    [CreateAssetMenu(menuName = "Datas/PhaseTableData")]
    public class PhaseTableData : ScriptableObject
    {
        public SpawnPhase SpawnPhase;
        public EmergenceTableData[] EmergenceDatas;
    }
}

