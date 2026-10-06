using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Player player;
    public Slider healthSlider;

    void Start()
    {
        if (player != null && healthSlider != null)
        {
            player.health = player.maxHealth;
            healthSlider.maxValue = player.maxHealth;
            healthSlider.value = player.health;
        }
    }

    void Update()
    {
        if (player != null && healthSlider != null)
        {
            healthSlider.value = Mathf.Clamp(player.health, 0, player.maxHealth);

            // Hide fill if health is 0
            if (healthSlider.fillRect != null)
                healthSlider.fillRect.gameObject.SetActive(healthSlider.value > 0);
        }
    }
}
