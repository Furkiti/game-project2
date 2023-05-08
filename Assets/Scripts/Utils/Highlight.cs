using UnityEngine;

namespace Utils
{
    public class Highlight
    {
        private SpriteRenderer _spriteOutlineRenderer;
        private static readonly int OutlineEnabled = Shader.PropertyToID("_OutlineEnabled");
        private static readonly int SolidOutline = Shader.PropertyToID("_SolidOutline");
        
        public Highlight(SpriteRenderer spriteOutlineRenderer)
        {
            _spriteOutlineRenderer = spriteOutlineRenderer;
        }
        
        public void OutlineHighlight(bool state)
        {
            _spriteOutlineRenderer.enabled = state;
        }
        
        public void OutlineHighlight(bool state,Color color)
        {
            _spriteOutlineRenderer.enabled = state;
            _spriteOutlineRenderer.color = color;
        }
    }
}

