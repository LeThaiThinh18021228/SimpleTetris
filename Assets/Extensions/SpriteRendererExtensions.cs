using UnityEngine;

namespace PFramework
{
    public static class SpriteRendererExtensions
    {
        public static void SetAlpha(this SpriteRenderer spriteRenderer, float alpha)
        {
            Color newColor = spriteRenderer.color;
            newColor.SetAlpha(alpha);

            spriteRenderer.color = newColor;
        }
    }
}