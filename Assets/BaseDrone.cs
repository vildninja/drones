using System;
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
    private readonly Stack<bool> _ifJump = new Stack<bool>(); 

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
            {
                Restart();
            }
            yield return new WaitForSeconds(0.1f);
            if (!Running && !Step)
            {
                continue;
            }
            
            var res = Lines[Pointer].Execute(ref Pointer);
            switch (res)
            {
                case CodeLine.IfResult.START_FALSE:
                    _ifJump.Push(true);
                    break;
                case CodeLine.IfResult.START_TRUE:
                    _ifJump.Push(false);
                    break;
                case CodeLine.IfResult.ELSE:
                    if (_ifJump.Count > 0)
                        _ifJump.Push(!_ifJump.Pop());
                    break;
                case CodeLine.IfResult.END:
                    if (_ifJump.Count > 0)
                        _ifJump.Pop();
                    break;
            }

            while (_ifJump.Count > 0 && _ifJump.Peek() && Pointer < Lines.Count)
            {
                if (Lines[Pointer].Cmd == CodeLine.Instruction.ELSE)
                {
                    _ifJump.Push(!_ifJump.Pop());
                }
                else if (Lines[Pointer].Cmd == CodeLine.Instruction.END_IF)
                {
                    _ifJump.Pop();
                }
                Pointer++;
            }
        }
    }

    public void Restart()
    {
        _ifJump.Clear();
        Pointer = 0;
    }
}
