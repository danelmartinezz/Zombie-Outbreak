using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestor de máquinas de estados de inteligencia artificial en un juego.
/// </summary>
public class GameSceneManager : MonoBehaviour
{
    // Static
    private static GameSceneManager _instance = null;
    public static GameSceneManager Instance 
    {
        get
        {
            if(_instance == null)
                _instance = (GameSceneManager)FindObjectOfType(typeof(GameSceneManager));
            return _instance;
        }
    }

    // Private
    private Dictionary<int, AIStateMachine> _stateMachines = new Dictionary<int, AIStateMachine>();

    // Public Methods    
    /// <summary>
    /// Registra una máquina de estados con una clave.
    /// </summary>
    /// <param name="key">La clave asociada con la máquina de estados.</param>
    /// <param name="stateMachine">La máquina de estados a registrar.</param>
    public void RegisterAIStateMachine(int key, AIStateMachine stateMachine)
    {
        // Si no se encuentra en el diccionario
        if(!_stateMachines.ContainsKey(key))
        {
            // Añade el estado al diccionario
            _stateMachines[key] = stateMachine;
        }
    }


    /// <summary>
    /// Obtiene una máquina de estados asociada con el clave dado.
    /// </summary>
    /// <param name="key">La clave de la máquina de estados.</param>
    /// <returns>La máquina de estados asociada con la clave o null si no se encuentra.</returns>
    public AIStateMachine GetAIStateMachine(int key)
    {
        AIStateMachine machine = null;
        // Si se encuentra en el diccionario 
        if(_stateMachines.TryGetValue(key, out machine))
        {
            // Devuelve la maquina de estados
            return machine;
        }
        return null;
    }
}
