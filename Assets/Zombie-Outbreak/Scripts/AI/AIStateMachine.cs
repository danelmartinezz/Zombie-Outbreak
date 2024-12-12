using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Definición de los estados posibles de la IA
public enum AIStateType { None, Idle, Alert, Patrol, Attack, Feeding, Pursuit, Dead }

// Definición de los tipos de objetivos que la IA puede manejar
public enum AITargetType { None, Waypoint, Visual_Player, Visual_Light, Visual_Food, Audio }

// Define los tipos de eventos de activación que puede manejar la IA
public enum AITriggerEventType { None, Enter, Stay, Exit }

// Representa un objetivo que puede ser procesado por la IA
public struct AITarget
{
    private AITargetType _type;     // Tipo de objetivo (visual, sonoro, etc.)
    private Collider _collider;    // Collider asociado al objetivo
    private Vector3 _position;     // Posición actual del objetivo en el mundo
    private float _distance;       // Distancia al objetivo desde la IA
    private float _time;           // Momento en el tiempo en que se detectó por última vez

    // Propiedades para acceder a los datos del objetivo
    public AITargetType Type { get { return _type; } }
    public Collider Collider { get { return _collider; } }
    public Vector3 Position { get { return _position; } }
    public float Distance { get { return _distance; } set { _distance = value; } }
    public float LastTime { get { return _time; } }

    // Configura un nuevo objetivo con los datos proporcionados
    public void Set(AITargetType type, Collider collider, Vector3 position, float distance)
    {
        _type = type;
        _collider = collider;
        _position = position;
        _distance = distance;
        _time = Time.time; // Captura el momento actual
    }

    // Resetea los datos del objetivo
    public void Clear()
    {
        _type = AITargetType.None;
        _collider = null;
        _position = Vector3.zero;
        _distance = 0f;
        _time = Mathf.Infinity; // Marca como no detectado
    }
}

/// <summary>
/// Clase base abstracta para máquinas de estados de IA que manejan los diferentes estados
/// y objetivos de una entidad en el juego.
/// </summary>
public abstract class AIStateMachine : MonoBehaviour
{
    // Public
    // Objetivos detectados por amenazas visuales y sonoras
    public AITarget visualThreat = new AITarget();
    public AITarget audioThreat = new AITarget();


    // Protected 
    // Referencia al estado actual
    protected AIState _currentState = null;
    // Diccionario que almacena los estados disponibles en la máquina de estados
    protected Dictionary<AIStateType, AIState> _states = new Dictionary<AIStateType, AIState>();
    protected AITarget _target = new AITarget(); // Objetivo actual
    
    // Protected Inspector Assigned
    [SerializeField] protected AIStateType _currentStateType = AIStateType.Idle;
    [SerializeField] protected SphereCollider _targetTrigger = null; // Zona de interacción con el objetivo
    [SerializeField] protected SphereCollider _sensorTrigger = null; // Zona de detección de sensores
    [SerializeField][Range(0f, 15f)] protected float _stoppingDistance = 1.0f; 

    // Component Cache
    protected Animator _animator = null;
    protected NavMeshAgent _navAgent = null;
    protected Collider _collider = null;
    protected Transform _transform = null;

    // Propiedades públicas para acceder a componentes
    public Animator Animator { get { return _animator; } }
    public NavMeshAgent NavAgent { get { return _navAgent; } }
    public Vector3 SensorPosition
    {
        get
        {
            if(_sensorTrigger == null) return Vector3.zero;
            Vector3 point = _sensorTrigger.transform.position;
            point.x += _sensorTrigger.center.x * _sensorTrigger.transform.lossyScale.x; 
            point.y += _sensorTrigger.center.y * _sensorTrigger.transform.lossyScale.y; 
            point.z += _sensorTrigger.center.z * _sensorTrigger.transform.lossyScale.z; 
            return point;
        }
    }

    public float SensorRadius
    {
        get
        {
            if(_sensorTrigger == null) return 0.0f;
            float radius =  Mathf.Max(_sensorTrigger.radius * _sensorTrigger.transform.lossyScale.x, 
                                      _sensorTrigger.radius * _sensorTrigger.transform.lossyScale.y);

            return Mathf.Max(radius, _sensorTrigger.radius * _sensorTrigger.transform.lossyScale.z);
        }
    }



    // Inicializa las referencias a los componentes en Awake
    protected virtual void Awake()
    {
        _transform = transform;
        _animator = GetComponent<Animator>();
        _navAgent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<Collider>();

        // Comprueba si hay una instancia de GameSceneManager
        if(GameSceneManager.Instance != null)
        {
            // Obtiene el ID de la instancia del objeto y registra la IA en la escena de juego
            if(_collider) GameSceneManager.Instance.RegisterAIStateMachine(_collider.GetInstanceID(), this); 
            if(_sensorTrigger) GameSceneManager.Instance.RegisterAIStateMachine(_sensorTrigger.GetInstanceID(), this);
        }
    }

    // Configura los estados de la máquina en el inicio
    protected virtual void Start()
    {
    
        if(_sensorTrigger != null)
        {
            // Obtiene la referencia al componente AISensor
            AISensor sensor = _sensorTrigger.GetComponent<AISensor>();
            if(sensor != null)
            {
                // Establece el padre de la IA en el sensor
                sensor.ParentStateMachine = this;
            }
        }

        // Obtiene todos los estados definidos en este GameObject
        AIState[] states = GetComponents<AIState>();

        // Registra cada estado en el diccionario
        foreach (AIState state in states)
        {
            if (state != null && !_states.ContainsKey(state.GetStateType()))
            {
                _states[state.GetStateType()] = state; // Añade el estado al diccionario
                state.SetStateMachine(this);
            }
        }

        // Verificamos si el estado actual se encuentra en el diccionario
        if(_states.ContainsKey(_currentStateType))
        {
            // Establece el estado actual
            _currentState = _states[_currentStateType];
            _currentState.OnEnterState();
        }
        else
        { 
            _currentState = null;
        }
    }

