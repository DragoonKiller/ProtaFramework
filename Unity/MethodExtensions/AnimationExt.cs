using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.UI;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
        public static float GetDurationOfCurrentPlaying(this Animator animator)
        {
            var controller = animator.runtimeAnimatorController;
            var state = animator.GetCurrentAnimatorStateInfo(0);
            var clipName = state.shortNameHash;
            return controller.GetDuration(clipName);
        }
        
        public static float GetDuration(this RuntimeAnimatorController controller, int hash)
        {
            var clips = controller.animationClips;
            foreach (var clip in clips)
            {
                if (Animator.StringToHash(clip.name) == hash)
                {
                    return clip.length;
                }
            }
            Debug.LogError($"AnimationClip with hash [{hash}] in [{controller.name}] not found ");
            Debug.LogError(clips.Select(x => x.name + " " + Animator.StringToHash(x.name)).ToStringJoined("\n"));
            return 1.0f;
        }
        
        public static float GetDuration(this RuntimeAnimatorController controller, string clipName)
        {
            var clips = controller.animationClips;
            foreach (var clip in clips)
            {
                if (clip.name == clipName)
                {
                    return clip.length;
                }
            }
            Debug.LogError($"AnimationClip [{clipName}] in [{controller.name}] not found ");
            Debug.LogError(clips.Select(x => x.name).ToStringJoined("\n"));
            return 1.0f;
        }
        
    }
}
