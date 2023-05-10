using DG.Tweening;
using Managers;
using UnityEngine;

namespace UI
{
    public enum UIAnimation
    {
        ToLeft,
        ToRight,
        ToUp,
        ToDown,
    }

    [System.Serializable]
    public class UIBehaviour
    {
        public UIAnimation uiAnimation;
        public float duration;
        public Ease ease;
    }

    [System.Serializable]
    public class UIBehaviourActivation : UIBehaviour
    {
        private Vector2 _reverseDirection;
        
        public Vector2 ReverseDirection
        {
            get
            {
                return _reverseDirection;
            }

            set => _reverseDirection = value;
        }
        
        public void SetReverseDirection()
        {
            if (uiAnimation == UIAnimation.ToDown)
            {
                ReverseDirection = -Vector2.down;
            }
            else if (uiAnimation == UIAnimation.ToLeft)
            {
                ReverseDirection = -Vector2.left;
            }
            else if (uiAnimation == UIAnimation.ToRight)
            {
                ReverseDirection = -Vector2.right;
            }
            else if (uiAnimation == UIAnimation.ToUp)
            {
                ReverseDirection = -Vector2.up;
            }
            
            //Debug.Log(ReverseDirection);
        }
    }

    [System.Serializable]
    public class UIBehaviourDeactivation : UIBehaviour
    {
        private Vector2 _direction;
        public Vector2 Direction
        {
            get
            {
                return _direction;
            }

            set => _direction = value;
        }
        
        public void SetDirection()
        {
            if (uiAnimation == UIAnimation.ToDown)
            {
                Direction = Vector2.down;
            }
            else if (uiAnimation == UIAnimation.ToLeft)
            {
                Direction = Vector2.left;
            }
            else if (uiAnimation == UIAnimation.ToRight)
            {
                Direction = Vector2.right;
            }
            else if (uiAnimation == UIAnimation.ToUp)
            {
                Direction = Vector2.up;
            }
            
            //Debug.Log(Direction);
        }
    }
    public class UIElement : MonoBehaviour
    {
        public bool isUIInitiallyActive = false;
        public bool isUIActive;
        public UIBehaviourActivation activationBehaviour;
        public UIBehaviourDeactivation deactivationBehaviour;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            ConfigureDirections();
          
        }

        private void Start()
        {
            if (!isUIInitiallyActive)
            {
                ResetUIPosition();
            }
            
        }

        public void ActivateUI()
        {
            UIManager.Instance.ActivateUI(_rectTransform,activationBehaviour.duration,activationBehaviour.ease);
        }

        public void DeactivateUI()
        {
            if (isUIActive)
            {
                UIManager.Instance.DeactivateUI(_rectTransform,deactivationBehaviour.Direction,deactivationBehaviour.duration,deactivationBehaviour.ease,
                    activationBehaviour.ReverseDirection);
            }
           
        }

        public void ResetUIPosition()
        {
            UIManager.Instance.ResetUIPosition(_rectTransform,activationBehaviour.ReverseDirection);
        }

        private void OnValidate()
        {
            /*ConfigureDirections();
            ResetUIPosition();*/
        }

        private void ConfigureDirections()
        {
            activationBehaviour.SetReverseDirection();
            deactivationBehaviour.SetDirection();
        }
    }
}
