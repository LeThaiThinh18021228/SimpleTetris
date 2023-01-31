using UnityEngine;
using UnityEngine.UI;

namespace PFramework
{
    public static class ImageExtensions
    {
        public static void SetAlpha(this Image image, float alpha)
        {
            Color newColor = image.color;
            newColor.SetAlpha(alpha);

            image.color = newColor;
        }
    }
}