using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
  

    public string file = "data.txt";

    public void Save(){
        string filePath = GetFilePath(file);
        string json = UIMainMenu.participantData.SaveToString();

        if(!File.Exists(filePath)){
            File.WriteAllText(filePath,"Participant Data \n\n");
        }
        File.AppendAllText(filePath,json+ "\n");

    }

    private string GetFilePath(string filename){
        return Application.dataPath + "/" + filename;
    }
}
