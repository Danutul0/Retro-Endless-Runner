using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public Player player;
    public float speedBoost;
    public float chanceToSpawn = 70;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();

        bool canSpawn = chanceToSpawn >= Random.Range(0, 100);

        if (!canSpawn)
            Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player.moveSpeed = player.moveSpeed + speedBoost;

            StartCoroutine(DestroySpeedBoost());
            
        }
    }

    IEnumerator DestroySpeedBoost()
    {
        yield return new WaitForSeconds(0.05f);
        Destroy(gameObject);
    }
}
