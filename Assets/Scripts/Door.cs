using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(requiredComponent: typeof(Animator))]
public class Door : MonoBehaviour, IInteractable
{
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        _animator.SetBool("hasOpened", !_animator.GetBool("hasOpened"));
    }

    public void Highlight()
    {

    }
}

interface IInteractable
{
    public void Interact();

    public void Highlight();
}
