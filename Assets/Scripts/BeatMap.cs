using System;
using UnityEngine;

[RequireComponent(typeof(NoteSpawner))]
public class BeatMap : MonoBehaviour
{
	[SerializeField] TextAsset beatMapFile = null;
	private float[] beatTimes;

	private Conductor conductor;
	private int lastBeatIndex = 0;
	public AudioClip blip;
	[SerializeField] float beatOffset = 0.0f;
	[Range(0.0f, 1.0f)] [SerializeField] private float volume = 1.0f;

	private NoteSpawner noteSpawner;
	// spawning is based on time proximity, not note proximity to ensure constant speed.
	// smaller value = faster notes, larger value = slower notes.
	// can later add speed modifier to make the game more customizable.
	[SerializeField] private float timeAheadOfSpawn = 1.0f;

	private void Start()
	{
		InitializeBeatTimes();
		conductor = Conductor.conductorInstance;
		conductor.resetSong += OnReset;
		noteSpawner = GetComponent<NoteSpawner>();
		noteSpawner.SetStartUpTime(timeAheadOfSpawn);
	}

	private void Update()
	{
		// how far in advance should notes be spawned? (dependent on speed)
		// circles should manage their own positions.
		// how to calculate the position of a note based on where it should be at a certain time
		// and move it forward out of the screen at the same velocity? (Vector3.Lerp)
		// basically, an equation relating the motion of the note to the position of the song.
		
		// this class will tell NoteSpawner to spawn each note when it is within the time to be spawned.
		// conductor needs to be able to start at a negative time relative to the actual song position to allow
		// notes to be placed earlier in relation to the timeAheadOfSpawn variable.
		if (lastBeatIndex < beatTimes.Length &&
		    conductor.SongPosition + timeAheadOfSpawn > beatTimes[lastBeatIndex])
		{
			noteSpawner.SpawnNote(beatTimes[lastBeatIndex]);
			lastBeatIndex++;
		}
	}

	void OnReset()
	{
		lastBeatIndex = 0;
	}

	private void InitializeBeatTimes()
	{
		string timesText = beatMapFile.ToString();
		string[] times = timesText.Split(new char[] { ' ', '\t', '\r',
			'\n'}, StringSplitOptions.RemoveEmptyEntries);
		beatTimes = new float[times.Length];
		for(var i = 0; i < times.Length; i++)
		{
			beatTimes[i] = (Int64.Parse(times[i]) / 1000.0f) + beatOffset;
		}
	}
}
