using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpenMenu : MonoBehaviour
{

    // public Button openMenu, closeMenu;
    public GameObject menu;
    private bool isOpen = false;

    public void Start() {

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
            menu.SetActive(true);
        }

        else {
            ResumeGame();
            menu.SetActive(false);
        }
    }
}
