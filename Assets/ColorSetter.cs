using UnityEngine;
using System.Collections;

public class ColorSetter : BaseRegister
{
    public Gradient Colors;
    public float Range = 10;

    public override float Number
    {
        get { return base.Number; }
        set
        {
            base.Number = value;
            var color = Colors.Evaluate((value%Range)/Range);
            foreach (var child in GetComponentsInChildren<Renderer>())
            {
                child.material.color = color;
            }
        }
    }

    void Start()
    {
        Number = 0;
    }
}
