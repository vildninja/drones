using UnityEngine;
using System.Collections;

public class VolumeSetter : BaseRegister
{
    [SerializeField]
    private AudioSource _source;
    public float Range = 10;

    public override float Number
    {
        get { return base.Number; }
        set
        {
            base.Number = value;
            if (_source)
                _source.volume = value/Range;
        }
    }

    void Start()
    {
        Number = 0;
    }
}
