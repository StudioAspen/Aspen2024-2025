using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AugmenTreeController : MonoBehaviour
{

    public GameObject augmentSlots;
    public GameObject slotBranches;
    public Button augment1;
    public Button augment2;
    public Button augment3;
    public Button closeWindow;
    public TextMeshProUGUI currAugment;

    void Start()
    {
        augment1.onClick.AddListener(() => {OpenBranches(augment1); });
        augment2.onClick.AddListener(() => {OpenBranches(augment2); });
        augment3.onClick.AddListener(() => {OpenBranches(augment3); });
        closeWindow.onClick.AddListener(CloseBranches);
    }

    public void OpenBranches(Button button)
    {
        //take in the augment or slot name so we can render to correct data that corresponds to that augment
        augmentSlots.SetActive(false);
        slotBranches.SetActive(true);
        currAugment.text = button.name;
    }

    public void CloseBranches()
    {
        slotBranches.SetActive(false);
        augmentSlots.SetActive(true);
        Debug.Log("Clicked");
    }
}
