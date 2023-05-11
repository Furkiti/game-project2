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
        private int count;
        private void OnEnable()
        {
            EventManager.OnStackBlockPlaced += OnStackBlockPlaced;
        }
        private void OnDisable()
        {
            EventManager.OnStackBlockPlaced -= OnStackBlockPlaced;
        }
        
        private void OnStackBlockPlaced(StackPlacement stackPlacement)
        {
            count = stackPlacement == StackPlacement.Perfect ? Mathf.Clamp(++count, 0, pitchStep) : -1;

            var ratio = count == -1 ? 0 : (float)count / pitchStep;
            var pitch = count == -1 ? 1 : Mathf.Lerp(minimumPitch, maximumPitch, ratio);

            audioSource.panStereo = 0;
            audioSource.pitch = pitch;

            var sound = stackPlacement == StackPlacement.Perfect ? placementAudio : cutAudio;
            audioSource.PlayOneShot(sound);
            
        }
    }
}
