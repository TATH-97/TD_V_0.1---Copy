using TMPro;
using UnityEngine;

public class DefenderMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currencyUI;
    

    private void OnGUI() {
        currencyUI.text=BuildManager.instance.currency.ToString();
    }
}

