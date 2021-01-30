using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float hp = 100;
    public float hunger = 100;
    private PlayerSound playerSound;

    void Start()
    {
        playerSound = GetComponent<PlayerSound>();
    }
    void Update()
    {
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
}
