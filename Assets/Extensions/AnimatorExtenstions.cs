using UnityEngine;

namespace PFramework
{
    public static class AnimatorExtensions
    {
        public static float GetLength(this Animator animator, string clipName)
        {
            var controller = animator.runtimeAnimatorController;
            var clips = controller.animationClips;
            int count = clips.Length;

            for (int i = 0; i < count; i++)
            {
                var clip = clips[i];
                if (clip.name == clipName)
                {
                    return clip.length;
                }
            }

            return 0f;
        }
    }
}
