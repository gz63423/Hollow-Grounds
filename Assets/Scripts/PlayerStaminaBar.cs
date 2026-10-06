using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaBar : MonoBehaviour
{
    public Slider staminaSlider;
    private Player player;

    void Start()
    {
        player = FindFirstObjectByType<Player>();

        if (player == null || staminaSlider == null)
        {
            Debug.LogError("StaminaBar missing Player or Slider reference!");
            enabled = false;
            return;
        }

        staminaSlider.minValue = 0f;
        staminaSlider.maxValue = player.maxStamina;
        staminaSlider.wholeNumbers = false;
        staminaSlider.value = player.stamina;
    }

    void Update()
    {
        staminaSlider.normalizedValue = player.stamina / player.maxStamina;
    }
}
