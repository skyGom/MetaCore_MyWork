using System.Collections.Generic;
using UnityEngine;

namespace Dasverse.Aleo
{
    [CreateAssetMenu(menuName = "Datas/EmergenceData")]
    public class EmergenceTableData : ScriptableObject
    {
        public int GroupNum;
        public List<int> Timelines;
        public List<int> ObjectNums;
        public List<int> PositionNums;
        public List<int> Powers;
    }
}
