using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

/*
 * This class is in charge of managing each of the different notes to be spawned and intitalizing them with the
 * correct data from BeatMap.cs
 */
public class NoteSpawner : MonoBehaviour
{
    private Conductor conductor;
    private BeatMap beatMap;
    
    // Queue that stores all currently active objects.
    private Queue<NoteObject> noteQueue;
    
    // might want to have beatBar as a child of the noteSpawner or vice versa such that this is automatically
    // intitalized in code. (Less error prone, more customizable maps.)
    [SerializeField] private GameObject spawnPosition;
    [SerializeField] private GameObject hitPosition;
    [SerializeField] private GameObject deathPosition;
    [SerializeField] private GameObject noteObjectPrefab;
    
    private float startUpTime = 0.0f;

    public Vector3 GetSpawnPosition() => spawnPosition.transform.position;

    public Vector3 GetHitPosition() => hitPosition.transform.position;
    
    public Vector3 GetDeathPosition() => deathPosition.transform.position;

    void Start()
    {
        conductor = Conductor.conductorInstance;
        noteQueue = new Queue<NoteObject>();
    }

    public void SetStartUpTime(float noteTravelTime)
    {
        this.startUpTime = noteTravelTime;
    }

    public NoteObject PeekNoteQueue()
    {
        return noteQueue.Peek();
    }

    public void NoteDequeue()
    {
        // note: using a queue forces the restriction in NoteObject that notes can only be destroyed in order.
        NoteObject note = noteQueue.Dequeue();
    }

    /**
     * Spawns a note to be clicked at the dspTime hitTime.
     * New note will start at spawnPosition and manage itself until deathPosition
     * estimated time to reach deathPosition will be timed based on
     */
    public void SpawnNote(float hitTime)
    {
        // note will handle updating position
        // noteSpawner will handle all gameobjects in a Queue system that allows the first note to be detected
        // if within a certain time range to its desired hit Time specified by hitTime
        NoteObject note = Instantiate(noteObjectPrefab, spawnPosition.transform.position,
            Quaternion.identity).GetComponent<NoteObject>();
        // need to initialize note manually.
        note.InitializeNote(hitTime, hitTime - startUpTime, this);
        noteQueue.Enqueue(note);
    }
    
    
}
