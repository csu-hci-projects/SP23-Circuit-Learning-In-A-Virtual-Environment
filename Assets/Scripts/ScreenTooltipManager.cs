using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScreenTooltipManager : MonoBehaviour
{
    #region Singleton

    private static ScreenTooltipManager instance = null;

    public static ScreenTooltipManager Instance => instance;

    private void Awake(){
        if(instance == null)
            instance = this;
        else
        {
            DestroyImmediate(this);
        }
    }

    #endregion


    private Vector3 centerScreenPos = new Vector3(Screen.width/2,Screen.height/2);
    [SerializeField] private GameObject tooltipObject = null;
    [SerializeField] private RectTransform transformTooltip = null;
    [SerializeField] private TextMeshProUGUI textTooltip = null;

    public void SetTooltipAtPosWithMessage(Vector3 pos, string message, bool isTwoDimensions = true){
        tooltipObject.SetActive(true);
        textTooltip.text = message;
        transformTooltip.position = isTwoDimensions ? GetPos_2D(pos) : GetPos_3D(pos);
    }
    public void DeactivateTooltip() => tooltipObject.SetActive(false);
    private Vector3 GetPos_2D(Vector3 pos) => pos + PosTowardCenter(pos);
    private Vector3 GetPos_3D(Vector3 pos) {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
        return screenPos + PosTowardCenter(screenPos);
    }

    private Vector3 PosTowardCenter(Vector3 pos) => (centerScreenPos - pos).normalized *(transformTooltip.rect.width*2);
}