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

    void Start()
    {
        conductor = Conductor.conductorInstance;
        conductor.resetSong += DestroySelf;
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

    public void DestroySelf()
    {
        // All particle effects, sounds, death should be handled here.
        
        // unsubscribe from Action in conductor.
        conductor.resetSong -= DestroySelf;
        
        // only spot that notedequeue should be called from.
        noteSpawnerInstance?.NoteDequeue();
        Destroy(gameObject);
    }
    
    // Update should move the note position and test if it is already past its time, and if it is dequeue it and destroy
    void Update()
    {
        // this needs to change when Lerp works!!!
        if (conductor.SongPosition > hitTime)
        {
            // destroy the object, it has reached the end.
            DestroySelf();
        }
        float pct = (float)(conductor.SongPosition - spawnTime) / (hitTime - spawnTime);
        print("spawn time: " + spawnTime + "song Pos: " + conductor.SongPosition + " hitTime: " + 
              hitTime + "pct: " + pct);
        gameObject.transform.position = Vector3.LerpUnclamped(noteSpawnerInstance.GetSpawnPosition(),
            noteSpawnerInstance.GetHitPosition(), pct);
    }
}
