using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camion : MonoBehaviour
{
   Ground velocity;
    
    // Sistema de objetivos por velocidad
    [System.Serializable]
    public class VelocityTarget
    {
        public float minSpeed; // Velocidad mínima para activar este objetivo
        public float targetX;  // Posición X objetivo
    }
    
    public List<VelocityTarget> velocityTargets = new List<VelocityTarget>()
    {
        new VelocityTarget { minSpeed = 0f, targetX = 6.80f },
        new VelocityTarget { minSpeed = 30f, targetX = 4f },
        new VelocityTarget { minSpeed = 40f, targetX = 3f },
        new VelocityTarget { minSpeed = 45f, targetX = -10f }
    };
    
    public float moveSpeed = 8f; // Velocidad de movimiento hacia el objetivo
    private float currentTargetX; // Objetivo actual
    private int currentTargetIndex = 0; // Índice del objetivo actual
    
    private void Awake()
    {
        velocity = GameObject.Find("Ground").GetComponent<Ground>();
        currentTargetX = transform.position.x; // Empezar en la posición actual
        
        // Ordenar objetivos por velocidad (por si acaso)
        velocityTargets.Sort((a, b) => a.minSpeed.CompareTo(b.minSpeed));
    }
    
    void FixedUpdate()
    {
        if (velocity.acelerar == false)
        {
            // Determinar el objetivo actual basado en la velocidad
            UpdateCurrentTarget();
            
            // Mover suavemente hacia el objetivo actual
            Vector2 pos = transform.position;
            pos.x = Mathf.MoveTowards(pos.x, currentTargetX, moveSpeed * Time.fixedDeltaTime);
            
            transform.position = pos;
        }
    }
    
    private void UpdateCurrentTarget()
    {
        // Encontrar el objetivo más alto que corresponda a la velocidad actual
        int newTargetIndex = 0;
        
        for (int i = 0; i < velocityTargets.Count; i++)
        {
            if (velocity.speed >= velocityTargets[i].minSpeed)
            {
                newTargetIndex = i;
            }
            else
            {
                break; // Los objetivos están ordenados, podemos salir
            }
        }
        
        // Si encontramos un nuevo objetivo, actualizar
        if (newTargetIndex != currentTargetIndex)
        {
            currentTargetIndex = newTargetIndex;
            currentTargetX = velocityTargets[currentTargetIndex].targetX;
            
            Debug.Log($"Cambiando objetivo a: {currentTargetX} (velocidad: {velocity.speed})");
        }
    }
    
    // Método para ver el estado actual (opcional, para debugging)
    void OnGUI()
    {
        #if UNITY_EDITOR
        GUI.Label(new Rect(10, 100, 300, 20), $"Camión - Objetivo: {currentTargetX}, Posición X: {transform.position.x:F2}");
        GUI.Label(new Rect(10, 120, 300, 20), $"Velocidad suelo: {velocity.speed:F1}, Objetivo index: {currentTargetIndex}");
        #endif
    }
}
