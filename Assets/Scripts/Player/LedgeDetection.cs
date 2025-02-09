using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeDetection : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] Player player;

    bool canDetect = true;

    public BoxCollider2D boxCd;

    private void Update()
    {
        if (canDetect)
        player.ledgeDetected = Physics2D.OverlapCircle(transform.position, radius, whatIsGround);
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            canDetect = false;

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(boxCd.bounds.center, boxCd.size, 0);

        foreach (var hit in colliders)
        {
            if (hit.gameObject.GetComponent<PlatformController>() != null)
                return;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            canDetect = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
