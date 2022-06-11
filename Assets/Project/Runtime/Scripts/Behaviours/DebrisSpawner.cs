using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class DebrisSpawner : MonoBehaviour
{
    
    // References
    public Transform boatPosition;
    public List<GameObject> debris;
    public List<GameObject> floaters;

    // Properties
    public float minStepsAhead = 100;
    public float maxStepsAhead = 250;
    public float minHeight = 30;
    public float maxHeight = 200;
    public float coneAngleLimit = 100;
    public float debrisSpawnChance;
    public float floaterSpawnChance;
    public float distanceToCheck;
    public int spawnLimit = 20;
    
    // State
    private Vector3 _prevPos;
    private float _distanceAccumulated;
    private List<GameObject> _spawnedObjects;
    private bool _active;
    private int _numberOfEntriesSpawned;

    private void Start()
    {
        _prevPos = boatPosition.position;
        _spawnedObjects = new List<GameObject>();
        _active = true;
        EventManager.Instance.Register<TutorialFinish>((e) => { _active = true;});
        EventManager.Instance.Register<StartTutorial>((e) => { _active = false;});
    }

    private void Update()
    {
        if (_active)
        {
            _distanceAccumulated += Vector3.Distance(_prevPos, boatPosition.position);
            _prevPos = boatPosition.position;

            if (_distanceAccumulated > distanceToCheck)
            {
                float prob = Random.Range(0f, 1f);
                if ( prob < debrisSpawnChance)
                {
                    SpawnDebrisSky();
                }
                else if (prob < floaterSpawnChance)
                {
                    SpawnFloaterAtSea();
                }
                _distanceAccumulated -= distanceToCheck;
            } 
        }
    }

    public void SpawnDebrisSky()
    {
        Vector3 spawnPos = boatPosition.forward.normalized * Random.Range(minStepsAhead,maxStepsAhead);
        spawnPos =  Quaternion.Euler(0, Random.Range(-coneAngleLimit,coneAngleLimit), 0) * spawnPos;
        spawnPos.y += Random.Range(minHeight, maxHeight);
        spawnPos += boatPosition.position;
        _spawnedObjects.Add(Instantiate(debris[Random.Range(0, debris.Count)], spawnPos, Quaternion.identity, transform));
        if (_spawnedObjects.Count > spawnLimit)
        {
            Destroy(_spawnedObjects[0]);
            _spawnedObjects.RemoveAt(0);
        }
    }

    public void SpawnFloaterAtSea()
    {
        Vector3 spawnPos = boatPosition.forward.normalized * Random.Range(minStepsAhead,maxStepsAhead);
        spawnPos =  Quaternion.Euler(0, Random.Range(-coneAngleLimit,coneAngleLimit), 0) * spawnPos;
        spawnPos.y = 0;
        spawnPos += boatPosition.position;
        GameObject floater = Instantiate(floaters[Random.Range(0, floaters.Count)], spawnPos, Quaternion.identity,
            transform);
        _spawnedObjects.Add(floater);
        if (_spawnedObjects.Count > spawnLimit)
        {
            Destroy(_spawnedObjects[0]);
            _spawnedObjects.RemoveAt(0);
        }
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(DebrisSpawner))]
public class DrawDebrisSpawner : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DebrisSpawner spawner = (DebrisSpawner)target;
        if (GUILayout.Button("Spawn Debris"))
        {
            spawner.SpawnDebrisSky();
        }
        
        if (GUILayout.Button("Spawn Floater"))
        {
            spawner.SpawnFloaterAtSea();
        }
    }
}
#endif