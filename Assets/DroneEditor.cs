using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class DroneEditor : MonoBehaviour
{
    public BaseDrone Drone;
    
    private List<RegisterVisualizer> _registers;
    private List<LineVisualizer> _lines;

    public Text DroneName;
    public Text DroneStory;
    public ScrollRect Scroll;

    private void Start()
    {
        _registers = new List<RegisterVisualizer>(GetComponentsInChildren<RegisterVisualizer>());
        _lines = new List<LineVisualizer>(GetComponentsInChildren<LineVisualizer>());
        CloseEditor();
    }

    public void CloseEditor()
    {
        SelectDrone(null);
    }

    public void SelectDrone(BaseDrone drone)
    {
        // unset last
        if (Drone)
        {
            foreach (var oldRegister in Drone.Registers)
            {
                oldRegister.Setup(null);
            }
            foreach (var oldLines in _lines)
            {
                oldLines.gameObject.SetActive(false);
            }
        }

        foreach (var visual in _registers)
        {
            visual.gameObject.SetActive(false);
        }
        foreach (var line in _lines)
        {
            line.gameObject.SetActive(false);
        }

        Drone = drone;

        if (!Drone)
        {
            GetComponent<Canvas>().enabled = false;

            return;
        }

        GetComponent<Canvas>().enabled = true;

        DroneName.text = Drone.name;
        DroneStory.text = Drone.Story;

        var outputs = _registers.Where(r => r.Type == RegisterType.OUTPUT).GetEnumerator();
        var registers = _registers.Where(r => r.Type == RegisterType.REGISTER).GetEnumerator();
        var sensors = _registers.Where(r => r.Type == RegisterType.SENSOR).GetEnumerator();

        foreach (var register in drone.Registers)
        {
            RegisterVisualizer visual = null;
            switch (register.Type)
            {
                case RegisterType.OUTPUT:
                    outputs.MoveNext();
                    visual = outputs.Current;
                    break;
                case RegisterType.SENSOR:
                    sensors.MoveNext();
                    visual = sensors.Current;
                    break;
                case RegisterType.REGISTER:
                    registers.MoveNext();
                    visual = registers.Current;
                    break;
            }

            if (visual != null)
            {
                visual.Label.text = register.Label;
                register.Setup(visual.Content);
                visual.gameObject.SetActive(true);
            }
        }

        for (int i = 0; i < Drone.Lines.Count; i++)
        {
            if (_lines.Count == i)
            {
                var next = Instantiate(_lines[0]);
                next.transform.SetParent(_lines[0].transform.parent, false);
                _lines.Add(next);
            }
            _lines[i].Setup(Drone.Lines[i], i);
        }


    }

    public void RestartDrone()
    {
        if (Drone)
        {
            Drone.Restart();
        }
    }

    public void PauseDrone()
    {
        if (Drone)
        {
            Drone.Running = !Drone.Running;
        }
    }

    public void StepDrone()
    {
        if (Drone)
        {
            Drone.Step = true;
        }
    }

    // select drone on click
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Drone == null)
        {
            SearchForDrone();
        }

        var ray = Camera.main.ScreenPointToRay(new Vector2());
        float multiply = -ray.origin.z / ray.direction.z;
        Vector2 min = ray.direction * multiply + ray.origin;
        ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width, Screen.height));
        Vector2 max = ray.direction * multiply + ray.origin;

        Bounds bounds = new Bounds(Vector3.zero, Vector3.one);
        bounds.Encapsulate(min);
        bounds.Encapsulate(max);
        bounds.size += new Vector3(1, 1);

        foreach (var drone in FindObjectsOfType<BaseDrone>())
        {
            var pos = drone.transform.position;
            if (bounds.Contains(pos))
                continue;
            
            if (pos.x < bounds.min.x)
                pos.x = bounds.max.x;
            if (pos.x > bounds.max.x)
                pos.x = bounds.min.x;
            if (pos.y < bounds.min.y)
                pos.y = bounds.max.y;
            if (pos.y > bounds.max.y)
                pos.y = bounds.min.y;

            drone.transform.position = pos;
        }
    }

    private void SearchForDrone()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float multiply = -ray.origin.z/ray.direction.z;
        var world = ray.direction * multiply + ray.origin;
        var hit = Physics2D.OverlapPoint(world);
        if (hit == null)
            return;

        var drone = hit.transform.root.GetComponent<BaseDrone>();
        if (drone)
        {
            SelectDrone(drone);
        }
    }
}
