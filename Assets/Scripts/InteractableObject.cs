using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class InteractableObject : NetworkBehaviour
{
    public string ItemName;
    public bool inRange;
    public SelectionManager sm;
    public bool canPickup;

    public string GetItemName()
    {
        return ItemName;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
        }
    }

    public void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.F) && inRange && canPickup) 
        {
            if (!InventorySystem.Instance.checkIfFull())
            {
                InventorySystem.Instance.addToInv(ItemName);
                pickupItemRpc();
            }
            else
            {
                Debug.Log("Inv is full");
            }
            //pickUpItemServerRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Everyone)]
    public void pickupItemRpc() {
        if (inRange)
        {
            Debug.Log("Item added to inv");
            Destroy(gameObject);
        }
    }
}