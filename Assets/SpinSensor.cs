using UnityEngine;
using System.Collections;

public class SpinSensor : BaseRegister {


    private Rigidbody2D _body;

    void Start()
    {
        _body = GetComponentInParent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        Number = _body.angularVelocity;
    }
}
