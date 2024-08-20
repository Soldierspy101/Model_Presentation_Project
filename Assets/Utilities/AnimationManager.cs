using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public List<CustomAnimation> animations = new List<CustomAnimation>();

    public void ApplyAnimation(CustomAnimation animationData, GameObject model)
    {
        Debug.Log($"Applying animation to model: {model.name}");
        Animation anim = model.GetComponent<Animation>();
        if (anim == null)
        {
            anim = model.AddComponent<Animation>();
        }

        if (animationData.animationClip != null)
        {
            Debug.Log($"Adding animation clip: {animationData.animationClip.name}");
            anim.AddClip(animationData.animationClip, animationData.animationClip.name);
            anim.clip = animationData.animationClip;
            anim.Play();
        }
        else
        {
            Debug.LogWarning("No animation clip found to apply.");
        }
    }

    public void ResetAllAnimations(GameObject model)
    {
        Animator animator = model.GetComponent<Animator>();
        if (animator != null)
        {
            animator.runtimeAnimatorController = null;
        }
    }

    public void RemoveAnimation(CustomAnimation animationData, GameObject model)
    {
        Debug.Log($"Removing animation from model: {model.name}");
        Animation anim = model.GetComponent<Animation>();
        if (anim != null && animationData.animationClip != null)
        {
            if (anim.GetClip(animationData.animationClip.name) != null)
            {
                anim.RemoveClip(animationData.animationClip.name);
            }
            else
            {
                Debug.LogWarning($"Animation clip '{animationData.animationClip.name}' not found on model '{model.name}'.");
            }
        }
        else
        {
            Debug.LogWarning("No animation component or animation clip found to remove.");
        }
    }
}

[System.Serializable]
public class CustomAnimation
{
    public AnimationClip animationClip; // Custom animation clip
    public string modelName; // Name of the model to which this animation should be applied
}
