using System;
using NaughtyAttributes;
using UnityEngine;

namespace Gameplay
{
    public class AnimationController : MonoBehaviour
    {
        private static event Action<AnimationState> OnUnitAnimationChanged;

        [Header("Info")]
        [ReadOnly] [SerializeField] private AnimationState currentAnimationState = AnimationState.Idle;
        
        //todo dependinciesleri kullanılan classa göre ayır
        [Header("Dependencies")]
        [SerializeField]private Animator animator;
        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private static readonly int RunHash = Animator.StringToHash("Running");
        private static readonly int FallingHash = Animator.StringToHash("Falling");
        private static readonly int DancingHash = Animator.StringToHash("Dancing");
      

        public enum AnimationState
        {
            Idle,
            Running,
            Falling,
            Dancing
        }
    
        public AnimationState CurrentAnimationState
        {
            get => currentAnimationState;
            set
            {
                if (currentAnimationState == value) return;
                
                currentAnimationState = value;
                SetAnimatorTrigger(value);
                OnUnitAnimationChanged?.Invoke(value);
            }
        }
    
        private void SetAnimatorTrigger(AnimationState value)
        {
            switch (value)
            {
                case AnimationState.Idle:
                    animator.SetTrigger(IdleHash);
                    break;
                case AnimationState.Running:
                    animator.SetTrigger(RunHash);
                    break;
                case AnimationState.Falling:
                    animator.SetTrigger(FallingHash);
                    break;
                case AnimationState.Dancing:
                    animator.SetTrigger(DancingHash);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    
    }
}
