using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class ContinuousTransform : MonoBehaviour
    {
        public Vector3 Position;
        public Vector3 Euler;
        public Vector3 Scale;

        private void Update()
        {
            transform.position += Position * Time.deltaTime;
            transform.localEulerAngles += Euler * Time.deltaTime;
            transform.localScale += Scale * Time.deltaTime;
        }
    }
}