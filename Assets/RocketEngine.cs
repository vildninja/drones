using UnityEngine;
using System.Collections;

public class RocketEngine : BaseRegister
{
    public float Power = 3;
    public float Emission = 300;
    public float MaxPower = 10;

    private Rigidbody2D _body;
    private ParticleSystem _particles;

    void Start()
    {
        _body = GetComponentInParent<Rigidbody2D>();
        _particles = GetComponentInChildren<ParticleSystem>();
    }

	// Update is called once per frame
	void FixedUpdate ()
	{
	    _body.AddForceAtPosition(transform.up*Mathf.Clamp(Power*Number, 0, MaxPower), transform.position);
	    if (_particles)
	    {
	        _particles.emissionRate = Emission*Mathf.Clamp(Power*Number, 0, MaxPower)/MaxPower;
	    }
	}
}
