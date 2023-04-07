using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;
    public GameObject completeCanvas;
    public GameObject notCompleteCanvas;

    [SerializeField] private Button MyButton = null; // assign in the editor

    void Start()
    {
        MyButton.onClick.AddListener(() => { IsComplete(); });
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
        //Debug.Log("Load next scene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);

        // if (SceneManager.GetActiveScene().buildIndex != 0)
        // {
        //     CircuitLab lab = FindObjectOfType<CircuitLab>();

        //     // if (lab.checkRequirements())
        //     // {
        //     //     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //     // }
        //     // else
        //     // {
        //     //     Debug.Log("Circuit not complete");
        //     // }
        // }


    }

    public void LoadMainMenu(){
        SceneManager.LoadScene(Scene.MainMenu.ToString());
    }

    public void IsComplete(){
        CircuitLab lab = FindObjectOfType<CircuitLab>();

            if (lab.checkRequirements())
            {
                
                completeCanvas.SetActive(true);
            }
            else
            {
                notCompleteCanvas.SetActive(true);
            }
        
    }

    public void hideCanvas(){
        notCompleteCanvas.SetActive(false);
    }
}

