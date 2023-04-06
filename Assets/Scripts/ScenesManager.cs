using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;

    private void Awake() {
        Instance = this;
    }

    public enum Scene{
        MainMenu,
        Level01
    }

    public void LoadScene(Scene scene){
        SceneManager.LoadScene(scene.ToString());
    }

    public void LoadNewGame(bool worldFixedActive){ 
        SceneManager.LoadScene(Scene.Level01.ToString());
        CircuitLab.isWorldFixed = worldFixedActive;
    }

    public void LoadNextScene(){
        CircuitLab lab = FindObjectOfType<CircuitLab>();

        if (lab.checkRequirements()) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            Debug.Log("Circuit not complete");
        }
       
    }

    public void LoadMainMenu(){
        SceneManager.LoadScene(Scene.MainMenu.ToString());
    }
}

