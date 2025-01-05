using UnityEngine;

namespace IgniteCoders.Simple_Water_Shader.Resources
{
    public class WaterReflection : MonoBehaviour
    {
        // referenses
        private Camera _mainCamera;
        private Camera _reflectionCamera;

        [Tooltip("The plane where the camera will be reflected, the water plane or any object with the same position and rotation")]
        public Transform reflectionPlane;
        [Tooltip("The texture used by the Water shader to display the reflection")]
        public RenderTexture outputTexture;

        // parameters
        public bool copyCameraParamerers;
        public float verticalOffset;
        private bool _isReady;

        // cache
        private Transform _mainCamTransform;
        private Transform _reflectionCamTransform;

        public void Awake()
        {
            _mainCamera = Camera.main;

            _reflectionCamera = GetComponent<Camera>();

            Validate();
        }

        private void Update()
        {
            if (_isReady)
                RenderReflection();
        }

        private void RenderReflection()
        {
            // take main camera directions and position world space
            Vector3 cameraDirectionWorldSpace = _mainCamTransform.forward;
            Vector3 cameraUpWorldSpace = _mainCamTransform.up;
            Vector3 cameraPositionWorldSpace = _mainCamTransform.position;

            cameraPositionWorldSpace.y += verticalOffset;

            // transform direction and position by reflection plane
            Vector3 cameraDirectionPlaneSpace = reflectionPlane.InverseTransformDirection(cameraDirectionWorldSpace);
            Vector3 cameraUpPlaneSpace = reflectionPlane.InverseTransformDirection(cameraUpWorldSpace);
            Vector3 cameraPositionPlaneSpace = reflectionPlane.InverseTransformPoint(cameraPositionWorldSpace);

            // invert direction and position by reflection plane
            cameraDirectionPlaneSpace.y *= -1;
            cameraUpPlaneSpace.y *= -1;
            cameraPositionPlaneSpace.y *= -1;

            // transform direction and position from reflection plane local space to world space
            cameraDirectionWorldSpace = reflectionPlane.TransformDirection(cameraDirectionPlaneSpace);
            cameraUpWorldSpace = reflectionPlane.TransformDirection(cameraUpPlaneSpace);
            cameraPositionWorldSpace = reflectionPlane.TransformPoint(cameraPositionPlaneSpace);

            // apply direction and position to reflection camera
            _reflectionCamTransform.position = cameraPositionWorldSpace;
            _reflectionCamTransform.LookAt(cameraPositionWorldSpace + cameraDirectionWorldSpace, cameraUpWorldSpace);
        }

        private void Validate()
        {
            if (_mainCamera != null)
            {
                _mainCamTransform = _mainCamera.transform;
                _isReady = true;
            }
            else
                _isReady = false;

            if (_reflectionCamera != null)
            {
                _reflectionCamTransform = _reflectionCamera.transform;
                _isReady = true;
            }
            else
                _isReady = false;

            if (_isReady && copyCameraParamerers)
            {
                copyCameraParamerers = !copyCameraParamerers;
                _reflectionCamera.CopyFrom(_mainCamera);

                _reflectionCamera.targetTexture = outputTexture;
            }
        }
    }
}