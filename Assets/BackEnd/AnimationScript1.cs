using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationScript1 : MonoBehaviour
{
    private bool isButtonInitialized = false;
   [System.Serializable]
    public class AnimationData
    {
        public Transform targetTransform; // Use Transform instead of MeshRenderer to manipulate position and rotation
        public Vector3 disassemblyOffset; // Offset for disassembly
        public float disassemblyDuration; // Duration of disassembly animation
        [HideInInspector]
        public Vector3 finalPosition; // Store final position for disassembly
        [HideInInspector]
        public Quaternion finalRotation; // Store final rotation for disassembly
        [HideInInspector]
        public Vector3 initialPosition; // Store initial position for reassembly
        [HideInInspector]
        public Quaternion initialRotation; // Store initial rotation for reassembly
    }

    [SerializeField] private AnimationData[] animationDataArray;
    //[HideInInspector]
    [SerializeField] private Toggle toggle; // Reference to the UI toggle

    void Start()
    {
        Debug.Log("Start method called.");

        // Check if animationDataArray is null
        if (animationDataArray == null)
        {
            Debug.LogWarning("AnimationDataArray is null.");
            return;
        }

        foreach (AnimationData animationData in animationDataArray)
        {
            // Check if targetTransform is null
            if (animationData.targetTransform == null)
            {
                Debug.LogWarning("TargetTransform is null for one of the animation data entries.");
                continue; // Skip this entry
            }

            // Store initial positions and rotations for reassembly
            animationData.initialPosition = animationData.targetTransform.position;
            animationData.initialRotation = animationData.targetTransform.rotation;
        }

        // Check if toggle is not null
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
        else
        {
            Debug.LogWarning("Toggle is null.");
        }
    }

    private void Update()
    {
        if (!isButtonInitialized)
        {
            InitializeButton();
        }
    }
    private void InitializeButton()
    {
        GameObject gameObject = GameObject.Find("UI");
        if (gameObject != null)
        {
            Transform toggleTransform = gameObject.transform.Find("AnimationToggle");
            if (toggleTransform != null)
            {
                toggle = toggleTransform.GetComponent<Toggle>();
                if (toggle != null)
                {
                    toggle.onValueChanged.AddListener(OnToggleValueChanged);
                    Debug.Log("Listener Added");
                    isButtonInitialized = true;
                }
                Debug.Log("Toggle Set!");
            }
        }
    }

    // Method to trigger disassembly animation
    public void Disassemble()
    {
        foreach (AnimationData animationData in animationDataArray)
        {
            // Check if the part is already disassembled
            if (animationData.targetTransform.position == animationData.finalPosition && animationData.targetTransform.rotation == animationData.finalRotation)
            {
                // If already disassembled, reset to initial position and rotation
                animationData.targetTransform.position = animationData.initialPosition;
                animationData.targetTransform.rotation = animationData.initialRotation;
            }
            else
            {
                // If not disassembled, play the disassembly animation
                StartCoroutine(DisassemblePart(animationData));
            }
        }
    }

    // Coroutine to disassemble a part
    IEnumerator DisassemblePart(AnimationData animationData)
    {
        float elapsedTime = 0f;
        Vector3 initialPosition = animationData.targetTransform.position;
        Quaternion initialRotation = animationData.targetTransform.rotation;
        Vector3 targetPosition = initialPosition + animationData.disassemblyOffset;

        while (elapsedTime < animationData.disassemblyDuration)
        {
            float t = elapsedTime / animationData.disassemblyDuration;
            animationData.targetTransform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            animationData.targetTransform.rotation = Quaternion.Lerp(initialRotation, Quaternion.identity, t); // Rotate to identity rotation
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final position and rotation are correct
        animationData.targetTransform.position = targetPosition;
        animationData.finalPosition = targetPosition;
        animationData.targetTransform.rotation = Quaternion.identity;
        animationData.finalRotation = Quaternion.identity;
    }

    public void ResetAll()
    {
        foreach (AnimationData animationData in animationDataArray)
        {
            // Reset the model to its initial position and rotation
            if (animationData.targetTransform != null)
            {
                animationData.targetTransform.position = animationData.initialPosition;
                animationData.targetTransform.rotation = animationData.initialRotation;
            }
        }
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            Disassemble();
        }
        else
        {
            ResetAll();
        }
    }
   
}
