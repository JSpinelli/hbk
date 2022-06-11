using UnityEngine;

public class Visor : MonoBehaviour
{
    // Properties
    public float sizeOfRay;
    public float maxDistance;

    // Start Init
    private Camera _cam;

    // State
    private JournalEntry _currentEntry;
    private int _objectID;
    private bool _visorEnabled;

    private void Start()
    {
        _cam = GetComponent<Camera>();
        EventManager.Instance.Register<VisorEnabled>((e) => { _visorEnabled = true;});
        EventManager.Instance.Register<GameStarted>((e) => { _visorEnabled = true;});
        EventManager.Instance.Register<StartTutorial>((e) => { _visorEnabled = false;});
    }

    private void Update()
    {
        if (!_visorEnabled) return;
        RaycastHit hit;
        Vector3 origin = _cam.ScreenToWorldPoint(Vector3.zero);
        if (Physics.SphereCast(origin, sizeOfRay / 2, _cam.transform.forward, out hit, maxDistance,LayerMask.GetMask("StoryBeacons")))
        {
            if (hit.transform.CompareTag("StoryBeacon"))
            {
                if (_objectID != hit.transform.gameObject.GetInstanceID())
                {
                    _objectID = hit.transform.gameObject.GetInstanceID();
                    _currentEntry = hit.transform.GetComponent<StoryBeacon>().GetEntry();
                    EventManager.Instance.Fire(new ObjectFound(_currentEntry));
                }
            }
            else
            {
                _objectID = 0;
                _currentEntry = null;
                EventManager.Instance.Fire(new ObjectLost());
            }
        }
        else
        {
            _objectID = 0;
            _currentEntry = null;
            EventManager.Instance.Fire(new ObjectLost());
        }
    }
}