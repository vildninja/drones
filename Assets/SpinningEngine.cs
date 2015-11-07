using UnityEngine;
using System.Collections;

public class SpinningEngine : BaseRegister {

    public float Power = 1;
    public float MaxPower = 10;

    private Rigidbody2D _body;

    void Start()
    {
        _body = GetComponentInParent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        _body.AddTorque(Mathf.Clamp(Number * Power, -MaxPower, MaxPower));
    }
}
