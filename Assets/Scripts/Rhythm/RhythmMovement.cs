using UnityEngine;
using System.Collections;

public class RhythmMovement : Photon.MonoBehaviour
{
	public float myCompassBeat = 1f;
	public float cubesPerBeat = 2f;
	public float velocity = 2f;

	private bool move = false;
	private float beatTime = 0f;

	private Vector3 startPosition;
	private Vector3 endPosition;

	private float previousEventTime;
	private float nextEventTime;

	private Vector3 correctPlayerPos = Vector3.zero;
	private Quaternion correctPlayerRot = Quaternion.identity;

	static private Color[] playerColors = { Color.red, Color.green, Color.yellow, Color.blue };

	// Use this for initialization
	void Start ()
	{
		startPosition = transform.position;
		endPosition = transform.position;

		previousEventTime = (float)AudioSettings.dspTime;
		nextEventTime = (float)AudioSettings.dspTime;

		Debug.Log(photonView.owner.ID);
		int color = photonView.owner.ID % playerColors.Length;

		ModelController model = GetComponentInChildren<ModelController>();
		model.SetColor(playerColors[color]);

		if (!photonView.isMine)
			GetComponent<CharacterController>().enabled = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!photonView.isMine)
		{
			transform.position = Vector3.Lerp(transform.position, this.correctPlayerPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot, Time.deltaTime * 5);
		}
		else
		{
			if (move)
			{
				move = false;
				Vector3 position = -Vector3.up;

				position += transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");

				/*
				if(Input.GetKey(KeyCode.A)) position -= transform.right;
				if(Input.GetKey(KeyCode.D)) position += transform.right;
				if(Input.GetKey(KeyCode.S)) position -= transform.forward;
				if(Input.GetKey(KeyCode.W)) position += transform.forward;
				*/

				startPosition = transform.position;
				endPosition = startPosition + position.normalized * cubesPerBeat;
			}

			float time = Mathf.Min(1.0f, ((float)AudioSettings.dspTime - previousEventTime) / (nextEventTime - previousEventTime));
			time = Easing.Circular.Out(time);
			CharacterController controller = GetComponent<CharacterController>();

			if (startPosition != endPosition)
				controller.Move(Vector3.Lerp(startPosition, endPosition, time) - transform.position);
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
		}
		else
		{
			this.correctPlayerPos = (Vector3)stream.ReceiveNext();
			this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
		}
	}


	public void Beat(Vector2 data) {
		//Debug.Log (data.x + " " + beatTime);
		if(data.x == beatTime) {
			move = true;
			beatTime += 1f/myCompassBeat;
			previousEventTime = data.y;
			nextEventTime = previousEventTime + (60f/(MusicBeat.beatsPerMinute*(MusicBeat.compassDenominator/MusicBeat.compassNumerator)*myCompassBeat*velocity));

		}
	}
}
