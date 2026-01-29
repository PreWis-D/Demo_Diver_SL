using System.Collections.Generic;
using UnityEngine;

public class RigidbodyModel : MonoBehaviour
{
    [SerializeField] private Rigidbody[] _rigidbodies;
    [field: SerializeField] public Transform Spine { get; private set; }

    private List<Collider> _colliders = new List<Collider>();


    private void Awake()
    {
        for (int i = 0; i < _rigidbodies.Length; i++)
            _colliders.Add(_rigidbodies[i].GetComponent<Collider>());
    }

    public List<Collider> GetColliders()
    {
        return _colliders;
    }
}
