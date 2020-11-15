using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDetecter : MonoBehaviour, IPointerClickHandler
{
    private InventoryManager mInventoryManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        int index = GetSlotIndex(eventData.pointerCurrentRaycast.gameObject.transform.parent.name);
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Left");
            mInventoryManager.UseItem(index);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Right");
            mInventoryManager.Drop(index);
        }
        //Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.transform.parent.name);
        //Debug.Log(index);
    }

    private void Start()
    {
        mInventoryManager = GameObject.FindObjectOfType<InventoryManager>();
    }

    private int GetSlotIndex(string SlotName)
    {
        for (int i = 1; i <= 6; i++) // 6 = Num slots
        {
            if (SlotName == "Slot" + i)
            {
                return i - 1;
            }
        }
        return -1;
    }
}
