using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGenerator : MonoBehaviour
{
    [SerializeField] int amountOfCoins;
    [SerializeField] GameObject coinPrefab;

    [SerializeField] int minCoins;
    [SerializeField] int maxCoins;

    [SerializeField] private SpriteRenderer[] coinImg;
    void Start()
    {
        for (int i = 0; i < coinImg.Length; i++)
            coinImg[i].sprite = null;

        amountOfCoins = Random.Range(minCoins, maxCoins);

        for (int i = 0; i < amountOfCoins; i++)
        {
            int additionalOffset = amountOfCoins / 2;

            Vector3 offset = new Vector2(i - additionalOffset, 0);
            Instantiate(coinPrefab, transform.position + offset, Quaternion.identity, transform);
        }
    }

}
