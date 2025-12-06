using UnityEngine;
using System.Collections;

namespace NPCWalk { 

[RequireComponent(typeof(Rigidbody2D))]
public class NPCWalk : MonoBehaviour
{
    public float npc_speed = 2.0f;
    public float dir_change_interv = 1.0f;
    

    Rigidbody2D rb2d;
    int direction = 1;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        StartCoroutine(ChangeDirection());
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        direction *= -1;
    }

    void FixedUpdate()
    {
        rb2d.linearVelocity = new Vector2(direction * npc_speed, rb2d.linearVelocity.y);
    }

    System.Collections.IEnumerator ChangeDirection()
    {
        while (true)
        {
            yield return new WaitForSeconds(dir_change_interv);
            direction *= Random.value > 0.5f ? 1 : -1;
        }
    }
}

}