﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatCircle : MonoBehaviour
{

    private Conductor conductor;
    private float lastbeat = 0f;
    private SpriteRenderer spriteRenderer;
    public AudioClip blip;
    [Range(0.0f, 1.0f)] [SerializeField] private float volume = 1.0f;

    void Start()
    {
        conductor = Conductor.conductorInstance;
        conductor.resetSong += OnReset;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnReset()
    {
		// honestly, almost all things should have this, So I will probably make it an interface that 
		// is required by all objects subscribed to Conductor.
        lastbeat = 0f;
        // probably play the destroy animation from here on the circle.
    }

    private void OnDestroy()
    {
        conductor.resetSong -= OnReset;
    }

    void Update()
    {
        // need some way of reseting this 
        if (conductor.SongPosition > lastbeat + conductor.Crotchet)
        {
            AudioSource.PlayClipAtPoint(blip, transform.position, volume);
            lastbeat += conductor.Crotchet;
        }
    }
}
