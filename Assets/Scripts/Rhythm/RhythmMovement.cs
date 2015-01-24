﻿using UnityEngine;
using System.Collections;

public class RhythmMovement : Photon.MonoBehaviour
{
	public float myCompassBeat = 1f;
	public float cubesPerBeat = 2f;
	public float velocity = 2f;

	private Vector3 startPosition = Vector2.zero;
	private Vector3 endPosition = Vector2.zero;

	private double lastBeat;
	private Vector3 correctPlayerPos = Vector3.zero;
	private Quaternion correctPlayerRot = Quaternion.identity;
	private Vector3 movementDirection = new Vector3();
	
	private ModelController model;
	
	static private Color[] playerColors = { Color.red, Color.green, Color.yellow, Color.blue };

	void Start ()
	{
		startPosition = transform.position;
		endPosition = transform.position;
		movementDirection = Vector3.zero;

		// Set Colors
		int color = photonView.owner.ID % playerColors.Length;
		model = GetComponentInChildren<ModelController>();
		model.SetColor(playerColors[color]);
		
		foreach (Light l in GetComponentsInChildren<Light>()) {
			l.color = playerColors[color];
		}

		if (!photonView.isMine)
			GetComponent<CharacterController>().enabled = false;
	}
	
	void Update ()
	{
		double beatTime = MusicBeat.BeatTime;
		float currentBeatFloor = Mathf.Floor((float) beatTime);
		
		if (!photonView.isMine)
		{
			transform.position = Vector3.Lerp(transform.position, this.correctPlayerPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot, Time.deltaTime * 5);
		}
		else
		{
			if (Mathf.Floor((float)lastBeat) != currentBeatFloor)
			{
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
				movementDirection = endPosition - startPosition;
			}

			lastBeat = beatTime;
			float time = (float) beatTime - Mathf.Floor((float)beatTime);
			time = Easing.Circular.Out(time);
			CharacterController controller = GetComponent<CharacterController>();

			if (startPosition != endPosition)
				controller.Move(Vector3.Lerp(startPosition, endPosition, time) - transform.position);
		}
		
		float speedFactor = (float) beatTime - Mathf.Floor ((float)beatTime);
		speedFactor = 1.0f - Easing.Circular.Out(speedFactor);
		model.SetAnimationState(speedFactor, movementDirection);
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(endPosition - startPosition);
		}
		else
		{
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			correctPlayerRot = (Quaternion)stream.ReceiveNext();
			movementDirection = (Vector3)stream.ReceiveNext();
		}
	}
}
