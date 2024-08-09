using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown fpsCapDropdown;
    [SerializeField] private TMP_Dropdown vSyncDropdown;

    [SerializeField] private Button closeButton;

    private void Awake()
    {
        fpsCapDropdown.onValueChanged.AddListener(ChangeFPSCap);
        vSyncDropdown.onValueChanged.AddListener(ChangeVSync);

        closeButton.onClick.AddListener(Hide);
    }

    private void ChangeFPSCap(int index)
    {
        string valueText = fpsCapDropdown.options[index].text;

        int fpsLimit = 0;

        switch (valueText)
        {
            case "60":
                fpsLimit = 60;
                break;
            case "120":
                fpsLimit = 120;
                break;
            case "144":
                fpsLimit = 144;
                break;
            case "240":
                fpsLimit = 240;
                break;
            case "Uncapped":
                fpsLimit = 0;
                break;
            default:
                break;
        }

        Application.targetFrameRate = fpsLimit;
    }

    private void ChangeVSync(int index)
    {
        string valueText = vSyncDropdown.options[index].text;

        QualitySettings.vSyncCount = valueText == "Enabled" ? 1 : 0;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
