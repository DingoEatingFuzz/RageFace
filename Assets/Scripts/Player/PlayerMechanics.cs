﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMechanics : MonoBehaviour {

	public float punchForce = 50;
	public float punchRange = 10;
    public Camera mainCamera;

	private Animator anim;
    public float shakeAmt = 1.5f;
    private Vector3 originalCameraPosition;
	
	private AudioSource punchAudioSource;
	public AudioClip punchNoise;

	// Use this for initialization
	void Start () {
		anim = this.GetComponent<Animator>();	
	}

	void Awake () {
        punchAudioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			anim.SetTrigger("Punch");
			this.Punch();
			punchAudioSource.PlayOneShot(punchNoise,50);
		}
	}

	void Punch() {
		var origin = transform.position;
		var offset = transform.right * 1.13f;

		var hits = new HashSet<RaycastHit2D>();
		foreach (var hit in Physics2D.RaycastAll(origin, transform.up, punchRange)) {
			hits.Add(hit);
		}
		foreach (var hit in Physics2D.RaycastAll(origin + offset, transform.up, punchRange)) {
			hits.Add(hit);
		}
		foreach (var hit in Physics2D.RaycastAll(origin + -offset, transform.up, punchRange)) {
			hits.Add(hit);
		}
		
		foreach(var hit in hits) {
			if (hit.transform == transform) { continue; }

			var rb = hit.transform.gameObject.GetComponent<Rigidbody2D>();
			if (rb) {
				rb.AddForce(
					new Vector2(transform.up.x, transform.up.y) * punchForce,
					ForceMode2D.Impulse
				);
			}
		}

		originalCameraPosition = mainCamera.transform.position;
        InvokeRepeating("CameraShake", 0, .01f);
        Invoke("StopShaking", 0.3f);
	}

    void CameraShake()
    {
		float quakeAmtX = Random.value*shakeAmt*2 - shakeAmt;
		float quakeAmtY = Random.value*shakeAmt*2 - shakeAmt;
		mainCamera.transform.position += new Vector3(quakeAmtX, quakeAmtY, 0);
    }

    void StopShaking()
    {
        CancelInvoke("CameraShake");
        // mainCamera.transform.position = originalCameraPosition;
    }
}