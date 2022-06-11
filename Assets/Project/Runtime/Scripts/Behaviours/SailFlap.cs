using UnityAtoms.BaseAtoms;
using UnityEngine;

public class SailFlap : MonoBehaviour
{
    private TrailRenderer _trail;
    private Vector3[] _positions;
    public float magnitude;
    public float speed;
    private Vector3 _transformVector;
    public FloatReference sailContribution;

    // Start is called before the first frame update
    void Start()
    {
        _trail = GetComponent<TrailRenderer>();
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
            _transformVector = transform.worldToLocalMatrix * _positions[i];
            _transformVector.x = _transformVector.x + (Mathf.Sin(_transformVector.y + (Time.deltaTime * speed)) * ((1-sailContribution) * magnitude) * Time.timeScale);
            _positions[i] = transform.localToWorldMatrix * _transformVector;
        }

        _trail.SetPositions(_positions);
    }
}