    protected virtual void Update()
    {
        if(_currentState == null) return;

        
        // Actualiza el estado actual y devuelve el tipo de estado que debe ser establecido en el próximo frame
        AIStateType newStateType = _currentState.OnUpdate();

        // Verifica si el nuevo estado es diferente al actual
        if(newStateType != _currentStateType)
        {
            // Se declara un nuevo estado
            AIState newState = null;

            // Verifica si el nuevo estado se encuentra en el diccionario
            if(_states.TryGetValue(newStateType, out newState))
            {
                // Limpia el estado actual
                _currentState.OnExitState();
                // Establece el nuevo estado
                newState.OnEnterState();
                // Actualiza el estado actual
                _currentState = newState;
            }
            // Si no se encuentra en el diccionario, establece el estado Idle
            else if(_states.TryGetValue(AIStateType.Idle, out newState))
            {
                _currentState.OnExitState();
                newState.OnEnterState();
                _currentState = newState;
            }

            // Actualiza el tipo de estado actual por el nuevo estado
            _currentStateType = newStateType;
        }
    }

    protected virtual void FixedUpdate()
    {
        // Limpia las amenazas detectadas en cada frame fijo
        visualThreat.Clear();
        audioThreat.Clear();

        // Actualiza la distancia al objetivo si existe uno asignado
        if (_target.Type != AITargetType.None)
        {
            _target.Distance = Vector3.Distance(_transform.position, _target.Position);
        }
    }

    protected virtual void OnAnimatorMove()
    {                
        if(_currentState != null)
            _currentState.OnAnimatorUpdated();
    }

    protected virtual void OnAnimatorIK(int layerIndex)
    {
        if(_currentState != null)
        {
            _currentState.OnAnimatorIKUpdated();
        }
    }

    /// <summary>
    /// Llamado cuando el trigger del objetivo colisiona con el personaje.
    /// </summary>
    /// <param name="other">El objeto que se encuentra en el trigger.</param>
    protected virtual void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto es el trigger del objetivo
        if(_targetTrigger == null || other != _targetTrigger) return;

        // Verifica si el estado actual no es null
        if(_currentState)
            _currentState.OnDestinationReached(true); // Establece el destino como alcanzado
    }

    /// <summary>
    /// Llamado cuando el trigger del objetivo deja de colisionar con el personaje.
    /// </summary>
    /// <param name="other">El objeto que ha salido del trigger.</param>
    protected virtual void OnTriggerExit(Collider other)
    {
        // Verifica si el objeto es el trigger del objetivo
        if(_targetTrigger == null || other != _targetTrigger) return;

        // Verifica si el estado actual no es null
        if(_currentState)
            _currentState.OnDestinationReached(false); // Establece el destino como no alcanzado
    }

    /// <summary>
    /// Establece la actualización de la posición y la rotación del agente de navegación.
    /// </summary>
    /// <param name="positionUpdate">Indica si se debe actualizar la posición del agente.</param>
    /// <param name="rotationUpdate">Indica si se debe actualizar la rotación del agente.</param>
    public void NavAgentControl(bool positionUpdate, bool rotationUpdate)
    {
         if(_navAgent)
         {
            // Establece la actualización de la posición y la rotación
            _navAgent.updatePosition = positionUpdate;
            _navAgent.updateRotation = rotationUpdate;
         }
    }

    /// <summary>
    /// Maneja el evento de colisión con un trigger.
    /// </summary>
    /// <param name="type">Tipo de evento de trigger.</param>
    /// <param name="other">Objeto que colisionó con el trigger.</param>
    public virtual void OnTriggerEvent(AITriggerEventType type, Collider other) 
    {
        if(_currentState != null)
            _currentState.OnTriggerEvent(type, other); // Llamada al estado actual para manejar el evento 
    }

    // Establece un objetivo con la información proporcionada
    public void SetTarget(AITargetType type, Collider collider, Vector3 position, float distance)
    {
        _target.Set(type, collider, position, distance);

        // Configura el trigger del objetivo
        if (_targetTrigger != null)
        {
            _targetTrigger.radius = _stoppingDistance;
            _targetTrigger.transform.position = _target.Position;
            _targetTrigger.enabled = true;
        }
    }

    // Variante de SetTarget con distancia de detención personalizada
    public void SetTarget(AITargetType type, Collider collider, Vector3 position, float distance, float stopping)
    {
        _target.Set(type, collider, position, distance);

        if (_targetTrigger != null)
        {
            _targetTrigger.radius = stopping;
            _targetTrigger.transform.position = _target.Position;
            _targetTrigger.enabled = true;
        }
    }

    // Establece un objetivo a partir de una estructura AITarget existente
    public void SetTarget(AITarget target)
    {
        _target = target;

        if (_targetTrigger != null)
        {
            _targetTrigger.radius = _stoppingDistance;
            _targetTrigger.transform.position = _target.Position;
            _targetTrigger.enabled = true;
        }
    }

    // Limpia el objetivo actual y desactiva el trigger asociado
    public void ClearTarget()
    {
        _target.Clear();

        if (_targetTrigger != null)
        {
            _targetTrigger.enabled = false;
        }
    }
    
}
