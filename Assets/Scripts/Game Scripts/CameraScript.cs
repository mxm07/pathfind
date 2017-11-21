using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
	public GameObject player;
	public GameObject camobj;
	public GameObject lightobj;

	protected Vector3 camOffset;
	protected Vector3 lightOffset;
	protected Camera cam;

	// Use this for initialization
	void Start () {
		camOffset = new Vector3(5f, 7f, -5.55f);
		lightOffset = new Vector3(-1f, 3f, 1f);

    	cam = camobj.GetComponent<Camera>();
		cam.orthographicSize = 3f;

		camobj.transform.position = new Vector3(player.transform.position.x + camOffset.x, camOffset.y, player.transform.position.z + camOffset.z);
		lightobj.transform.position = new Vector3(player.transform.position.x + lightOffset.x, lightOffset.y, player.transform.position.z + lightOffset.z);
	}


	public void updatePos(Vector3 posOffset) {
		PAnim anim = GetComponent<PAnim>();
		anim.translate(camobj, posOffset);
		anim.translate(lightobj, posOffset);
	}
}
