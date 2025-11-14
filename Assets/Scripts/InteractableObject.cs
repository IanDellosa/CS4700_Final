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
        sm = FindFirstObjectByType<SelectionManager>();
        if (Input.GetKeyDown(KeyCode.F) && inRange && sm.onTarget && canPickup && sm.selectedObject==gameObject)
        {
            if (!InventorySystem.Instance.checkIfFull())
            {
                InventorySystem.Instance.addToInv(ItemName);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Inv is full");
            }
            //pickUpItemServerRpc();
            
        }
        {

        }
    }

    //[Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    //public void pickUpItemServerRpc()
    //{
    //    if (inRange && sm.onTarget)
    //    {
    //        Debug.Log("Item added to inv");
    //        Destroy(gameObject);
    //    }
    //}
}