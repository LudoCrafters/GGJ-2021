using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float hp = 100;
    public float hunger = 100;
    private PlayerSound playerSound;
    public int toFindBabyCount = 2;
    private int currentBabyCount = 0;

    void Start()
    {
        playerSound = GetComponent<PlayerSound>();
    }
    void Update()
    {
        // 배고파짐
        hunger -= 0.1f * Time.deltaTime;

        // 너무 배고프면 hp가 깎임
        if (hunger <= 0)
        {
            hunger = 0;
            hp -= 1f * Time.deltaTime;
        }

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
    public void eat(int amount)
    {
        hunger = Mathf.Min(hunger + amount, 100);
        playerSound.playEatingSound();
    }

    public void findBaby()
    {
        ++currentBabyCount;

        if (currentBabyCount >= toFindBabyCount)
        {
            // TODO
            // GAME WINS
            playerSound.playGameWinSound();
        }
    }
}
