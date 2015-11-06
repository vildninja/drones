    using UnityEngine;
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
        LineNumber.text = (number + 1).ToString("D2");

        var cmds = new List<Dropdown.OptionData>();
        foreach (var cmd in System.Enum.GetNames(typeof(CodeLine.Instruction)))
        {
            cmds.Add(new Dropdown.OptionData(cmd));
        }
        Command.options = cmds;


        SetupParam(0);
        SetupParam(1);
    }

    private void SetupParam(int i)
    {
        if (_codeLine.ParamCount() > i)
        {
            Params[i].gameObject.SetActive(true);
            var options = new List<Dropdown.OptionData>();
            options.Add(new Dropdown.OptionData(_codeLine[i].ToString("F")));
            foreach (var register in _editor.Drone.Registers)
            {
                options.Add(new Dropdown.OptionData(register.Label));
            }
            Params[i].options = options;
        }
        else
        {
            Params[i].gameObject.SetActive(false);
        }
    }

    void OnGUI()
    {
        if (!_selected || _selected.value > 0 || (_selected != Params[0] && _selected != Params[1]))
            return;

        Event e = Event.current;
        if (e.type == EventType.keyDown)
        {
            var current = _selected.options[0].text;
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

            if (_selected.options[0].text != current)
            {
                _codeLine.Params[_selected == Params[0] ? 0 : 1] = current;
                _selected.options[0].text = current;
                _selected.captionText.text = current;
            }
        }
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
        _selected = null;
        _codeLine.Cmd = (CodeLine.Instruction) command;
        Setup(_codeLine, _number);
    }

    public void OnParamOneChanged(int param)
    {
        if (param == 0)
            _selected = Params[0];
        _codeLine.Params[0] = Params[0].options[param].text;
    }

    public void OnParamTwoChanged(int param)
    {
        if (param == 0)
            _selected = Params[1];
        _codeLine.Params[1] = Params[1].options[param].text;
    }
}
