using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;

    int bgmIndex;

    private void Awake() => instance = this;

    private void Update()
    {
        if (!bgm[bgmIndex].isPlaying)
            PlayRandomBGM();
    }
    public void PlayRandomBGM()
    {
        bgmIndex = (Random.Range(0, bgm.Length));

        PlayBGM(bgmIndex);
    }

    public void PlaySFX(int Index)
    {
        if (Index < sfx.Length)
        {
            sfx[Index].pitch = Random.Range(0.85f, 1.2f);
            sfx[Index].Play();
        }
    }

    public void StopSFX(int Index)
    {
        sfx[Index].Stop();
    }

    public void PlayBGM(int index)
    {
        StopBGM();
        bgm[index].Play();

    }

    public void StopBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[(i)].Stop();
        }
    }

}
