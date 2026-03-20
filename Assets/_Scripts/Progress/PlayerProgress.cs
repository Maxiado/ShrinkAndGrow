using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ProgresoRECLAIMER", menuName = "ScaleEscape/PlayerProgress")]
public class PlayerProgress : ScriptableObject
{
    [Header("Recursos Principales")]
    public int quantumScraps = 0; // La moneda del juego

    [Header("Mejoras de Estadísticas (Niveles)")]
    [Tooltip("Nivel 1 es el base, Nivel 3 es el máximo")]
    public int strengthLevel = 1;  // Define la masa que puede empujar el Titán
    public int speedLevel = 1;     // Velocidad de movimiento base
    public int jumpLevel = 1;      // Altura del salto
    public int batteryLevel = 1;   // ¡Tu idea! El tiempo límite de transformación

    [Header("Habilidades Especiales (Metroidvania)")]
    public bool hasJetpack = false;
    public bool hasMagneticHook = false;
    public bool canGlide = false;
    public bool hasDensityScanner = false; // Para ver paredes falsas

    [Header("Registro de Coleccionables")]
    [Tooltip("Guarda los IDs de los secretos ya encontrados para no repetirlos")]
    public List<string> collectedSecrets = new List<string>();

    // --- FUNCIONES DE UTILIDAD PARA EL JUEGO ---
    
    // El PlayerScaleController llamará a esto para saber cuánto tiempo tiene
    public float GetTransformationTimeLimit()
    {
        return batteryLevel switch
        {
            1 => 5.0f,  // Nivel básico: 5 segundos (tensión alta)
            2 => 12.0f, // Nivel medio: 12 segundos
            3 => 30.0f, // Nivel máximo: 30 segundos (casi libertad total)
            _ => 5.0f
        };
    }

    public float GetSpeedMultiplier()
    {
        // Añade un 15% extra de velocidad por cada nivel
        return 1.0f + ((speedLevel - 1) * 0.15f); 
    }
}