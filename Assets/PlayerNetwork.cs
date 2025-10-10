using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{

    NetworkVariable<int> randNum = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] private Transform spawnedObjectPrefab;
    private Transform spawnedObjectTransform;
    [SerializeField] private Transform orientation;
    float horizontalInput;
    float verticalInput;

    public GameObject firstPersonCam;
    private void Update()
    {
        //Debug.Log(OwnerClientId + "; randNum: " + randNum.Value);

        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            spawnObjServerRpc();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            deleteObjServerRpc();
        }

        if (IsLocalPlayer)
        {
            firstPersonCam.gameObject.SetActive(true);
        }

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        float moveSpeed = 10f;
        transform.position += (horizontalInput * orientation.right + verticalInput * orientation.forward) * moveSpeed * Time.deltaTime;
    }

    [ServerRpc]
    private void spawnObjServerRpc()
    {
        spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
        spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
    }

    [ServerRpc]
    private void deleteObjServerRpc()
    {
        Destroy(spawnedObjectTransform);
    }
}
