using UnityEngine;

namespace Managers
{
    public class SoundManager : MonoBehaviour
    {
        
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip placementAudio;
        [SerializeField] private AudioClip cutAudio;
        [SerializeField] private float minimumPitch;
        [SerializeField] private float maximumPitch;
        [SerializeField] private int pitchStep;
        
        private int _stepCount;
        private void OnEnable()
        {
            EventManager.OnStackBlockPlaced += OnStackBlockPlaced;
        }
        private void OnDisable()
        {
            EventManager.OnStackBlockPlaced -= OnStackBlockPlaced;
        }

        /*private void OnStackBlockPlaced(StackPlacement stackPlacement)
        {
            if (stackPlacement == StackPlacement.Perfect)
            {
                _stepCount = Mathf.Clamp(_stepCount += 1, 0,pitchStep);
                var ratio = (float)_stepCount / pitchStep;
                var pitch = Mathf.Lerp(minimumPitch, maximumPitch, ratio);
                audioSource.panStereo = 0;
                audioSource.pitch = pitch;
                audioSource.PlayOneShot(placementAudio);
            }
            else
            {
                audioSource.panStereo = 0;
                audioSource.pitch = 1;
                audioSource.PlayOneShot(cutAudio);
                _stepCount = -1;
            }
        }*/
        
        private void OnStackBlockPlaced(StackPlacement stackPlacement)
        {
            _stepCount = stackPlacement == StackPlacement.Perfect ? Mathf.Clamp(++_stepCount, 0, pitchStep) : -1;

            var ratio = _stepCount == -1 ? 0 : (float)_stepCount / pitchStep;
            var pitch = _stepCount == -1 ? 1 : Mathf.Lerp(minimumPitch, maximumPitch, ratio);

            audioSource.panStereo = 0;
            audioSource.pitch = pitch;

            var sound = stackPlacement == StackPlacement.Perfect ? placementAudio : cutAudio;
            audioSource.PlayOneShot(sound);
            
        }
        
#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetMouseButtonUp(1))
            {
                OnStackBlockPlaced(StackPlacement.Perfect);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                OnStackBlockPlaced(StackPlacement.Cutted);
            }
        }
    }
#endif
}
