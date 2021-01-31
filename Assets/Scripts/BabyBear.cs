using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyBear : MonoBehaviour
{
    private Player player;
    private float time;
    public AudioClip babySound;

    void Start()
    {
        time = 0.0f;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void Update()
    {
        time += Time.deltaTime;
        if(time > 3.0f)
        {
            time = 0.0f;
            playBabySound();
        }
    }

    public void playBabySound()
    {
        AudioSource.PlayClipAtPoint(babySound, transform.position, 1.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        player.findBaby();

        Destroy(this.gameObject);
    }
}
