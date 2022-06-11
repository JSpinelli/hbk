using UnityEngine;
using Random = UnityEngine.Random;

public class GemstoneMovement : MonoBehaviour
{
    public float spinningSpeed;
    public float floatingSpeed;
    public float floatingAmplitude;
    
    private Vector3 _pos;
    private float _startOffset;


    private void Start()
    {
        _startOffset = Random.Range(0,10);
    }

    private void Update()
    {
        _pos = transform.position;
        transform.RotateAround(_pos, Vector3.up, Time.deltaTime *spinningSpeed );
        transform.position = new Vector3(_pos.x, _pos.y + Mathf.Sin(((Time.time * floatingSpeed) + _startOffset )) * floatingAmplitude, _pos.z);
    }
}
