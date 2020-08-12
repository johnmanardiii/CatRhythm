using System;
using UnityEngine;

public class BeatMap : MonoBehaviour
{
	[SerializeField] TextAsset beatMapFile = null;
	private float[] beatTimes;

	private Conductor conductor;
	private int lastBeatIndex = 0;
	public AudioClip blip;
	[SerializeField] float beatOffset = 0.0f;
	[Range(0.0f, 1.0f)] [SerializeField] private float volume = 1.0f;

	private void Start()
	{
		InitializeBeatTimes();
		conductor = FindObjectOfType<Conductor>();
		conductor.resetSong += OnReset;
	}

	private void Update()
	{
		// how far in advance should notes be spawned? (dependent on speed)
		// circles should manage their own positions.
		// how to calculate the position of a note based on where it should be at a certain time
		// and move it forward out of the screen at the same velocity? (Vector3.Lerp)
		// basically, an equation relating the motion of the note to the position of the song.
		if (lastBeatIndex < beatTimes.Length && conductor.SongPosition > beatTimes[lastBeatIndex])
		{
			AudioSource.PlayClipAtPoint(blip, transform.position, volume);
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
		for(int i = 0; i < times.Length; i++)
		{
			beatTimes[i] = ((float)Int64.Parse(times[i]) / 1000.0f) + beatOffset;
		}
	}
}
