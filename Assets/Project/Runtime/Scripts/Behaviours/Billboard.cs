using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform _target;
    public void LateUpdate()
    {
        transform.LookAt(_target);
    }

    public void Start()
    {
        _target = Camera.main.transform; //cace the transform of the camera
    }
}