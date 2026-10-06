using UnityEngine;
using UnityEngine.UI;

public class HouseHealthBar : MonoBehaviour
{

    public House house;
    public Slider healthSlider;

    void Start()
    {
        if (house != null && healthSlider != null)
        {
            house.health = house.maxHealth;
            healthSlider.maxValue = house.maxHealth;
            healthSlider.value = house.health;
        }
    }

    void Update()
    {
        if (house != null && healthSlider != null)
        {
            healthSlider.value = Mathf.Clamp(house.health, 0, house.maxHealth);

            // Hide fill if health is 0
            if (healthSlider.fillRect != null)
                healthSlider.fillRect.gameObject.SetActive(healthSlider.value > 0);
        }
    }
}
