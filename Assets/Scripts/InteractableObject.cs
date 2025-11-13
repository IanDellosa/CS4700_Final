using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string ItemName;
    public bool inRange;

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
        if (inRange && Input.GetKeyDown(KeyCode.F) && SelectionManager.Instance.onTarget)
        {
            Debug.Log("Item added to inv");
            Destroy(gameObject);
        }
        {

        }
    }
}