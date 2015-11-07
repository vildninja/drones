using UnityEngine;
using System.Collections;

public class RandomRegister : BaseRegister
{

    public float Max = 10;

	// Update is called once per frame
	void Update ()
	{
	    Number = Random.Range(0, Max);
	}
}
