using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio Config", menuName = "Configs/Audio Config")]
public class AudioConfig : ScriptableObject
{
    [SerializeField] private AudioClipEntry[] audioClips;
    public AudioClipEntry[] AudioClips => audioClips;

    [Serializable]
    public class AudioClipEntry
    {
        public string id;
        public AudioClip clip;
    }
}