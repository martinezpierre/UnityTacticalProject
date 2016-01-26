using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {

    public static SoundManager instance = null;
    public static SoundManager Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
    }

    public AudioClip mainTheme;

    public List<AudioClip> attackSounds;
    public List<AudioClip> laserSounds;
    public List<AudioClip> invocSounds;
    public AudioClip reducDamageSound;
    public AudioClip healSound;
    public AudioClip buffSound;
    public AudioClip stunSound;
    public AudioClip teleportationSound;

    // Use this for initialization
    void Start () {
        AudioSource aS = gameObject.AddComponent<AudioSource>();
        aS.clip = mainTheme;
        aS.loop = true;
        aS.Play();
	}

    public AudioClip GetAttackSound()
    {
        int n = Random.Range(0, attackSounds.Count);

        return attackSounds[n];
    }

    public AudioClip GetLaserSound()
    {
        int n = Random.Range(0, laserSounds.Count);

        return laserSounds[n];
    }

    public AudioClip GetInvocSound()
    {
        int n = Random.Range(0, invocSounds.Count);

        return invocSounds[n];
    }

	// Update is called once per frame
	void Update () {
	
	}
}
