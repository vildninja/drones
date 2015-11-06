using UnityEngine;
using System.Collections;

public class ProximitySensor : BaseRegister
{

    public float MaxDistance;

	// Update is called once per frame
	void FixedUpdate ()
	{
	    var near = Physics2D.OverlapCircleAll(transform.position, MaxDistance);
	    float min = MaxDistance;
	    for (int i = 0; i < near.Length; i++)
	    {
            if (near[i].transform.root == transform.root)
                continue;

	        float d = Vector2.Distance(transform.position, near[i].transform.position);
	        if (min > d)
	        {
	            min = d;
	        }
	    }

	    Number = min;
	}
}
