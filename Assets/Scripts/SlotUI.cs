using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public enum SlotPanelType { Inventory, LootBox }

public class SlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Данные слота")]
    public SlotPanelType panelType; 
    public int slotIndex;           

    [Header("Ссылки на визуал")]
    public Image iconImage;
    public TextMeshProUGUI amountText;

    private InventoryUI inventoryUI;
    private LootUI lootUI;
    private SelectionController selectionCtrl;

    private static Image ghostIcon; 
    private static SlotUI draggedSlot;

    void Start()
    {
        inventoryUI = FindObjectOfType<InventoryUI>();
        lootUI = FindObjectOfType<LootUI>();
        selectionCtrl = FindObjectOfType<SelectionController>();

        if (ghostIcon == null)
        {
            GameObject ghostObj = GameObject.Find("DragIconGhost");
            if (ghostObj != null) ghostIcon = ghostObj.GetComponent<Image>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(amountText.text)) return;

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            bool isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            if (panelType == SlotPanelType.LootBox)
                TransferFromLootToInventory(isShiftPressed);
            else if (panelType == SlotPanelType.Inventory)
                TransferFromInventoryToLoot(isShiftPressed);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (iconImage.color.a < 0.1f || string.IsNullOrEmpty(amountText.text)) return;

        draggedSlot = this;

        if (ghostIcon != null)
        {
            ghostIcon.sprite = iconImage.sprite;
            ghostIcon.color = new Color(1f, 1f, 1f, 0.7f); 
            ghostIcon.gameObject.SetActive(true);
            ghostIcon.transform.position = Input.mousePosition;
        }

        iconImage.color = new Color(1f, 1f, 1f, 0.4f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ghostIcon != null && ghostIcon.gameObject.activeSelf)
        {
            ghostIcon.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedSlot == null) return;
        
        if (ghostIcon != null) ghostIcon.gameObject.SetActive(false);
        iconImage.color = Color.white; 

        GameObject hoveredUI = eventData.pointerCurrentRaycast.gameObject;
        
        if (hoveredUI == null) 
        {
            draggedSlot = null;
            return;
        }

        SlotUI targetSlot = hoveredUI.GetComponentInParent<SlotUI>();

        if (targetSlot != null && targetSlot != draggedSlot)
        {
            if (draggedSlot.panelType == SlotPanelType.LootBox && targetSlot.panelType == SlotPanelType.Inventory)
            {
                draggedSlot.TransferFromLootToInventory(true);
            }
            else if (draggedSlot.panelType == SlotPanelType.Inventory && targetSlot.panelType == SlotPanelType.LootBox)
            {
                draggedSlot.TransferFromInventoryToLoot(true);
            }
        }
        
        draggedSlot = null;
    }

    public void TransferFromLootToInventory(bool allStack)
    {
        if (lootUI == null || lootUI.CurrentLootBox == null || selectionCtrl == null) return;
        UnitController activeUnit = selectionCtrl.GetMainSelectedUnit();
        if (activeUnit == null) return;
        UnitInventory unitInv = activeUnit.GetComponent<UnitInventory>();
        if (unitInv == null) return;

        LootBox box = lootUI.CurrentLootBox;
        if (slotIndex >= box.boxContents.Count) return;
        var lootItem = box.boxContents[slotIndex];

        int amountToTransfer = allStack ? lootItem.amount : 1;
        unitInv.AddResource(lootItem.itemType, amountToTransfer);

        lootItem.amount -= amountToTransfer;
        if (lootItem.amount <= 0) box.boxContents.RemoveAt(slotIndex);

        lootUI.UpdateUI();
        inventoryUI.UpdateUI();
        box.CheckEmptyState();
    }

    public void TransferFromInventoryToLoot(bool allStack)
    {
        if (lootUI == null || lootUI.CurrentLootBox == null || selectionCtrl == null) return;
        UnitController activeUnit = selectionCtrl.GetMainSelectedUnit();
        if (activeUnit == null) return;
        UnitInventory unitInv = activeUnit.GetComponent<UnitInventory>();
        if (unitInv == null) return;

        if (slotIndex >= unitInv.slots.Count) return;
        var invSlot = unitInv.slots[slotIndex];
        LootBox box = lootUI.CurrentLootBox;

        int amountToTransfer = allStack ? invSlot.amount : 1;
        box.AddItem(invSlot.itemType, amountToTransfer);

        invSlot.amount -= amountToTransfer;
        if (invSlot.amount <= 0) unitInv.slots.RemoveAt(slotIndex);

        lootUI.UpdateUI();
        inventoryUI.UpdateUI();
    }
}