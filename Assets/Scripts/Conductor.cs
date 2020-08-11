using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// maybe a reference to this should be made static to avoid find component calls.
public class Conductor : MonoBehaviour
{
    
    // Move all of these class attributes to a separate ScriptableObject to be used.
    public event Action resetSong;

    [SerializeField] private float bpm = 120f;
    // Duration of the beat, calculated by bpm.
    [SerializeField] private float crotchet;
    [SerializeField] private float offset = 0f;

    [SerializeField] private int beatsPerMeasure = 4;
    // AudioSource and AudioClip should be moved to the ScriptableObject later
    private AudioSource song;

    // non-song specific class variables.
    // position of the song in dsp-time
    private double songPosition = 0.0;
    private double timeStarted;
    private Coroutine dspIncrementor;
    private bool songStarted = false;

    // Engine-specific properties.
    // Time given to load in ms for dsp-time.
    [SerializeField] private float loadDelay = 0.032f;

    public double SongPosition => songPosition;

    public float Crotchet => crotchet;

    void Awake()
    {
        // seconds per beat. (duration of beat in seconds.
        crotchet = 1.0f / (bpm / 60.0f);
    }

    public float GetMeasurePosition()
    {
        // returns the current position in the measure as a value 0-1
        // songpos / dur gives you how many have passed followed by a fraction, which represents the
        // position of the song in the measure.
        float measurePosition = ((float)songPosition / (beatsPerMeasure * crotchet));
        return measurePosition - Mathf.Floor(measurePosition);
    }

    private void Update()
    {
        if (!song.isPlaying && songStarted)
        {
            PlaySong();
        }
    }

    public float GetBeatPosition()
    {
        float measurePosition = ((float)songPosition / (beatsPerMeasure * (crotchet / 4.0f)));
        return measurePosition - Mathf.Floor(measurePosition);
    }

    void Start()
    {
        song = GetComponent<AudioSource>();
    }

    public void PlaySong()
    {
        songStarted = true;
        resetSong?.Invoke();
        double expectedDsp = AudioSettings.dspTime + loadDelay;
        song.PlayScheduled(expectedDsp);
        print("current dsp: " + AudioSettings.dspTime + " expected dsp: " + expectedDsp);
        StartCoroutine(WaitForSong(expectedDsp));
    }

    IEnumerator WaitForSong(double expectedTime)
    {
        while (AudioSettings.dspTime < expectedTime)
        {
            // wait for the frame after the expected time.
            yield return null;
        }
        // adjust for delay and stepping behavior of dspTime.
        songPosition = (float)(AudioSettings.dspTime - expectedTime);
        timeStarted = expectedTime;
        // songPosition should now be updated every frame.
        dspIncrementor = StartCoroutine(UpdateSongPosition());
    }

    IEnumerator UpdateSongPosition()    
    {
        while (true)
        {
            songPosition = AudioSettings.dspTime - timeStarted - offset;
            yield return null;
        }
    }

    public void StopSong()
    {
        songStarted = false;
        song.Stop();
        songPosition = 0;
        if (dspIncrementor != null)
        {
            StopCoroutine(dspIncrementor);
            dspIncrementor = null;
        }
    }
}
