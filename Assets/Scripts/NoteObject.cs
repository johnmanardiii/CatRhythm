using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    private Conductor conductor;
    private float hitTime;
    private float spawnTime;
    private float estimatedLifeExpectancy;
    private NoteSpawner noteSpawnerInstance = null;

    [SerializeField] private GameObject explosionParticles;
    [SerializeField] private AudioClip perfectSound;
    [SerializeField] private float perfectSoundVolume = 1.0f;

    void Start()
    {
        conductor = Conductor.conductorInstance;
        conductor.resetSong += OnReset;
    }

    public float GetHitTime()
    {
        return hitTime;
    }

    public void InitializeNote(float arrivalTime, float spawnTimeSpawner, NoteSpawner noteSpawner)
    {
        noteSpawnerInstance = noteSpawner;
        this.hitTime = arrivalTime;
        this.spawnTime = spawnTimeSpawner;

        float distanceMultiplier = (Vector3.Distance(noteSpawnerInstance.GetDeathPosition(),
                                        noteSpawnerInstance.GetSpawnPosition()) /
                                    Vector3.Distance(noteSpawnerInstance.GetSpawnPosition(),
                                        noteSpawnerInstance.GetHitPosition()));
        estimatedLifeExpectancy = (hitTime - spawnTime) * distanceMultiplier;
    }

    public void OnReset()
    {
        Destroy(gameObject);
    }

    public void DestroySelf(NoteSpawner.HitType type)
    {
        // type to be used for different sound effects.
        // unsubscribe from Action in conductor.
        conductor.resetSong -= OnReset;
        // only spot that notedequeue should be called from.
        // note objects tell the manager objects of their state, including a score object and the queue.
        Instantiate(explosionParticles, transform.position, Quaternion.identity);
        if (type == NoteSpawner.HitType.Perfect)
        {
            AudioSource.PlayClipAtPoint(perfectSound, transform.position, perfectSoundVolume);
        }
        Destroy(gameObject);
    }
    
    // Update should move the note position and test if it is already past its time, and if it is dequeue it and destroy
    void Update()
    {
        // this needs to change when Lerp works!!!
        // note, if direction relative to start position and end position changes, this value needs to be inverted.
        if (gameObject.transform.position.x < noteSpawnerInstance.GetDeathPosition().x)
        {
            // destroy the object, it has reached the end.
            noteSpawnerInstance.NoteDequeue(NoteSpawner.HitType.Fail);
        }
        float pct = (float)(conductor.SongPosition - spawnTime) / (hitTime - spawnTime);
        gameObject.transform.position = Vector3.LerpUnclamped(noteSpawnerInstance.GetSpawnPosition(),
            noteSpawnerInstance.GetHitPosition(), pct);
    }
}
