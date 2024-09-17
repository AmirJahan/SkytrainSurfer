using UnityEngine;
using System.Collections;
using UnityEditor;

public class Gonk : PropertyAttribute
{
    public readonly Texture2D Image;
    public readonly float Scale;

    public Gonk(float scale)
    {
       Image = Resources.Load<Texture2D>("gonk");
       Scale = scale;
    }
}
