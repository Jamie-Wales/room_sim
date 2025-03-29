using System;
using Interfaces;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveableObject : MonoBehaviour, IInteractable
{
    [SerializeField]
    private GameObject guide;

    private Rigidbody _rigidbody;
    private bool _isDragging;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void OnMouseEnter()
    {
        guide.transform.position = transform.position;
    }

    private void OnMouseDown()
    {
        if (!_isDragging)
        {
            _rigidbody.useGravity = false;
            _isDragging = true;
            guide.transform.position = transform.position;
            transform.parent = guide.transform;
        }

        transform.position = guide.transform.position;
        transform.rotation = guide.transform.rotation;
    }

    private void OnMouseUp()
    {
        _rigidbody.useGravity = true;
        _isDragging = false;

        transform.parent = null;
        transform.position = guide.transform.position;
        transform.rotation = Quaternion.identity;
    }

    public void OnInteract(GameObject interactor)
    {
    }

    public bool CanInteract(GameObject interactor)
    {
        return true;
    }

    public string GetInteractionPrompt(GameObject interactor)
    {
        return _isDragging ? "Release to drop" : "Click to pick up";
    }
}