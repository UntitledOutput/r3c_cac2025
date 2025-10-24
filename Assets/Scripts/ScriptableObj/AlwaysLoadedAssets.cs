using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObj
{
    [CreateAssetMenu(fileName = "AlwaysLoadedAssets", menuName = "CAC/AWA", order = 0)]
    public class AlwaysLoadedAssets : ScriptableObject
    {
        public List<Object> assets;
    }
}