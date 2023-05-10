using System.Collections;
using Cinemachine;
using UnityEngine;

namespace Managers
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField]private CinemachineBrain mainCameraBrain;
        [SerializeField]private CinemachineVirtualCamera mainMenuCam;
        [SerializeField]private CinemachineVirtualCamera playerFollowerCam;
        [SerializeField]private CinemachineVirtualCamera levelCompletedCam;
        private CinemachineVirtualCamera _currentCam;
        [SerializeField]private float levelCompletedCamRotationSpeed=30;
        private bool _isCameraRotating;
        
        private void OnEnable()
        {
            EventManager.OnGameLoaded += OnGameLoaded;
            EventManager.OnGameStarted += OnGameStarted;
            EventManager.OnGameCompleted += OnGameCompleted;
            EventManager.OnGameContinue += OnGameContinue;
        }
    
        private void OnDisable()
        {
            EventManager.OnGameLoaded -= OnGameLoaded;
            EventManager.OnGameStarted -= OnGameStarted;
            EventManager.OnGameCompleted -= OnGameCompleted;
            EventManager.OnGameContinue -= OnGameContinue;
        }

        private void OnGameLoaded()
        {
            _currentCam = mainMenuCam;
            ChangeCamera(mainMenuCam,.6f);
        }

        private void OnGameStarted()
        {
            ChangeCamera(playerFollowerCam);
        }
        
        private void OnGameCompleted()
        {
            ChangeCamera(levelCompletedCam);
            StartCoroutine(RotateCam(levelCompletedCam));
        }
        
        private void OnGameContinue()
        {
            ChangeCamera(playerFollowerCam);
            _isCameraRotating = false;
        }
        private void ChangeCamera(CinemachineVirtualCamera newCam,float blendTime)
        {
            mainCameraBrain.m_DefaultBlend.m_Time = blendTime;
            ChangeCamera(newCam);
        }
    
        private void ChangeCamera(CinemachineVirtualCamera newCam)
        {
            _currentCam.Priority = 10;
            _currentCam = newCam;
            _currentCam.Priority = 11;
        }
        
        private IEnumerator RotateCam(CinemachineVirtualCamera rotatedCam)
        {
            _isCameraRotating = true;
            var orbitalTransposer = rotatedCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
            var angle = orbitalTransposer.m_Heading.m_Bias;
            while (_isCameraRotating)
            {
                angle += levelCompletedCamRotationSpeed * Time.deltaTime;
                var val = Mathf.Repeat(angle, 360);
                var ratio=Mathf.InverseLerp(0, 360, val);
                orbitalTransposer.m_Heading.m_Bias = Mathf.Lerp(-180, 180, ratio);
                yield return new WaitForEndOfFrame();
            }
        }
        
    }
}