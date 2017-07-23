using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	private GameObject player;
	private Vector3 offset;

	void Start(){
		player = GameObject.FindGameObjectWithTag("Player");
		offset = player.transform.position - transform.position;
	}

	void Update(){
		transform.position = Vector3.Lerp( transform.position, player.transform.position - offset, Time.deltaTime * 10f );
	}
}
