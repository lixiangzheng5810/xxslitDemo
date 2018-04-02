﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class View_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private Image _ImgHoverOverlay;
    private Image _ImgIcon;
    private Ctrl_Slot slot;
    private Text _Amount;
    private Ctrl_PickUp pickupItemSlot;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _ImgHoverOverlay.gameObject.SetActive(true);
        if (slot.Item != null && View_PlayerinfoPespons.Instance.GetPlayerPackagePanelAlpha() > 0f)
        {
            Ctrl_InventoryManager.Instance.isToolTipShow = true;
            Ctrl_InventoryManager.Instance.Tootip.GetComponent<View_ToolTip>()
                .Show(slot.Item.ItemInfo());
        }
    }

    private void Awake()
    {
        slot = gameObject.GetComponent<Ctrl_Slot>();
        slot.slotItemInit += SlotInit;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _ImgHoverOverlay.gameObject.SetActive(false);
        Ctrl_InventoryManager.Instance.isToolTipShow = false;
    }

    public void SlotInit()
    {
        if (slot.Item != null)
        {
            _ImgIcon.gameObject.SetActive(true);
            Sprite sprite = Resources.Load<Sprite>(slot.Item.sprite);
            _ImgIcon.sprite = sprite;
            _Amount.text = slot.Item.currentNumber.ToString();
        }
        else
        {
            IconSetNull();
        }
    }

    void Start()
    {
        _ImgHoverOverlay = transform.Find("Hover Overlay").GetComponent<Image>();
        _ImgIcon = transform.Find("Icon").GetComponent<Image>();
        _Amount = transform.Find("Amount").GetComponent<Text>();
        pickupItemSlot = Ctrl_InventoryManager.Instance.PickUpItem.GetComponent<Ctrl_PickUp>();
    }


    private void UseItem()
    {
        slot.UseItem();
    }

    public void IconSetNull()
    {
        _ImgIcon.sprite = null;
        _ImgIcon.gameObject.SetActive(false);
        _Amount.text = "";
    }

    public void UpdateAmount()
    {
        _Amount.text = slot.Item.currentNumber.ToString();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (View_PlayerinfoPespons.Instance.GetPlayerPackagePanelAlpha() > 0)
        {
            //鼠标右键点击
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                //如果现在手上没有物品,并且当前slot有物品
                if (Ctrl_InventoryManager.Instance.IsPickedItem == false && slot.Item != null)
                {
                    Ctrl_InventoryManager.Instance.IsPickedItem = true;
                    pickupItemSlot.Item = slot.Item;
                    slot.Item = null;
                }
            }

            //鼠标左键点击
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                //当前手上存在物品
                if (Ctrl_InventoryManager.Instance.IsPickedItem)
                {
                    //当前格子内没有物品
                    if (slot.Item == null)
                    {
                        //按住Ctrl放下一个物品
                        if (Input.GetKey(KeyCode.LeftControl))
                        {
                            slot.Item = new Model_Item
                            {
                                id = pickupItemSlot.Item.id,
                                itemName = pickupItemSlot.Item.itemName,
                                itemType = pickupItemSlot.Item.itemType,
                                equipmentType = pickupItemSlot.Item.equipmentType,
                                maxStack = pickupItemSlot.Item.maxStack,
                                currentNumber = 1,
                                buyPriceByGold = pickupItemSlot.Item.buyPriceByGold,
                                buyPriceByDiamond = pickupItemSlot.Item.buyPriceByDiamond,
                                sellPriceByGold = pickupItemSlot.Item.sellPriceByGold,
                                sellPriceByDiamond = pickupItemSlot.Item.sellPriceByDiamond,
                                minLevel = pickupItemSlot.Item.minLevel,
                                sellable = pickupItemSlot.Item.sellable,
                                tradable = pickupItemSlot.Item.tradable,
                                destroyable = pickupItemSlot.Item.destroyable,
                                description = pickupItemSlot.Item.description,
                                sprite = pickupItemSlot.Item.sprite,
                                useDestroy = pickupItemSlot.Item.useDestroy,
                                useHealth = pickupItemSlot.Item.useHealth,
                                useMagic = pickupItemSlot.Item.useMagic,
                                useExperience = pickupItemSlot.Item.useExperience,
                                equipHealthBonus = pickupItemSlot.Item.equipHealthBonus,
                                equipManaBonus = pickupItemSlot.Item.equipManaBonus,
                                equipDamageBonus = pickupItemSlot.Item.equipDamageBonus,
                                equipDefenseBonus = pickupItemSlot.Item.equipDefenseBonus,
                                equipSpeedcBonus = pickupItemSlot.Item.equipSpeedcBonus,
                                modelPrefab = pickupItemSlot.Item.modelPrefab
                            };
                            pickupItemSlot.Item.currentNumber -= 1;
                            if (pickupItemSlot.Item.currentNumber == 0)
                            {
                                pickupItemSlot.Item = null;
                                Ctrl_InventoryManager.Instance.IsPickedItem = false;
                            }
                        }
                        else
                        {
                            slot.Item = pickupItemSlot.Item;
                            pickupItemSlot.Item = null;
                            Ctrl_InventoryManager.Instance.IsPickedItem = false;
                        }
                    }
                    //格子内存在物品
                    else
                    {
                        //如果是相同的物品,叠加 不是相同物品替换
                        if (pickupItemSlot.Item.id == slot.Item.id)
                        {
                            if (Input.GetKey(KeyCode.LeftControl))
                            {
                                if (slot.Item.currentNumber != slot.Item.maxStack)
                                {
                                    slot.Item.currentNumber += 1;
                                    slot.UpdateAmount();
                                    pickupItemSlot.Item.currentNumber -= 1;
                                    if (pickupItemSlot.Item.currentNumber == 0)
                                    {
                                        pickupItemSlot.Item = null;
                                        Ctrl_InventoryManager.Instance.IsPickedItem = false;
                                    }
                                }
                            }
                            else
                            {
                                //相同物品下 当前手上的物品个数小于格子物品剩余到达上限的个数,手上物品的个数添加到格子中
                                //大于的情况下 添加到格子上限 手上保存剩下的物品
                                if (pickupItemSlot.Item.currentNumber <= (slot.Item.maxStack - slot.Item.currentNumber))
                                {
                                    slot.Item.currentNumber += pickupItemSlot.Item.currentNumber;
                                    slot.UpdateAmount();
                                    pickupItemSlot.Item = null;
                                    Ctrl_InventoryManager.Instance.IsPickedItem = false;
                                }
                                else
                                {
                                    //更新手上物品数量
                                    pickupItemSlot.Item.currentNumber -= (slot.Item.maxStack - slot.Item.currentNumber);
                                    //更新格子物品数量及UI
                                    slot.Item.currentNumber = slot.Item.maxStack;
                                    slot.UpdateAmount();
                                }
                            }
                        }
                        else
                        {
                            //交换Item
                            Model_Item tempItem;
                            tempItem = slot.Item;
                            slot.Item = pickupItemSlot.Item;
                            pickupItemSlot.gameObject.GetComponent<Ctrl_PickUp>().Item = tempItem;
                        }
                    }
                }
                else
                {
                    UseItem();
                }
            }
        }
    }
}