using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarsTabHandler : MonoBehaviour
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _staminaBar;


    private void UpdateStamina(float stamina)
    {
        _staminaBar.fillAmount = stamina;
    }

    private void UpdateHealth(float health)
    {
        _healthBar.fillAmount = health;
    }

    private void OnEnable()
    {
        Player.OnUpdateStamina += UpdateStamina;
        Player.OnUpdateHealth += UpdateHealth;
    }

    private void OnDisable()
    {
        Player.OnUpdateStamina -= UpdateStamina;
        Player.OnUpdateHealth -= UpdateHealth;
    }
}
