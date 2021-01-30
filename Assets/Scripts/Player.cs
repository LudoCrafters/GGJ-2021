using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float hp = 100;
    public float hunger = 100;
    private PlayerSound playerSound;
    public int toFindBabyCount = 2;
    public int currentBabyCount = 0;

    private bool died = false;

    void Start()
    {
        playerSound = GetComponent<PlayerSound>();
    }
    void Update()
    {
        if (died)
        {
            return;
        }

        // �������
        hunger -= 0.1f * Time.deltaTime;

        // �ʹ� ������� hp�� ����
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
            playerSound.playGameOverSound();

            died = true;
        }
    }

    public void hurt(float damage)
    {
        hp -= damage;
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
