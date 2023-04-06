using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;


    [SerializeField] private Button MyButton = null; // assign in the editor

    void Start()
    {
        MyButton.onClick.AddListener(() => { LoadNextScene(); });
    }


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
        Debug.Log("Load next scene");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);

        CircuitLab lab = FindObjectOfType<CircuitLab>();

        if (lab.checkRequirements())
        {
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

