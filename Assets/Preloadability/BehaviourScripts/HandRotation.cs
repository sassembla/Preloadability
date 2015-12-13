using UnityEngine;

public class HandRotation : MonoBehaviour {
	private Vector3 startPos = Vector3.zero;

	public void Start () {
		startPos = transform.position;
	}

	public void Update () {
		transform.position = transform.position + new Vector3(0.6f, 0, 0);
		if (startPos.x + 20 < transform.position.x) transform.position = startPos;
	}
}