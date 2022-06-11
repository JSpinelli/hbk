using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorChanger : MonoBehaviour
{
    public List<TextMeshProUGUI> texts;
    public List<Image> images;

    public void ChangeColor(Color color)
    {
        foreach (var text in texts)
        {
            text.color = color;
        }

        foreach (var img in images)
        {
            img.color = color;
        }
    }

}
