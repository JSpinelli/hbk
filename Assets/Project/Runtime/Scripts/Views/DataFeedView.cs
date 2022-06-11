using Shapes;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class DataFeedView : MonoBehaviour
{
    //Globals
    public IntReference speed;
    public FloatReference mainSailEfficiency;
    public FloatReference frontSailEfficiency;
    public StringReference pointOfSailing;
    
    // References
    [Header("Boat Manager")]
    public BoatController bm;
    
    [Header("Speedometer")]
    public Disc speedometer;
    public GameObject speedometerBackground;
    
    [Header("Efficiency")]
    public Disc mainSailEff;
    public GameObject mainSailEffObj;
    public Disc frontSailEff;
    public GameObject frontSailEffObj;
    
    [Header("Wind")]
    public Transform windIndicator;
    public GameObject windObj;

    [Header("Boat Lines")]
    public GameObject tillerShape;
    public GameObject mainSailShape;
    public GameObject frontSailShape;
    public GameObject sailBoatShape;
    
    [Header("Point of Sailing Indicator")]
    public TextMeshPro pointOfSailingText;
    
    [Header("Anchor")]
    public GameObject anchor;

    private bool _updateAnchor = true;
    
    private void Start()
    {
        EventManager.Instance.Register<StartTutorial>((e) =>
        {
            tillerShape.SetActive(false);
            mainSailShape.SetActive(false);
            frontSailShape.SetActive(false);
            speedometerBackground.SetActive(false);
            mainSailEffObj.SetActive(false);
            frontSailEffObj.SetActive(false);
            windObj.SetActive(false);
            sailBoatShape.SetActive(false);
            anchor.SetActive(false);
            _updateAnchor = false;
        });
        
        EventManager.Instance.Register<SetTutorialActive>((e) =>
        {
            SetTutorialActive sta = (SetTutorialActive) e;
            switch (sta.Input)
            {
                case "tiller":
                {
                    tillerShape.SetActive(sta.Enable);
                    break;
                }                
                case "boat":
                {
                    sailBoatShape.SetActive(sta.Enable);
                    break;
                }
                case "frontSail":
                {
                    frontSailShape.SetActive(sta.Enable);
                    frontSailEffObj.SetActive(sta.Enable);
                    break;
                }
                case "mainSail":
                {
                    mainSailShape.SetActive(sta.Enable);
                    mainSailEffObj.SetActive(sta.Enable);
                    break;
                }
                case "wind":
                {
                    windObj.SetActive(sta.Enable);
                    break;
                }
                case "anchor":
                {
                    _updateAnchor = true;
                    anchor.SetActive(sta.Enable);
                    break;
                }                
                case "speed":
                {
                    speedometerBackground.SetActive(sta.Enable);
                    break;
                }
            }
        });
    }

    private void Update()
    {
        // Rotate Wind to match
        Vector3 wind = new Vector3(WindManager.Instance.wind.x, 0, WindManager.Instance.wind.y);
        //windIndicator.LookAt(wind,Vector3.up);
        Vector3 newDirection = Vector3.RotateTowards(windIndicator.right, wind,500,500);
        windIndicator.right = wind;
        
        // Update Speedometer
        speedometer.DashSize = speed / 100;
        speedometer.AngRadiansEnd = Map(0.2f, 5.4f, 0, 7500, speed);

        //Update main sail efficency
        mainSailEff.DashSize = mainSailEfficiency * 10;
        mainSailEff.AngRadiansEnd = Map(0, 3, 0, 1, mainSailEfficiency);
        
        //Update front sail efficency
        frontSailEff.DashSize = frontSailEfficiency * 10;
        frontSailEff.AngRadiansEnd = Map(0, 3, 0, 1, frontSailEfficiency);
        
        // Point Of Sailing
        pointOfSailingText.text = pointOfSailing.Value;

        if (_updateAnchor)
            anchor.SetActive(bm.anchorDropped);
    }
    
    public float Map(float from, float to, float from2, float to2, float value){
        if(value <= from2){
            return from;
        }else if(value >= to2){
            return to;
        }else{
            return (to - from) * ((value - from2) / (to2 - from2)) + from;
        }
    }

}
