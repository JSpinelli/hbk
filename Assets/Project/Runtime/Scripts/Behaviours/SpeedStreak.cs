using UnityEngine;

public class SpeedStreak : MonoBehaviour
{
    private TrailRenderer _trail;
    private Vector3[] _positions;
    private Vector3 _transformVector;

    public float emittingTimer;
    private float _emitTimer;

    // Start is called before the first frame update
    void Start()
    {
        _trail = GetComponent<TrailRenderer>();
        _emitTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _positions = new Vector3[_trail.positionCount];
        _trail.GetPositions(_positions);
        float yPos = transform.position.y;
        for (int i = 0; i < _positions.Length; i++)
        {
            _positions[i].y = yPos;
            _transformVector = transform.worldToLocalMatrix * _positions[i]; ;
            _positions[i] = transform.localToWorldMatrix * _transformVector;
        }
        _trail.SetPositions(_positions);

        if (_emitTimer < emittingTimer)
        {
            _emitTimer += Time.deltaTime;
        }
        else
        {
            _emitTimer = 0;
            _trail.emitting = !_trail.emitting;
        }
    }
}
