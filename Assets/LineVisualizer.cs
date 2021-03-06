﻿    using UnityEngine;
using System.Collections;
    using System.Collections.Generic;
    using UnityEngine.UI;

public class LineVisualizer : MonoBehaviour
{
    public Text LineNumber;
    public Dropdown Command;
    public Dropdown[] Params;

    private DroneEditor _editor;
    private int _number;
    private CodeLine _codeLine;

    private static Dropdown _selected;
    private static Dropdown Selected
    {
        get { return _selected; }
        set
        {
            if (_selected != null && _selected.captionText.text.EndsWith("_"))
            {
                _selected.captionText.text = _selected.captionText.text.Substring(0,
                    _selected.captionText.text.Length - 1);
            }
            _selected = value;
        }
    }

    // Use this for initialization
    void Start ()
    {
        _editor = GetComponentInParent<DroneEditor>();
    }

    public void Setup(CodeLine line, int number)
    {
        _codeLine = line;
        if (line == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        _number = number;
        LineNumber.text = number.ToString("D2");

        var cmds = new List<Dropdown.OptionData>();
        foreach (CodeLine.Instruction cmd in System.Enum.GetValues(typeof(CodeLine.Instruction)))
        {
            cmds.Add(new Dropdown.OptionData(CodeLine.PrettyName(cmd)));
        }
        Command.options = cmds;
        Command.value = (int)line.Cmd;


        SetupParam(0);
        SetupParam(1);
    }

    private void SetupParam(int i)
    {
        if (_codeLine.ParamCount() > i)
        {
            Params[i].gameObject.SetActive(true);
            var options = new List<Dropdown.OptionData>();
            options.Add(new Dropdown.OptionData(Mathf.Approximately(_codeLine[i], 0) ? "0" : _codeLine[i].ToString("F")));
            int selected = 0;
            for (int j = 0; j < _editor.Drone.Registers.Count; j++)
            {
                var register = _editor.Drone.Registers[j];
                options.Add(new Dropdown.OptionData(register.Label));
                if (_codeLine.Params[i] == register.Label)
                    selected = j + 1;
            }
            Params[i].options = options;
            Params[i].value = selected;
        }
        else
        {
            Params[i].gameObject.SetActive(false);
        }
    }

    void OnGUI()
    {
        if (!Selected || Selected.value > 0 || (Selected != Params[0] && Selected != Params[1]))
            return;

        Event e = Event.current;
        if (e.type == EventType.keyDown)
        {
            var current = Selected.options[0].text;
            if (e.functionKey && current.Length > 0)
            {
                switch (e.keyCode)
                {
                    case KeyCode.Backspace:
                        current = current.Substring(0, current.Length - 1);
                        break;
                }
            }
            else if (char.IsDigit(e.character))
            {
                current += e.character;
            }
            else if (!current.Contains(".") && (e.character == '.' || e.character == ','))
            {
                current += '.';
            }
            else if (e.character == '-' && current == "")
            {
                current += e.character;
            }

            if (Selected.options[0].text != current)
            {
                _codeLine.Params[Selected == Params[0] ? 0 : 1] = current;
                Selected.options[0].text = current;
                Selected.captionText.text = current;
            }
        }

        Selected.captionText.text = _codeLine.Params[Selected == Params[0] ? 0 : 1] + (Time.time % 1 < 0.5f ? "" : "_");
    }

    void Update()
    {
        if (_number == _editor.Drone.Pointer)
        {
            LineNumber.color = Color.green;
        }
        else
        {
            LineNumber.color = Color.black;
        }
    }

    public void OnCommandChanged(int command)
    {
        Selected = null;
        if ((int)_codeLine.Cmd == command)
            return;
        _codeLine.Cmd = (CodeLine.Instruction) command;
        Setup(_codeLine, _number);
    }

    public void OnParamOneChanged(int param)
    {
        _codeLine.Params[0] = Params[0].options[param].text;
    }

    public void OnParamOneSelected()
    {
        if (Params[0].value == 0)
            Selected = Params[0];
    }

    public void OnParamTwoChanged(int param)
    {
        _codeLine.Params[1] = Params[1].options[param].text;
    }

    public void OnParamTwoSelected()
    {
        if (Params[1].value == 0)
            Selected = Params[1];
    }
}
