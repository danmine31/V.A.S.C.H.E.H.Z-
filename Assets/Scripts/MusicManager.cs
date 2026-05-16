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

    void PlayTrack(int index)
    {
        CancelInvoke(nameof(NextTrack));

        audioSource.clip = playlist[index];
        audioSource.Play();
        Debug.Log($"Сейчас играет трек: {playlist[index].name}");

        Invoke(nameof(NextTrack), playlist[index].length);
    }

    void NextTrack()
    {
        currentTrackIndex = (currentTrackIndex + 1) % playlist.Count;
        PlayTrack(currentTrackIndex);
    }
}