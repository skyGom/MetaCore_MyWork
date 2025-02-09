using System;
using System.Collections;
using System.Collections.Generic;
using Rukha93.ModularAnimeCharacter.Customization;
using UnityEngine;
using WebSocketSharp;


namespace Dasverse.Aleo
{
    [CreateAssetMenu(menuName = "Datas/CostumeAssetDataContainer")]
    public class CostumeAssetDataContainer : ScriptableObject
    {   
        public ItemGroup MaleItems;
        public ItemGroup FemaleItems;
    }

    [Serializable]
    public class ItemGroup
    {   
        public CostumeAssetData[] Bodies;
        public CostumeAssetData[] Heads;
        public CostumeAssetData[] HairStyles;
        public CostumeAssetData[] Tops;
        public CostumeAssetData[] Bottoms;
        public CostumeAssetData[] Shoes;
        [Header("x: Top, y: Bottom, z: Shoes")]
        [SerializeField]
        public Vector3[] Outfits;
    }
}
