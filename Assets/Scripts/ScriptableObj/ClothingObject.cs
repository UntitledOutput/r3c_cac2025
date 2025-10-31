using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObj
{
    [CreateAssetMenu(fileName = "New Clothing", menuName = "CAC/New Clothing", order = 0)]
    public class ClothingObject : ScriptableObject
    {
        public enum ClothingType
        {
            Hat,
            Shirt,
            Bottom,
            Shoe,
            Hair
        }

        public string Name;
        public ClothingType Type;
        public int Cost;

        public Sprite Icon;
        public List<Color> Colors;
        
        public GameObject Prefab;
    }
}