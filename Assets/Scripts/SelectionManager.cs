using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class SelectionManager : MonoBehaviour
{
    public Camera cam;
    public GameObject interaction_Info_UI;
    TextMeshProUGUI interaction_text;
    public bool onTarget;
    public GameObject selectedObject;

    public static SelectionManager Instance { get; private set; }

    public void Awake()
    {
        //if (Instance != null && Instance != this)
        //{
        //    Destroy(gameObject);
        //}
        //else
        //{
        //    Instance = this;
        //}
    }

    private void Start()
    {
        interaction_text = interaction_Info_UI.GetComponent<TextMeshProUGUI>();
    }

    

    void Update()
    {
        //if (!IsOwner) return;
        if (!cam) return;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;
            InteractableObject interactable = selectionTransform.GetComponent<InteractableObject>();
            if (interactable && interactable.inRange)
            {
                selectedObject = interactable.gameObject;
                interaction_text.text = interactable.GetItemName();
                interaction_Info_UI.SetActive(true);
                onTarget = true;
            }
            else
            {
                interaction_Info_UI.SetActive(false);
                onTarget = false;
            }
        } else
        {
            interaction_Info_UI.SetActive(false);
            onTarget = false;
        }
    }
}