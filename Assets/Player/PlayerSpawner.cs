using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Player playerPrefab;
    public PointsTracker pointsTracker;

    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        Player spawnedPlayer = GameObject.Instantiate(playerPrefab, this.transform.position, this.transform.rotation);

        FollowCamera followCamera = GameObject.FindObjectOfType<FollowCamera>();
        followCamera.lookAtTarget = spawnedPlayer.transform;
        followCamera.follow = true;
        followCamera.player = spawnedPlayer;

        pointsTracker.TrackPlayer(spawnedPlayer);
    }
}
