using UnityEngine;

namespace Gameplay.Stacks
{
    public class StackBlock : MonoBehaviour
    {
        [HideInInspector]
        public Material Material;
        private void Awake()
        {
            Material = GetComponent<MeshRenderer>().material;
        }
    }
}