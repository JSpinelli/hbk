using System;
using Shapes;
using UnityEngine;

public class ShapesScaler : MonoBehaviour
{
    public RectTransform parentTransform;
    public Rectangle shapeToScale;

    public float verticalMargin;
    public float horizontalMargin;

    private void OnBecameVisible()
    {
        shapeToScale.Height = parentTransform.rect.height + verticalMargin;
        shapeToScale.Width = parentTransform.rect.width + horizontalMargin;
    }

    public void Scale()
    {
        shapeToScale.Height = parentTransform.rect.height + verticalMargin;
        shapeToScale.Width = parentTransform.rect.width + horizontalMargin; 
    }


}
