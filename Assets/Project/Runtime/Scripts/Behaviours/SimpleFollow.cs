using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollow : MonoBehaviour
{
    // References
    public Transform target;
    
    // Properties
    public bool withOffset = true;
    
    // State
    private Vector3 _offset;

    void Start()
    {
        if (withOffset)
            _offset = transform.position - target.position;
        else
        {
            _offset = Vector3.zero;
        }
    }
    
    void Update()
    {
        Vector3 position = new Vector3(target.position.x, 0, target.position.z) + _offset;
        transform.position = position;
    }
}
