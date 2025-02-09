using System;
using UnityEngine;

namespace Dasverse.Aleo
{   
    [Serializable]
    public class CostumeServerData
    {   
        /// <summary>
        /// 코스튬의 ID
        /// </summary>
        [Tooltip("코스튬의 ID")]
        public int Sep;
        /// <summary>
        /// 남 = 1, 여 = 2
        /// </summary>
        [Tooltip("남 = 1, 여 = 2")]
        public int Avartar;
        /// <summary>
        /// 코스튬 부위 Skin = 1, Head = 2, Hair = 3, Top = 4, Bottom = 5, Shoes = 6
        /// </summary>
        [Tooltip("코스튬 부위 Skin = 1, Head = 2, Hair = 3, Top = 4, Bottom = 5, Shoes = 6")]
        public int Parts;
        /// <summary>
        /// 실장 여부
        /// </summary>
        [Tooltip("실장 여부")]
        public int Builtin;
        /// <summary>
        /// 코스튬의 이름
        /// </summary>
        [Tooltip("코스튬의 이름")]
        public string Name;
        public float Ability_Speed;
        public float Ability_Stamina;
        public float Ability_Speedrate;
        public float Ability_Staminarate;
    }

    [CreateAssetMenu(menuName = "Datas/CostumeAssetData")]
    public class CostumeAssetData : ScriptableObject
    {   
        /// <summary>
        /// 서버와 엑셀 데이터 기반의 데이터
        /// </summary>
        public CostumeServerData CostumeServerData;
        /// <summary>
        /// 코스튬의 클라리언트상의 ID
        /// </summary>
        [Tooltip("코스튬의 클라리언트상의 ID")]
        public int ID;
        /// <summary>
        /// 코스튬의 파츠 타입
        /// </summary>
        [Tooltip("코스튬의 파츠 타입")]
        public CostumeType CostumeType;
        /// <summary>
        /// 코스튬 장비시 비활성화할 바디파츠 타입
        /// </summary>
        [Tooltip("코스튬 장비시 비활성화할 바디파츠 타입")]
        public BodyPartType[] DisableBodyPartTypes;
        /// <summary>
        /// 코스튬 장비시 타겟 할 본
        /// </summary>
        [Tooltip("헤어 타겟 할 본, 추후 장비 아이탬 타겟 본으로 사용 가능")]
        public HumanBodyBones TargetBone;
    }
}
