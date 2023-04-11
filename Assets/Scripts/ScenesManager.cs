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
    public DataManager dataManager;

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

        if(SceneManager.GetActiveScene().buildIndex == 3)
            Application.Quit();
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);

    }

    public void LoadMainMenu(){
        SceneManager.LoadScene(Scene.MainMenu.ToString());
    }

    public void IsComplete(){
        CircuitLab lab = FindObjectOfType<CircuitLab>();

            if (lab.checkRequirements())
            {
                int sceneNum = SceneManager.GetActiveScene().buildIndex;
                
                switch(sceneNum){
                    case 1:
                        UIMainMenu.participantData.level01endTime = Time.time;
                        
                        break;
                    case 2:
                        UIMainMenu.participantData.level02endTime = Time.time;
                        break;
                    case 3:
                        UIMainMenu.participantData.level03endTime = Time.time;
                        dataManager.Save();
                        break;
                }
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

