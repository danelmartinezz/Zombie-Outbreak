using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

[CustomEditor(typeof(AIWaypointNetwork))]
public class AIWaypointNetworkEditor : Editor
{    
    private void OnSceneGUI()
    {
        AIWaypointNetwork network = (AIWaypointNetwork)target;

        // Configuramos el estilo del Texto
        GUIStyle style = new GUIStyle();
        style.fontSize = 12;
        style.fontStyle = FontStyle.Bold;  
        style.normal.textColor = Color.white;

        // Se pinta en pantalla los nombres de los waypoints e indices que pertenecen.
        for (int i = 0; i < network.Waypoints.Count; i++)
        {       
            if(network.Waypoints[i] != null)
            {
                Handles.Label(network.Waypoints[i].position, $"Waypoint {i}", style);
            }            
        }

        // Se pinta de un color la linea
        Color color = new Color(network.colorLine.r, network.colorLine.g, network.colorLine.b);
        Handles.color = color;

        // Si es por conexiones
        if (network.displayMode == PathDisplayMode.Connetions)
        {
            // Declaramos un array de vector para dibujar las líneas en pantalla.
            // Colocamos +1 para cerrar la línea
            Vector3[] linePoints = new Vector3[network.Waypoints.Count + 1];

            // Recorremos los waypoints
            for (int i = 0; i <= network.Waypoints.Count; i++)
            {
                // Asignamos el índice mientras no sea igual a la longitud de la lista.
                int index = i != network.Waypoints.Count ? i : 0;

                // Si la posición no es nulo
                if (network.Waypoints[index] != null)
                {
                    // Se almacena la posición del waypoint actual en el array de puntos para dibujar la línea.
                    linePoints[i] = network.Waypoints[index].position;
                }
                else
                {
                    // Si el waypoint no es válido, se asigna un marcador especial (Infinity) para omitirlo en el dibujo de la línea.
                    linePoints[i] = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
                }

            }
            
            // Dibujamos las líneas
            Handles.DrawPolyLine(linePoints);
        }
        // Si es por caminos
        else if(network.displayMode == PathDisplayMode.Paths)
        {
            // Se declara para almacenar el cálculo del camino entre dos posiciones en el NavMesh.
            NavMeshPath path = new NavMeshPath();

            if (network.Waypoints[network.uiStart] != null &&  network.Waypoints[network.uiEnd] != null)
            {
                Vector3 from = network.Waypoints[network.uiStart].position;
                Vector3 to = network.Waypoints[network.uiEnd].position;

                // Se calcula el camino en el NavMesh desde el punto 'from' hasta el punto 'to',
                NavMesh.CalculatePath(from, to, NavMesh.AllAreas, path);                

                // Dibujamos la linea de 'from' hasta 'to'
                Handles.DrawPolyLine(path.corners);
            }            
        }        
    }

    public override void OnInspectorGUI()
    {        
        AIWaypointNetwork network = (AIWaypointNetwork)target;

        network.displayMode = (PathDisplayMode)EditorGUILayout.EnumPopup("Display Mode", network.displayMode);        
        // Si es el display es por Paths
        if (network.displayMode == PathDisplayMode.Paths)
        {
            // Mostramos los campos en el inspector
            network.uiStart = EditorGUILayout.IntSlider("Waypoint Start", network.uiStart, 0, network.Waypoints.Count - 1);
            network.uiEnd = EditorGUILayout.IntSlider("Waypoint End", network.uiEnd, 0, network.Waypoints.Count - 1);
        }
        // Dibuja el inspector con los campos predeterminados de la clase, sin modificaciones adicionales.
        DrawDefaultInspector();        
    }
}
