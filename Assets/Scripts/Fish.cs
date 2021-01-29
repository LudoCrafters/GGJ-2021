using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public Player player;

    private void OnTriggerEnter(Collider other)
    {
        player.eat();

        Destroy(this.gameObject);
    }
}
