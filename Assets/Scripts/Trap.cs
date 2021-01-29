using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public Player player;
    private Animator trapAnimator;
    private bool isActive = false;

    void Start()
    {
        trapAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive)
        {
            trapAnimator.SetTrigger("Active");
            player.hurt();

            isActive = true;
        }
    }
}
