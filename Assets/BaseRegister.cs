using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum RegisterType
{
    OUTPUT, SENSOR, REGISTER
}

public class BaseRegister : MonoBehaviour
{
    private float _number;

    public RegisterType Type;
    public string Label;

    public virtual float Number
    {
        get { return _number; }
        set
        {
            _number = Mathf.Clamp(value, -10000, 10000);
            if (_text)
            {
                _text.text = value.ToString("F");
            }
        }
    }

    private Text _text;

    public void Setup(Text text)
    {
        _text = text;
        if (_text)
        {
            _text.text = _number.ToString("F");
        }
    }
}
