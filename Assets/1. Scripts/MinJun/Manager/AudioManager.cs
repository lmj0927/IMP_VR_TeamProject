using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private SerializedDictionary<AudioType, AudioClip> audioClips = new SerializedDictionary<AudioType, AudioClip>();
    [SerializeField] private List<AudioSource> audioSources;


    public void PlaySound(AudioType type)
    {
        if (!audioClips.TryGetValue(type, out AudioClip clip))
            return;

        var volume = 0.5f;

        if (type == AudioType.Crying)
        {
            var source = audioSources[0];
            source.clip = clip;
            source.loop = true;
            source.volume = volume;
            source.Play();
            return;
        }

        if (audioSources[0].isPlaying)
            audioSources[1].PlayOneShot(clip, volume);
        else
            audioSources[0].PlayOneShot(clip, volume);
    }

    public void StopSound()
    {
        foreach (var audioSource in audioSources)
        {
            audioSource.Stop();
        }
    }
}

public enum AudioType
{
    Crying,

}