using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string tooltipText;
    public float yOffset = 100f;
    public float xOffset = 100f;
    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.instance.ShowTooltip(tooltipText);
        Vector3 newPosition = Input.mousePosition;
        newPosition.y += yOffset;
        newPosition.x += xOffset;
        TooltipManager.instance.SetPosition(newPosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.instance.HideTooltip();
    }
}