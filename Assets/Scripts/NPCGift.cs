using UnityEngine;
using System.Collections;

namespace NPCGift {

[RequireComponent(typeof(Rigidbody2D))]
public class NPCGift : MonoBehaviour
    {

        void Awake()
        {
            return;
        }

        //Assuming tag is gift for Gift objects
        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Gift"))
            {
                HandleGiftInteraction(collision.gameObject);
            }
        }

        void HandleGiftInteraction(GameObject gift)
        {
            Destroy(gift);
        }

    }

}
