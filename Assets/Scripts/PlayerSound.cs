using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public AudioClip trapSound;
    public AudioClip dyingSound;
    public AudioClip eatingSound;
    public AudioClip attackSound;
    public AudioClip gameWinSound;
    public AudioClip gameOverSound;
    public AudioClip enemySound;

    public void playTrapSound()
    {
        AudioSource.PlayClipAtPoint(trapSound, transform.position, 0.5f);
    }
    public void playDyingSound()
    {
        AudioSource.PlayClipAtPoint(dyingSound, transform.position);
    }

    public void playEatingSound()
    {
        AudioSource.PlayClipAtPoint(eatingSound, transform.position, 0.5f);
    }
    public void playAttackSound()
    {
        AudioSource.PlayClipAtPoint(attackSound, transform.position, 0.1f);
    }
    public void playGameWinSound()
    {
        AudioSource.PlayClipAtPoint(gameWinSound, transform.position, 0.3f);
    }
    public void playGameOverSound()
    {
        AudioSource.PlayClipAtPoint(gameOverSound, transform.position, 0.5f);
    }
    public void playEnemySound()
    {
        AudioSource.PlayClipAtPoint(enemySound, transform.position);
    }
}
