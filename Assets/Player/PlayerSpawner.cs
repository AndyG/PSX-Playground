using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Player playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        Player spawnedPlayer = GameObject.Instantiate(playerPrefab, this.transform.position, this.transform.rotation);
        // spawnedPlayer.velocity = Vector3.zero;
        // Rigidbody headRigidBody = spawnedPlayer.head.GetComponent<Rigidbody>();
        // Rigidbody bodyRigidBody = spawnedPlayer.body.GetComponent<Rigidbody>();

        // headRigidBody.isKinematic = true;
        // bodyRigidBody.isKinematic = true;
        // spawnedPlayer.body.GetComponent<Collider>().enabled = false;
        // spawnedPlayer.head.GetComponent<Collider>().enabled = false;

        FollowCamera followCamera = GameObject.FindObjectOfType<FollowCamera>();
        followCamera.lookAtTarget = spawnedPlayer.transform;
        followCamera.follow = true;
        followCamera.player = spawnedPlayer;

        PlayerDebugUi playerDebugUi = FindObjectOfType<PlayerDebugUi>();
        playerDebugUi.player = spawnedPlayer;
    }
}
