using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDown1 : MonoBehaviour
{
    private Animator _doorAnimator;
    // Start is called before the first frame update
    void Start()
    {
        _doorAnimator = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _doorAnimator.SetTrigger("isOpen");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _doorAnimator.SetTrigger("isClosed");
        }
    }

}
