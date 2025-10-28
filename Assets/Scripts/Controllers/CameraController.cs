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
        public float TargetFieldOfView = 75;

        public bool CollideWithMap = true;
        
        private PlayerController _plrCtrl;
        private float _plrRotateAngle = 180;
        private float _plrOffsetY = 2.5f;

        public bool FocusOnPlayer = true;

        public Transform FocusPoint;
        public Vector3 FocusOffset;
        public Vector3 PositionOffset;

        private Camera _camera;
        
        private void Start()
        {
            _plrCtrl = FindAnyObjectByType<PlayerController>();
            _camera = GetComponent<Camera>();
        }
        
        private float currentDistance;
        private float currentFOV;
        private void Update()
        {
            //_plrRotateAngle += InputSystem.Look.x;
            //_plrOffsetY += InputSystem.Look.y;

            if (_camera == null)
                _camera = GetComponent<Camera>();

            if (FocusOnPlayer)
            {
                
                _plrOffsetY = 2.5f;
                _plrRotateAngle = Mathf.Deg2Rad * 180;

                _plrOffsetY = _plrOffsetY.Clamp(-1.0f, 2.5f);

                var minDistance = 1.0f;
                
                var targetPosition  =_plrCtrl.transform.position + new Vector3(
                    Mathf.Sin(_plrRotateAngle) * PlrRadius, PlrY + _plrOffsetY, Mathf.Cos(_plrRotateAngle) * PlrRadius);


                
                RaycastHit hit;
                if (Physics.Raycast(_plrCtrl.transform.position, (targetPosition - _plrCtrl.transform.position).normalized, out hit, PlrRadius, LayerMask.GetMask("Map", "MapObject")) && CollideWithMap)
                {
                    // If an obstruction is found, adjust the current distance
                    currentDistance = Mathf.Lerp(
                        currentDistance, Mathf.Max(minDistance, hit.distance), Time.deltaTime * 3.0f);
                    var multiplier = (1.0f - (currentDistance / PlrRadius)) * 35f;
                    currentFOV = Mathf.Lerp(currentFOV, TargetFieldOfView + multiplier, Time.deltaTime * 3.0f);
                }
                else
                {
                    // No obstruction, return to max distance
                    currentDistance = Mathf.Lerp(currentDistance, PlrRadius, Time.deltaTime * 3.0f);
                    currentFOV = Mathf.Lerp(currentFOV, TargetFieldOfView, Time.deltaTime * 3.0f);
                }
                
                _camera.fieldOfView = currentFOV;
                
                var _targetPosition = _plrCtrl.transform.position + new Vector3(
                    Mathf.Sin(_plrRotateAngle) * currentDistance, PlrY + _plrOffsetY, Mathf.Cos(_plrRotateAngle) * currentDistance);
                
                transform.position = Vector3.Lerp(
                    transform.position, targetPosition, Time.deltaTime * 3.0f);

                transform.position = transform.position.ClampZ(-105, Mathf.Infinity);
            
                transform.LookAt(_plrCtrl.transform.position + new Vector3(0,1.5f,0));
            }
            else
            {
                transform.position = Vector3.Lerp(
                    transform.position, FocusPoint.position + PositionOffset, Time.deltaTime * 3.0f);
                transform.LookAt(FocusPoint.position+FocusOffset);
            }
        }
    }
}