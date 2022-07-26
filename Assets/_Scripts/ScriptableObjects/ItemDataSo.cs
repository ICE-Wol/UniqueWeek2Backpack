using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.ScriptableObjects{
    
    [CreateAssetMenu(fileName = "New Data", menuName = "Item Data")]
    public class ItemDataSo : ScriptableObject {
        [Header("Sprite")] 
        public Sprite sprite;

        [Header("Description")]
        public string itemName;
        public string owner;
        [TextArea]
        public string description;
        [TextArea]
        public string ability;
        [FormerlySerializedAs("specialAbility")] [TextArea]
        public string special;

        [Header("Data")]
        public int number;
        public Type type;
        public int price;
        public int quantity;
    }
}
