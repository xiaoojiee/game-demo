using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TrashClickHandler : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    public Color hoverColor  = new Color(0.6f, 0.2f, 0.2f, 1f);
    public Color normalColor = new Color(0.3f, 0.1f, 0.1f, 1f);
    private Image img;

    void Awake() { img = GetComponent<Image>(); if (img) img.color = normalColor; }
    public void OnPointerClick(PointerEventData e) { SlotDragHandler.TryDestroyCarried(); }
    public void OnDrop(PointerEventData e)         { SlotDragHandler.TryDestroyCarried(); }
    public void OnPointerEnter(PointerEventData e) { if (img) img.color = hoverColor; }
    public void OnPointerExit(PointerEventData e)  { if (img) img.color = normalColor; }
}
