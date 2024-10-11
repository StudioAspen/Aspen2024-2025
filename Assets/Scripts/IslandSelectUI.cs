using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IslandSelectUI : MonoBehaviour
{
    [System.Serializable]
    public class Island
    {
        public string islandName; // Name of the island
        public Sprite islandImage; // Image for the island (optional)

    }

    [SerializeField]
    public Island[] availableIslands; // Array of available islands
    public Button[] isSelectPlaceHolder; // UI Buttons to display the islands
    public float animationDuration = 1f; // Duration of the animation
    public float spacing = 10f;  // Space between buttons
    public float bounceDuration = 5f; // Duration of the bounce



    void Start()
    {
        // Ensure all buttons are not active and not shown to the player
        foreach (Button button in isSelectPlaceHolder)
        {
            button.gameObject.SetActive(false);
        }
        //   PrepareIslandSelction();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("AugmentSelector script started");
            Cursor.lockState = CursorLockMode.None; // Unlock the mouse
            Cursor.visible = true; // Show the cursor
            PrepareIslandSelction();
        }
    }
    public void PrepareIslandSelction()
    {
        Cursor.lockState = CursorLockMode.None; // Unlock the mouse
        Cursor.visible = true; // Show the cursor
        ///deal 4 random islands 
        Island[] selectedIslands = GetRandomIslands(4);

        //Set up buttons with selected islands 
        for (int i = 0; i < isSelectPlaceHolder.Length; i++)
        {
            ///Set the button text anmd image 
            Button button = isSelectPlaceHolder[i];
            Island island = selectedIslands[i];
            button.GetComponentInChildren<TMP_Text>().text = island.islandName;
            button.image.sprite = island.islandImage; // Set button image if you have one

            // Set initial position to a random off-screen point
            Vector3 randomOffScreenPosition = new Vector3(Random.Range(-Screen.width * 2, 5000), Random.Range(-2000, 2000), 0);
            button.transform.localPosition = randomOffScreenPosition;

            // Calculate target position for row alignment
            float targetXPosition = i * spacing * 50; // Align in a row
            Vector3 targetPosition = new Vector3(targetXPosition - 750, 0, 0); // Adjust X offset as needed

            // Create a sequence for smooth movement with a bounce effect
            Sequence buttonSequence = DOTween.Sequence();
            buttonSequence.Append(button.transform.DOLocalMove(targetPosition, animationDuration).SetEase(Ease.OutExpo));
            buttonSequence.Append(button.transform.DOLocalMoveY(100, bounceDuration).SetEase(Ease.OutBounce)); // Bounce up
            buttonSequence.Append(button.transform.DOLocalMoveY(0, bounceDuration).SetEase(Ease.OutBounce)); // Bounce down
            buttonSequence.SetDelay(i * 0.25f); // Stagger the animations by 0.2 seconds


            // Add listener for selection
            button.gameObject.SetActive(true); // Enable the button here
            button.onClick.AddListener(() => OnButtonSelected(button, island));
        }
    }

    Island[] GetRandomIslands(int count)
    {
        Island[] selectedIslands = new Island[count];
        List<Island> availableList = new List<Island>(availableIslands); // Create a copy of the available islands

        for (int i = 0; i < count; i++)
        {
            if (availableList.Count == 0) break; // Break if no islands are left
            int randomIndex = Random.Range(0, availableList.Count);
            selectedIslands[i] = availableList[randomIndex];
            availableList.RemoveAt(randomIndex); // Remove the selected island to avoid duplicates
        }

        return selectedIslands;
    }

    public void OnButtonSelected(Button selectedButton, Island selectedIsland)
    {
        Debug.Log(selectedIsland.islandName + " selected!");

        // Move the selected button to the top middle of the screen
        Vector3 targetPosition = new Vector3(0, Screen.height / 2, 0); // Adjust Y position as needed

        // Animate the selected button
        selectedButton.transform.DOLocalMove(targetPosition, 0.5f).SetEase(Ease.OutBack);

        // Perform the action based on the selected island
        PlaceIsland(selectedIsland);



        // Hide other buttons
        foreach (Button button in isSelectPlaceHolder)
        {
            if (button != selectedButton)
            {
                button.gameObject.SetActive(false);
            }
        }
    }
    void PlaceIsland(Island selectedIsland)
    {


        // Tell the spawn sphere what to do

        ResetSelection();



    }
 
    void ResetSelection()
    {
        // Re-enable all buttons and reset their positions
        foreach (Button button in isSelectPlaceHolder)
        {
            button.gameObject.SetActive(true); // Show the button again

        }

    }

}
