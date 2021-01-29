using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float hp = 100;
    private PlayerSound playerSound;

    void Start()
    {
        playerSound = GetComponent<PlayerSound>();
    }
    void Update()
    {
        // die
        if (hp <= 0)
        {
            hp = 0;
            playerSound.playDyingSound();

            // TODO
            // GAME OVER
        }
    }

    public void hurt()
    {
        hp -= 50;
        playerSound.playTrapSound();
    }
}
