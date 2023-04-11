using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticipantData
{
   public string participantID ="";

   public float level01startTime =0f;
    public float level01endTime =0f;
   public float level02startTime =0f;
   public float level02endTime =0f;
    public float level03startTime = 0f;
    public float level03endTime = 0f;

  
   
  

    public ParticipantData(){
    participantID ="";

   level01startTime =0f;
   level02startTime =0f;
    level03startTime = 0f;

   level01endTime =0f;
   level02endTime =0f;
    level03endTime = 0f;
    }

   public string SaveToString()
    {
        return JsonUtility.ToJson(this,true);
    }


}
