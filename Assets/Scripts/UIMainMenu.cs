using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] Button _newGameGroupA;
    [SerializeField] Button _newGameGroupB;


    // Start is called before the first frame update
    void Start()
    {
        _newGameGroupA.onClick.AddListener(delegate{StartNewGame(true);});
        _newGameGroupB.onClick.AddListener(delegate{StartNewGame(false);});
    }

    private void StartNewGame(bool worldFixedActive){
        ScenesManager.Instance.LoadNewGame(worldFixedActive);
    
    }
}

