using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;

[ExecuteInEditMode]
public class DebugUI : MonoBehaviour
{
    public string variableName;
    
    public FloatReference myFloat;
    public bool showFloat;
    
    public IntReference myInt;
    public bool showInt;
    
    public StringReference myString;
    public bool showString = false;
    
    private TextMeshProUGUI _container;

    private void Start()
    {
        _container = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (showFloat)
            _container.text = variableName + " " + myFloat.Value;
        
        if (showInt)
            _container.text = variableName + " " + myInt.Value;
        
        if (showString)
            _container.text = variableName + " " + myString.Value;
    }
}
