using UnityEngine;
using UnityEngine.UI;
using TMPro; // Usamos TextMeshPro para textos de UI más nítidos

public class UpgradeMenu : MonoBehaviour
{
    [Header("Base de Datos")]
    public PlayerProgress progressData;

    [Header("UI - Textos")]
    public TextMeshProUGUI scrapsText;
    public TextMeshProUGUI batteryLevelText;
    public TextMeshProUGUI batteryCostText;

    [Header("UI - Botones")]
    public Button upgradeBatteryButton;

    [Header("Configuración de Economía")]
    public int baseUpgradeCost = 50; // Costo base que se multiplicará por el nivel

    void OnEnable()
    {
        UpdateUI(); // Actualiza la UI cada vez que abres el menú
    }

    // Esta función se enlaza al botón de "Mejorar Batería" en el Inspector
    public void BuyBatteryUpgrade()
    {
        int cost = GetCurrentCost(progressData.batteryLevel);
        
        if (progressData.batteryLevel < 3 && progressData.quantumScraps >= cost)
        {
            progressData.quantumScraps -= cost; // Restar chatarra
            progressData.batteryLevel++;        // Subir nivel
            UpdateUI();                         // Refrescar pantalla
            
            Debug.Log("¡Batería de Transformación Mejorada!");
            // Aquí podrías reproducir un sonido de éxito
        }
    }

    private int GetCurrentCost(int currentLevel)
    {
        // Nivel 1 a 2 = 50. Nivel 2 a 3 = 100.
        return baseUpgradeCost * currentLevel; 
    }

    private void UpdateUI()
    {
        // 1. Mostrar chatarra actual
        scrapsText.text = $"Chatarra Cuántica: {progressData.quantumScraps}";

        // 2. Mostrar datos de la batería
        if (progressData.batteryLevel >= 3)
        {
            batteryLevelText.text = "Batería: MAX";
            batteryCostText.text = "---";
            upgradeBatteryButton.interactable = false; // Desactivar botón
        }
        else
        {
            batteryLevelText.text = $"Batería: Lvl {progressData.batteryLevel}";
            int cost = GetCurrentCost(progressData.batteryLevel);
            batteryCostText.text = $"Costo: {cost}";
            
            // El botón solo se puede clickear si tienes suficiente chatarra
            upgradeBatteryButton.interactable = (progressData.quantumScraps >= cost);
        }
    }
}