using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WindArea : MonoBehaviour
{
    //Properties
    [HeaderAttribute("Wind Zone Properties")]
    public Vector2 wind;
    public float magnitude;
    public float timeToChange;
    public float forceFieldOverlap = 100;
    
    // State
    private bool _windZoneTriggered = false;
    
    //References
    public GameObject windIndicator;
    public ParticleSystemForceField windForceField;
    public BoxCollider boxCollider;

    private void Start()
    {
        UpdateWindIndicator();
    }

    public void SetUp(Vector2 wind)
    {
        magnitude = wind.magnitude;
        this.wind = wind.normalized;
        windIndicator.transform.forward = new Vector3(wind.x, 0, wind.y);
        windIndicator.transform.localScale = new Vector3(windIndicator.transform.localScale.x,windIndicator.transform.localScale.y, magnitude/5);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_windZoneTriggered)
        {
            _windZoneTriggered = true;
            WindManager.Instance.SetWind(wind,magnitude,timeToChange);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && _windZoneTriggered)
        {
            _windZoneTriggered = false;
        }
    }

    public void UpdateWindIndicator()
    {
        wind = new Vector2(windIndicator.transform.forward.x, windIndicator.transform.forward.z).normalized;
        windIndicator.transform.localScale = new Vector3(windIndicator.transform.localScale.x,windIndicator.transform.localScale.y, magnitude/5);
        windForceField.endRange = ( boxCollider.size.x / 2 ) + forceFieldOverlap;
        windForceField.directionX = wind.x * magnitude;
        windForceField.directionZ = wind.y * magnitude;
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(WindArea))]
public class DrawWindZoneBehaviour: Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WindArea manager = (WindArea)target;
        if(GUILayout.Button("Update Wind Zone"))
        {
            manager.UpdateWindIndicator();
        }
    }
}
#endif
