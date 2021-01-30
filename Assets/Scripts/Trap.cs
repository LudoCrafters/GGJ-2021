using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private Player player;
    private Animator trapAnimator;
    private bool isActive = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        trapAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive)
        {
            trapAnimator.SetTrigger("Active");
            player.hurt(50.0f);

            isActive = true;
        }
    }
}
