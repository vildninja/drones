using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CodeLine
{

    private BaseDrone _drone;

    public CodeLine(BaseDrone drone)
    {
        _drone = drone;
        Cmd = Instruction.NO_OP;
    }

    public enum Instruction
    {
        NO_OP,
        IF_EQUAL,
        IF_GREATER,
        IF_LESS,
        ASIGN,
        GOTO,
        ADD,
        SUBTRACT,
        MULTIPLY,
        DIVIDE,
        SLEEP,
        TRIGGER,
    }

    public Instruction Cmd;
    public readonly string[] Params = new[] {"0.00", "0.00"};
    private readonly BaseRegister[] _regs = new BaseRegister[2];



    public float this[int i]
    {
        get
        {
            if (_regs[i] && _regs[i].Label == Params[i])
                return _regs[i].Number;
            float res;
            if (float.TryParse(Params[i], out res))
                return res;
            var reg = _drone.Registers.FirstOrDefault(r => r.Label == Params[i]);
            if (reg != null)
            {
                _regs[i] = reg;
                return reg.Number;
            }
            return 0;
        }
        set
        {
            if (!_regs[i] || _regs[i].Label != Params[i])
                _regs[i] = _drone.Registers.FirstOrDefault(r => r.Label == Params[i]);
            if (_regs[i])
                _regs[i].Number = value;
        }
    }

    public void SetLine(string cmd, string param1, string param2)
    {
        try
        {
            Cmd = (Instruction) System.Enum.Parse(typeof (Instruction), cmd);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex, _drone);
            Cmd = Instruction.NO_OP;
        }
    }

    public int ParamCount()
    {
        switch (Cmd)
        {
            case Instruction.NO_OP:
                return 0;
            case Instruction.IF_EQUAL:
                return 2;
            case Instruction.IF_GREATER:
                return 2;
            case Instruction.IF_LESS:
                return 2;
            case Instruction.ASIGN:
                return 2;
            case Instruction.GOTO:
                return 1;
            case Instruction.ADD:
                return 2;
            case Instruction.SUBTRACT:
                return 2;
            case Instruction.MULTIPLY:
                return 2;
            case Instruction.DIVIDE:
                return 2;
            case Instruction.SLEEP:
                return 1;
            case Instruction.TRIGGER:
                return 1;
        }
        return 0;
    }

    public void Execute(ref int pointer)
    {
        switch (Cmd)
        {
            case Instruction.NO_OP:
                pointer++;
                break;
            case Instruction.IF_EQUAL:
                if (Mathf.Abs(this[0] - this[1]) > 0.01f)
                    pointer++;
                pointer++;
                break;
            case Instruction.IF_GREATER:
                if (this[0] < this[1])
                    pointer++;
                pointer++;
                break;
            case Instruction.IF_LESS:
                if (this[0] > this[1])
                    pointer++;
                pointer++;
                break;
            case Instruction.ASIGN:
                this[0] = this[1];
                pointer++;
                break;
            case Instruction.GOTO:
                pointer = (int)this[0];
                break;
            case Instruction.ADD:
                _drone.Registers[0].Number = this[0] + this[1];
                pointer++;
                break;
            case Instruction.SUBTRACT:
                _drone.Registers[0].Number = this[0] - this[1];
                pointer++;
                break;
            case Instruction.MULTIPLY:
                _drone.Registers[0].Number = this[0] * this[1];
                pointer++;
                break;
            case Instruction.DIVIDE:
                if (Mathf.Approximately(this[1], 0))
                    _drone.Registers[0].Number = this[0] / 0.001f;
                else
                    _drone.Registers[0].Number = this[0] / this[1];
                pointer++;
                break;
            case Instruction.SLEEP:
                pointer++;
                break;
            case Instruction.TRIGGER:
                pointer++;
                break;
            default:
                pointer++;
                break;
        }
    }
}
