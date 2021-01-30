using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public AudioClip trapSound;
    public AudioClip dyingSound;
    public AudioClip eatingSound;
    public AudioClip attackSound;

    public void playTrapSound()
    {
        AudioSource.PlayClipAtPoint(trapSound, transform.position);
    }
    public void playDyingSound()
    {
        AudioSource.PlayClipAtPoint(dyingSound, transform.position);
    }

    public void playEatingSound()
    {
        AudioSource.PlayClipAtPoint(eatingSound, transform.position);
    }
    public void playAttackSound()
    {
        AudioSource.PlayClipAtPoint(attackSound, transform.position);
    }
}
