using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [Header("Плейлист из FL Studio")]
    public List<AudioClip> playlist;

    private AudioSource audioSource;
    private int currentTrackIndex = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (playlist != null && playlist.Count > 0)
        {
            PlayTrack(currentTrackIndex);
        }
        else
        {
            Debug.LogWarning("Бро, ты забыл закинуть треки в список MusicManager в Инспекторе!");
        }
    }

    void Update()
    {
        if (playlist.Count > 0 && !audioSource.isPlaying)
        {
            NextTrack();
        }
    }

    void PlayTrack(int index)
    {
        audioSource.clip = playlist[index];
        audioSource.Play();
        Debug.Log($"Сейчас играет трек: {playlist[index].name}");
    }

    void NextTrack()
    {
        currentTrackIndex = (currentTrackIndex + 1) % playlist.Count;
        PlayTrack(currentTrackIndex);
    }
}