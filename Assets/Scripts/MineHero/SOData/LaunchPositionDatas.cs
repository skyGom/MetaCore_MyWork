using System;
using UnityEngine;

[Serializable]
public class LaunchPositionPoolDic : SerializableDictionary<SpawnPosition, Vector3> { };

[Serializable]
public class LaunchRotationPoolDic : SerializableDictionary<SpawnRotation, Vector3> { };

[CreateAssetMenu(menuName = "Datas/LaunchPositionPoolDatas")]
public class LaunchPositionDatas : ScriptableObject
{
    public LaunchPositionPoolDic LaunchPositionPoolDic;
    public LaunchRotationPoolDic LaunchRotationPoolDic;
}


