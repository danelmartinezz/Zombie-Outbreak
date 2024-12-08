using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PathDisplayMode { None, Connetions, Paths }


public class AIWaypointNetwork : MonoBehaviour
{
    [HideInInspector]
    public PathDisplayMode displayMode = PathDisplayMode.Connetions;
    public Color colorLine;
    [HideInInspector]
    public int uiStart = 0;
    [HideInInspector]
    public int uiEnd = 0;
    public List<Transform> Waypoints = new List<Transform>();
}
