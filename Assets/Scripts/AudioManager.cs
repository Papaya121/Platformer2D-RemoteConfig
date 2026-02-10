using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioConfig audioConfig;

    private static Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();

    void Awake()
    {
        if(audioConfig != null)
        {
            SpawnAudioSources();
        }
    }

    public static void PlaySound(string id, float randomPitchRange = 0f)
    {
        if (audioSources.ContainsKey(id))
        {
            if (randomPitchRange > 0f)
                audioSources[id].pitch = 1f + Random.Range(-randomPitchRange, randomPitchRange);
            else
                audioSources[id].pitch = 1f;
                
            audioSources[id].Play();
        }
        else
        {
            Debug.LogWarning($"AudioManager: Sound with id '{id}' not found!");
        }
    }

    private void SpawnAudioSources()
    {
        audioSources.Clear();
        var root = new GameObject("AudioSources");

        foreach (var entry in audioConfig.AudioClips)
        {
            var audioSourceObject = new GameObject(entry.id, typeof(AudioSource));
            audioSourceObject.transform.SetParent(root.transform, false);
            var source = audioSourceObject.GetComponent<AudioSource>();
            source.clip = entry.clip;
            source.playOnAwake = false;

            audioSources.Add(entry.id, source);
        }
    }
}