using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] Button _newGameGroupA;
    [SerializeField] Button _newGameGroupB;
    public static ParticipantData participantData = new ParticipantData();

    public TMP_InputField playerID;
    // Start is called before the first frame update
    void Start()
    {
       
        _newGameGroupA.onClick.AddListener(delegate{StartNewGame(true);});
        _newGameGroupB.onClick.AddListener(delegate{StartNewGame(false);});
    }

    private void StartNewGame(bool worldFixedActive){
        ScenesManager.Instance.LoadNewGame(worldFixedActive);
    
    }

    public void saveParticipantID(){
        participantData.participantID = playerID.text;
    }
}


