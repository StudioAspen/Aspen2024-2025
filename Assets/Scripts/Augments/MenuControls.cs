using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpenMenu : MonoBehaviour
{

    // public Button openMenu, closeMenu;
    public GameObject openMenu, closeMenu;
    private bool isOpen = false;

    public void Start() {
        // openMenu.onClick.AddListener(PauseGame);
        // closeMenu.onClick.AddListener(ResumeGame);
    }

    public void PauseGame() {
        Debug.Log("Paused");
        Time.timeScale = 0;
    }

    public void ResumeGame() {
        Debug.Log("Resumed");
        Time.timeScale = 1;
    }

    public void Update() {
        if (Input.GetKeyDown("m")) {
            isOpen = !isOpen;
        }

        if (isOpen) {
            PauseGame();
            openMenu.SetActive(false);
            closeMenu.SetActive(true);
        }

        else {
            ResumeGame();
            closeMenu.SetActive(false);
            openMenu.SetActive(true);
        }
    }
}
