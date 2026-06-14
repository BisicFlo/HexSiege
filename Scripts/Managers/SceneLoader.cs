using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;


[System.Serializable]
public class ButtonEntry {
    public Button Button;
    public int SceneIndex;
    public int ColorTheme; // ColorShifter.cs    (0-87)
    public int BackGround; // ColorShifter.cs    (0-13)
}

public class SceneLoader : MonoBehaviour {

    [SerializeField] private ColorShifter colorShifter;
    [SerializeField] private List<ButtonEntry> ButtonEntries; // set in Inspector

    //InventorySlot;
    //[SerializeField] private Transform buttonsParent;
    //[SerializeField] private List<Button> buttonList;

    [SerializeField] private Button  quitButton;
    [SerializeField] private SliderBar LoadingBar;    // Used to change bar visual Loading Screen

    private void OnEnable() {
        SetupButtonsEvents();
    }

    private void OnDisable() {
        UnsubscribeAllButtons();
    }

    private void SetupButtonsEvents() {

        for (int i = 0; i < ButtonEntries.Count; i++) {

            int buttonIndex = ButtonEntries[i].SceneIndex;
            int colorIndex = ButtonEntries[i].ColorTheme;
            int BackgroundIndex = ButtonEntries[i].BackGround;

            if (buttonIndex == -2) { // - - - - Next Level - - - -

                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex; // Gets the current active scene
                if (DoesSceneIndexExist(currentSceneIndex + 1)) {
                    ButtonEntries[i].Button.onClick.AddListener(() => OnClick(currentSceneIndex + 1, colorIndex, BackgroundIndex));
                }
            }

            else if (buttonIndex == -1) { // - - - - Reload Same Level - - - -

                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex; // Gets the current active scene
                ButtonEntries[i].Button.onClick.AddListener(() => OnClick(currentSceneIndex, colorIndex, BackgroundIndex));
            }

            else if (buttonIndex == 0) { // - - - - Load Main Menu - - - -

                ButtonEntries[i].Button.onClick.AddListener(() => OnClick( 0, colorIndex, BackgroundIndex));
            }

            else {      // - - - - Load Level n° buttonIndex - - - -
                
                bool canPlay = SaveManager.Instance.Data.levelUnlocked[buttonIndex]; // if level unlocked 
                if (DoesSceneIndexExist(buttonIndex) && canPlay) {
                    ButtonEntries[i].Button.onClick.AddListener(() => OnClick(buttonIndex, colorIndex, BackgroundIndex));
                }
                else {
                    ButtonEntries[i].Button.interactable = false;
                }
            }
        }
    }

    private void UnsubscribeAllButtons() {
        // Quit button
        if (quitButton != null)
            quitButton.onClick.RemoveAllListeners();

        // All child buttons
        for (int i = 0; i < ButtonEntries.Count; i++) {
            //Button button = buttonsParent.GetChild(i).GetComponent<Button>();
            ButtonEntries[i].Button.onClick.RemoveAllListeners();            
        }
    }

    private void OnClick(int buttonIndex, int colorIndex, int backgroundIndex) {
        Debug.Log("Onclick : " + buttonIndex + "|" + colorIndex + "|" + backgroundIndex);
        // Display Loading Screen
        UIManager.Instance.ShowScreen(ScreenType.Loading);
        // Start Loading 
        StartCoroutine(LoadAsync(buttonIndex));
        // Change Color Theme 
        colorShifter.SetColorByIndex(colorIndex);
        // Change Background Color  
        colorShifter.SetBackGroundColorFromIndex(backgroundIndex);
    }



    private bool DoesSceneIndexExist(int index) {
        // Indices are 0-based, so they must be >= 0 and less than the total count
        return index >= 0 && index < SceneManager.sceneCountInBuildSettings;
    }

    private IEnumerator LoadAsync(string sceneName) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");
            yield return null;
        }
    }
    private IEnumerator LoadAsync(int sceneIndex) {

        if (LoadingBar!=null) LoadingBar.SetMaxValue(100);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");

            int myProgress = (int) progress*100;

            if (LoadingBar != null) LoadingBar.SetValue(myProgress);

            yield return null;
        }
    }
}
