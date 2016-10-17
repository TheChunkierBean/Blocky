using UnityEngine;
using System.Collections;
using BeardedManStudios.Network;

public class Spawner : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Networking.Instantiate("VanillaPlayer", NetworkReceivers.AllBuffered);
	}
}
