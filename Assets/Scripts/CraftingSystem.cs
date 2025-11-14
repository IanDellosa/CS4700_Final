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
        craftLeverBtn.onClick.AddListener(delegate { CraftAnyItem(); });
    }

    void CraftAnyItem()
    {
        // add item to inv


    }

    // Update is called once per frame
    void Update()
    {
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
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            craftingScreenUI.SetActive(false);
            isOpen = false;
        }
    }
}
