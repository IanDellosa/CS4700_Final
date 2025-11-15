using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class CraftingSystem : MonoBehaviour
{

    public GameObject craftingScreenUI;

    public List<string> invItemList = new List<string>();

    // Item Buttons
    Button craftLeverBtn;

    //REquirement Text
    TextMeshProUGUI leverReq1, leverReq2;

    public bool isOpen;

    // All Blueprints
    public itemBlueprint leverBlueprint = new itemBlueprint("Lever", "Stone", "Stick", 3, 2, 2);

    public static CraftingSystem Instance { get; set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        } 
        else
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isOpen = false;

        leverReq1 = craftingScreenUI.transform.Find("Lever").Find("req1").GetComponent<TextMeshProUGUI>();
        leverReq2 = craftingScreenUI.transform.Find("Lever").Find("req2").GetComponent<TextMeshProUGUI>();

        craftLeverBtn = craftingScreenUI.transform.Find("Lever").transform.Find("Button").GetComponent<Button>();
        craftLeverBtn.onClick.AddListener(delegate { CraftAnyItem(leverBlueprint); });
    }

    void CraftAnyItem(itemBlueprint blueprint)
    {
        switch (blueprint.numOfReqs)
        {
            case 2:
                InventorySystem.Instance.RemoveItem(blueprint.req2, blueprint.req2Amt);
                goto case 1;
            case 1:
                InventorySystem.Instance.RemoveItem(blueprint.req1, blueprint.req1Amt);
                break;
            default:
                Debug.Log("Crafting error");
                break;
        }

        RefreshNeededItems();

        StartCoroutine(Calculate());

        InventorySystem.Instance.addToInv(blueprint.itemName);

    }

    public IEnumerator Calculate()
    {
        yield return new WaitForSeconds(0.01f);

        InventorySystem.Instance.ReCalculateList();
    }

    // Update is called once per frame
    void Update()
    {
        RefreshNeededItems();

        if (Input.GetKeyDown(KeyCode.C) && !isOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("c is pressed");
            craftingScreenUI.SetActive(true);
            isOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.C) && isOpen)
        {
            craftingScreenUI.SetActive(false);
            if (!InventorySystem.Instance.isOpen) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
                        
            isOpen = false;
        }
    }

    private void RefreshNeededItems()
    {
        int stoneCount = 0;
        int stickCount = 0;

        invItemList = InventorySystem.Instance.itemList;

        foreach (string item in invItemList)
        {
            switch (item)
            {
                case "Stone":
                    stoneCount++;
                    break;
                case "Stick":
                    stickCount++;
                    break;

            }
        }

        // -----LEVER-----
        leverReq1.text = "3 Stone [" + stoneCount + "]";
        leverReq2.text = "2 Stick [" + stickCount + "]";

        if(stoneCount >= 3 && stickCount >= 2)
        {
            craftLeverBtn.gameObject.SetActive(true);
        } else
        {
            craftLeverBtn.gameObject.SetActive(false);
        }



    }
}
