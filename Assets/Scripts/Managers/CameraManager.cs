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
        
        
        private readonly WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();
        [SerializeField]private float levelCompletedCamRotationSpeed=30;
        private CinemachineOrbitalTransposer _orbitalTransposer;
        private float _angle;
        private bool _isCameraRotating;
        
        private void OnEnable()
        {
            EventManager.OnGameLoaded += OnGameLoaded;
            EventManager.OnGameCompleted += OnGameCompleted;
        }
    
        private void OnDisable()
        {
            EventManager.OnGameLoaded -= OnGameLoaded;
        }

        private void OnGameLoaded()
        {
            _orbitalTransposer = levelCompletedCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
            _currentCam = mainMenuCam;
            ChangeCamera(mainMenuCam,.6f);
        }
        
        private void GameStarted()
        {
            
        }

        private void OnGameCompleted()
        {
            
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
        
        private IEnumerator RotateCam()
        {
            _isCameraRotating = true;
            _angle = _orbitalTransposer.m_Heading.m_Bias;
            while (_isCameraRotating)
            {
                _angle += levelCompletedCamRotationSpeed * Time.deltaTime;
                var val = Mathf.Repeat(_angle, 360);
                var ratio=Mathf.InverseLerp(0, 360, val);
                _orbitalTransposer.m_Heading.m_Bias = Mathf.Lerp(-180, 180, ratio);
                yield return _waitForEndOfFrame;
            }
        }
        
    }
}