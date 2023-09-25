using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Before Game Sound
    public AudioClip openingBgm;
    public AudioClip lobbyBgm;
    public AudioClip patritBgm;
    
    public AudioClip startSound;
    public AudioClip clickSound;

    public AudioSource loginSource;
    public AudioSource lobbySource;
    public AudioSource patritSource;

    public AudioSource clickSource;

    // In Game Sound
    public AudioClip bombSet;
    public AudioClip bombExplosion;
    public AudioClip bombLocked;
    public AudioClip bombDead;

    public AudioClip getItem;
    public AudioClip lose;
    public AudioClip win;

    // public AudioSource bombSetSound;
    // public AudioSource bombExplosionSound;
    // public AudioSource bombLockedSound;
    // public AudioSource bombDeadSound;
    // 
    // public AudioSource getItemSound;
    // public AudioSource loseSound;
    // public AudioSource winSound;

    private void Awake()
    {
        openingBgm = GameManager.Resource.Load<AudioClip>("Sound/Opening");
        lobbyBgm = GameManager.Resource.Load<AudioClip>("Sound/Lobby");
        patritBgm = GameManager.Resource.Load<AudioClip>("Sound/Patrit");

        startSound = GameManager.Resource.Load<AudioClip>("Sound/StartButton");
        clickSound = GameManager.Resource.Load<AudioClip>("Sound/ClickSound");

        loginSource = gameObject.AddComponent<AudioSource>();
        lobbySource = gameObject.AddComponent<AudioSource>();
        patritSource = gameObject.AddComponent<AudioSource>();

        clickSource = gameObject.AddComponent<AudioSource>();

        loginSource.clip = openingBgm;
        lobbySource.clip = lobbyBgm;
        patritSource.clip = patritBgm;

        clickSource.clip = clickSound;

        bombSet = GameManager.Resource.Load<AudioClip>("Sound/BombSet");
        bombExplosion = GameManager.Resource.Load<AudioClip>("Sound/BombExplosion");
        bombLocked = GameManager.Resource.Load<AudioClip>("Sound/BombLocked");
        bombDead = GameManager.Resource.Load<AudioClip>("Sound/BombDead");

        getItem = GameManager.Resource.Load<AudioClip>("Sound/GetItem");
        lose = GameManager.Resource.Load<AudioClip>("Sound/Lose");
        win = GameManager.Resource.Load<AudioClip>("Sound/Win");

        // bombSetSound = gameObject.AddComponent<AudioSource>();
        // bombExplosionSound = gameObject.AddComponent<AudioSource>();
        // bombLockedSound = gameObject.AddComponent<AudioSource>();
        // bombDeadSound = gameObject.AddComponent<AudioSource>();
        // 
        // getItemSound = gameObject.AddComponent<AudioSource>();
        // loseSound = gameObject.AddComponent<AudioSource>();
        // winSound = gameObject.AddComponent<AudioSource>();
    }
    public void SFXPlay(string sfxName, AudioClip clip)
    {
        GameObject go = new GameObject(sfxName + "Sound");
        AudioSource audiosource = go.AddComponent<AudioSource>();
        audiosource.clip = clip;
        audiosource.Play();

        Destroy(go, clip.length);
    }

    public void BgmPlay(AudioSource audioSource)
    {
        audioSource.loop = true;
        audioSource.Play();
    }

    public void BgmStop(AudioSource audioSource)
    {
        audioSource.Stop();
    }

    public void Onclick()
    {
        clickSource.loop = false;
        clickSource.Play();
    }
}
