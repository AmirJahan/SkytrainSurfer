using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(Gonk))]
public class GonkDrawer : DecoratorDrawer
{
    Gonk gonkAttribute => ((Gonk)attribute);

    public override float GetHeight()
    {
        return base.GetHeight() + gonkAttribute.Scale;
    }

    public override void OnGUI(Rect position)
    {
        EditorGUI.DrawPreviewTexture(new Rect(0, 0, gonkAttribute.Scale, gonkAttribute.Scale), gonkAttribute.Image);
    }
}