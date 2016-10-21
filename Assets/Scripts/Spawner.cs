using UnityEngine;
using System.Collections;
using BeardedManStudios.Network;
using System;
public class Spawner : MonoBehaviour {

    public GameObject player;
	// Use this for initialization
	void Start () {
        System.Random r = new System.Random();
        player.transform.position = new Vector3(3 - (float)r.NextDouble() * 6, .5f, 3 - (float)r.NextDouble() * 6);
        Networking.Instantiate(player/*"VanillaPlayer"*/, NetworkReceivers.AllBuffered);
	}
}
