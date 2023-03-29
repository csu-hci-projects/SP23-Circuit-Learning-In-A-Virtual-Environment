using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CompleteUI{

    public class Dialogue{
        public string Message = "You completed the circuit. Click the Continue button to progress to the next level.";
    }

    public class CompletionUI : MonoBehaviour
    {
        [SerializeField] GameObject canvas;
        [SerializeField] TextMeshProUGUI messageText;
        [SerializeField] Button continueButton;

        Dialogue dialogue = new Dialogue();
        
        //Initializing this as a singleton
        public static CompletionUI Instance;

        void Awake(){
            Instance = this;

            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(Hide);
        }

        public CompletionUI SetMessage(string message){
            dialogue.Message = message;
            return Instance;
        }

        public void Show(){
            messageText.text = dialogue.Message;

            dialogue = new Dialogue();

            canvas.SetActive(true);
        }

        public void Hide(){
            canvas.SetActive(false);
            dialogue = new Dialogue();
        }
    }
}
