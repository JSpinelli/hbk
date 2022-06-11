#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class WindZoneSpawner : MonoBehaviour
{
    // State
    private GameObject _zoneHolder;
    
    // References
    public Texture2D flowmap;
    
    // Properties
    public GameObject windZonePrefab;
    public int amountXTiles;
    public int amountZTiles;
    public int tileSize;
    public int textureScaling;
    public float magnitudeMultiplier;
    
    public void PlaceTiles()
    {
        DestroyTiles();
        _zoneHolder = new GameObject("Wind Zones");
        _zoneHolder.transform.parent = transform;
        var total = 0;
        
        float positionX = (-amountXTiles * tileSize) / 2;
        for (int i = 0; i < amountXTiles; i++)
        {
            float positionZ = -(amountZTiles * tileSize)/ 2;
            for (int j = 0; j < amountZTiles; j++)
            {
                GameObject zone = Instantiate(windZonePrefab, new Vector3(positionX, 0, positionZ), Quaternion.identity, _zoneHolder.transform);
                zone.name = "Zone " + total;
                Color flowMapValue = flowmap.GetPixel(i*textureScaling, j*textureScaling);
                Vector2 windValue = new Vector2(flowMapValue.r, flowMapValue.g)*magnitudeMultiplier;
                WindArea windArea = zone.GetComponent<WindArea>();
                windArea.SetUp(windValue);
                positionZ += tileSize;
                total++;
            }
            positionX += tileSize;
        }
    }

    public void DestroyTiles()
    {
        if (_zoneHolder == null) return;
        DestroyImmediate(_zoneHolder);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(WindZoneSpawner))]
public class DrawSeaManager : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WindZoneSpawner manager = (WindZoneSpawner) target;
        if (GUILayout.Button("Position Wind Zones"))
        {
            manager.PlaceTiles();
        }
        
        if (GUILayout.Button("Destroy Zones"))
        {
            manager.DestroyTiles();
        }
    }
}
#endif
