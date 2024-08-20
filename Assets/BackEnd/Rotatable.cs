using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rotatable : MonoBehaviour
{
    [SerializeField] private InputActionMap inputMap;
    public CinemachineFreeLook freelookCamera;

    private void Awake()
    {
        freelookCamera = FindFreelookCamera();
        inputMap = new InputActionMap("Rotatable");
        inputMap.AddAction("Rotate", binding: "<Mouse>/rightButton");
        inputMap.actionTriggered += OnActionTriggered;
    }

    private void OnActionTriggered(InputAction.CallbackContext context)
    {
        if (context.action.name == "Rotate")
        {
            if (context.started)
            {
                StartRotation();
            }
            else if (context.canceled)
            {
                StopRotation();
            }
        }
    }

    private void StartRotation()
    {
        if (freelookCamera != null)
        {
            freelookCamera.enabled = true;
        }
    }

    private void StopRotation()
    {
        if (freelookCamera != null)
        {
            freelookCamera.enabled = false;
        }
    }

    private CinemachineFreeLook FindFreelookCamera()
    {
        GameObject cameraObject = GameObject.FindGameObjectWithTag("FreelookCamera");

        if (cameraObject != null)
            return cameraObject.GetComponent<CinemachineFreeLook>();
        else
        {
            Debug.LogError("Freelook camera not found!");
            return null;
        }
    }

    private void OnEnable()
    {
        inputMap.Enable();
    }

    private void OnDisable()
    {
        inputMap.Disable();
    }
}
