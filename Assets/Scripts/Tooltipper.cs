using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltipper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
   
   [SerializeField] private string tooltip = null;

    #region Pointer Events Implementation
   public void OnPointerEnter(PointerEventData eventData){
    Debug.Log("Entered");
    if(ScreenTooltipManager.Instance)
        ScreenTooltipManager.Instance.SetTooltipAtPosWithMessage(transform.position, tooltip);
   }

   public void OnPointerExit(PointerEventData eventData)
    {
        if(ScreenTooltipManager.Instance)
        ScreenTooltipManager.Instance.DeactivateTooltip();
    }
    #endregion
   private void OnMouseEnter(){
    if(ScreenTooltipManager.Instance)
        ScreenTooltipManager.Instance.SetTooltipAtPosWithMessage(transform.position, tooltip, false);
   }

   private void OnMouseExit(){
    if(ScreenTooltipManager.Instance)
        ScreenTooltipManager.Instance.DeactivateTooltip();
   }

   private void OnMouseDown() {
    if(ScreenTooltipManager.Instance)
        ScreenTooltipManager.Instance.DeactivateTooltip();
   }
}

