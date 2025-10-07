using System;
using MyBox;
using UnityEngine;
using Utils;

namespace Controllers
{
    public class CameraController : MonoBehaviour
    {
        public float PlrRadius = 3;
        public float PlrY;
        
        private PlayerController _plrCtrl;
        private float _plrRotateAngle = 180;
        private float _plrOffsetY = 2.5f;
        
        private void Start()
        {
            _plrCtrl = FindAnyObjectByType<PlayerController>();
        }


        private void Update()
        {
            //_plrRotateAngle += InputSystem.Look.x;
            //_plrOffsetY += InputSystem.Look.y;

            _plrOffsetY = 2.5f;
            _plrRotateAngle = Mathf.Deg2Rad * 180;

            _plrOffsetY = _plrOffsetY.Clamp(-1.0f, 2.5f);
            
            transform.position = _plrCtrl.transform.position + new Vector3(
                Mathf.Sin(_plrRotateAngle) * PlrRadius, PlrY + _plrOffsetY, Mathf.Cos(_plrRotateAngle) * PlrRadius);

            transform.position = transform.position.ClampZ(-105, Mathf.Infinity);
            
            transform.LookAt(_plrCtrl.transform.position + new Vector3(0,1.5f,0));
        }
    }
}