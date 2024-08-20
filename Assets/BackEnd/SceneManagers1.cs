using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManagers1 : MonoBehaviour
{
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private Transform _button1Location; // Position of Button 1
    [SerializeField] private Transform _button2Location; // Position of Button 2
    [SerializeField] private float _interactionPointRadius;
    [SerializeField] private LayerMask _interactableMask;
    public Button button1;
    public Button button2;

    private readonly Collider[] _layers = new Collider[3];
    [SerializeField] private int _numFound;

    private void Start()
    {
        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);
    }

    private void Update()
    {
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _layers, _interactableMask);

        if (_numFound >= 1)
        {
            UnlockMouse();

            foreach (Collider col in _layers)
            {
                if (col != null)
                {
                    // Check if the interactable object is near Button 1
                    if (Vector3.Distance(col.transform.position, _button1Location.position) < 1.0f)
                    {
                        button1.gameObject.SetActive(true);
                        button2.gameObject.SetActive(false);
                        return; // Exit loop since we found one interactable
                    }
                    // Check if the interactable object is near Button 2
                    else if (Vector3.Distance(col.transform.position, _button2Location.position) < 1.0f)
                    {
                        button1.gameObject.SetActive(false);
                        button2.gameObject.SetActive(true);
                        return; // Exit loop since we found one interactable
                    }
                }
            }
        }
        else
        {
            // No interactables found, deactivate both buttons
            button1.gameObject.SetActive(false);
            button2.gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
        Gizmos.DrawWireSphere(_button1Location.position, 0.1f); // Draw Button 1 location
        Gizmos.DrawWireSphere(_button2Location.position, 0.1f); // Draw Button 2 location
    }

    void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
