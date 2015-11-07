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
        ELSE,
        END_IF,
        ASIGN,
        GOTO,
        ADD,
        SUBTRACT,
        MULTIPLY,
        DIVIDE,
    }

    public enum IfResult
    {
        NONE,
        START_FALSE,
        START_TRUE,
        ELSE,
        END,
    }

    public Instruction Cmd;
    public readonly string[] Params = new[] {"0", "0"};
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
        }
        return 0;
    }

    public IfResult Execute(ref int pointer)
    {
        switch (Cmd)
        {
            case Instruction.NO_OP:
                pointer++;
                break;
            case Instruction.IF_EQUAL:
                pointer++;
                if (Mathf.Abs(this[0] - this[1]) < 0.01f)
                    return IfResult.START_TRUE;
                return IfResult.START_FALSE;
            case Instruction.IF_GREATER:
                pointer++;
                if (this[0] > this[1])
                    return IfResult.START_TRUE;
                return IfResult.START_FALSE;
            case Instruction.IF_LESS:
                pointer++;
                if (this[0] < this[1])
                    return IfResult.START_TRUE;
                return IfResult.START_FALSE;
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
            case Instruction.ELSE:
                pointer++;
                return IfResult.ELSE;
            case Instruction.END_IF:
                pointer++;
                return IfResult.END;
            default:
                pointer++;
                break;
        }
        return IfResult.NONE;
    }

    public static string PrettyName(Instruction cmd)
    {
        switch (cmd)
        {
            case Instruction.NO_OP:
                return "";
            case Instruction.IF_EQUAL:
                return "if a = b";
            case Instruction.IF_GREATER:
                return "if a > b";
            case Instruction.IF_LESS:
                return "if a < b";
            case Instruction.ELSE:
                return "else";
            case Instruction.END_IF:
                return "end if";
            case Instruction.ASIGN:
                return "asign a <- b";
            case Instruction.GOTO:
                return "goto line";
            case Instruction.ADD:
                return "calc a + b";
            case Instruction.SUBTRACT:
                return "calc a - b";
            case Instruction.MULTIPLY:
                return "calc a * b";
            case Instruction.DIVIDE:
                return "calc a / b";
        }
        return "";
    }
}
