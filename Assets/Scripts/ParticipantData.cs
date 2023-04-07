using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticipantData
{
   public string participantID ="";

   public float level01time =0f;
   public float level02time =0f;
    public float level03time = 0f;

    public ParticipantData(){
    participantID ="";

   level01time =0f;
   level02time =0f;
    level03time = 0f;
    }

   public string SaveToString()
    {
        return JsonUtility.ToJson(this,true);
    }


}
