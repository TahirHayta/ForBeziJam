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

        private void UpdateOnGiftCollected(NPCBehaviour NPC)
        {
            switch(NPC.giftCount)
            {
                case 1:
                    NPC.npc_speed += 1.0f;
                    NPC.npc_scarability -= 0.25f;
                    break;
                case 2:
                    NPC.npc_speed += 1.0f;
                    NPC.npc_scarability -= 0.25f;
                    break;
                case 3:
                    NPC.npc_speed += 1.0f;
                    NPC.npc_scarability -= 0.25f;
                    break;
                default:
                    break;
            }
        }

        void HandleGiftInteraction(GameObject gift)
        {
            NPCBehaviour npcBehaviour = GetComponent<NPCBehaviour>();
            npcBehaviour.giftCount += 1;
            UpdateOnGiftCollected(npcBehaviour);
            print("Gift collected! Total gifts: " + npcBehaviour.giftCount);
            Destroy(gift);
        }

    }

}
