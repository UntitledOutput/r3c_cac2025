using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

namespace Controllers
{
    public class SurfaceController : MonoBehaviour
    {
        public enum SurfaceType
        {
            Cloth,
            Dirt,
            Glass,
            Grass,
            Metal,
            Plastic,
            Rubber,
            Stone,
            Vinyl,
            Wood,
        }

        public SurfaceType Type;

        private void Start()
        {

        }
    }
}