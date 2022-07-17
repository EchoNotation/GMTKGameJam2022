using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Sound
{
    public const int STEP1 = 0;
    public const int STEP2 = 1;
    public const int AXE_SWING = 2;
    public const int AXE_HIT = 3;
    public const int RIFLE = 4;
    public const int RIFLE_RELOAD = 5;
    public const int SHOTGUN = 6;
    public const int DASH = 7;
    public const int DASH_HIT = 8;
    public const int CLICK = 9;
    public const int DICE_REFRESH = 10;
    public const int ZOMBIE_BITE = 11;
}

public class SoundController : MonoBehaviour
{
    public AudioSource[] sources = new AudioSource[12];

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);

        GameObject[] sounders = GameObject.FindGameObjectsWithTag("SoundController");
        if (sounders.Length > 1) Destroy(gameObject);
    }

    public void PlaySound(int soundID)
    {
        if (sources[soundID].isPlaying) sources[soundID].Stop();
        sources[soundID].Play();
    }

}
