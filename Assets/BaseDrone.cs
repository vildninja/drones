using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseDrone : MonoBehaviour
{
    public List<BaseRegister> Registers;
    public List<CodeLine> Lines;

    public int NumLines;
    public int Pointer;

    private bool _running = true;

    public bool Running
    {
        get { return _running; }
        set
        {
            _running = value;
            _step = false;
        }
    }

    private bool _step = false;

    public bool Step
    {
        get
        {
            bool tmp = _step;
            _step = false;
            return tmp;
        }
        set
        {
            _step = value;
            _running = false;
        }
    }

    public string Story = "Unknown";

	// Use this for initialization
	void Awake ()
	{
	    var otherRegisters = GetComponentsInChildren<BaseRegister>();
        var result = gameObject.AddComponent<BaseRegister>();
	    result.Label = "Result";
        result.Type = RegisterType.REGISTER;

        Registers = new List<BaseRegister>();
        Registers.Add(result);
        Registers.AddRange(otherRegisters);

        Lines = new List<CodeLine>();
	    for (int i = 0; i < NumLines; i++)
	    {
	        Lines.Add(new CodeLine(this));
	    }
	}

    IEnumerator Start()
    {
        while (true)
        {
            if (Pointer < 0 || Pointer >= Lines.Count)
                Pointer = 0;
            yield return new WaitForSeconds(0.1f);
            if (!Running && !Step)
            {
                continue;
            }

            Lines[Pointer].Execute(ref Pointer);
        }
    }
}
