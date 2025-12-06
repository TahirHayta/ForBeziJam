using UnityEngine;
using System.Collections;

namespace NPCGift {

[RequireComponent(typeof(Rigidbody2D))]
public class NPCGift : MonoBehaviour
    {
        [Range(0.0f, 1.0f)]
        public float scarabilityModifier = 0.25f;

        // void Awake()
        // {
        //     return;
        // }

        //Assuming tag is gift for Gift objects
        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Gift"))
            {
                HandleGiftInteraction(collision.gameObject, collision.gameObject.GetComponent<Gift>().team); 
            }
        }
        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("NPC"))
            {
                HandleNPCCollision(collision.gameObject);
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


        // UNTESTED CODE BELOW
        void HandleNPCCollision(GameObject otherNPC)
        {
            NPCBehaviour thisNPC = GetComponent<NPCBehaviour>();
            NPCBehaviour thatNPC = otherNPC.GetComponent<NPCBehaviour>();

            if(thisNPC.team != thatNPC.team)
            {
                if(Random.value < thisNPC.npc_scarability * scarabilityModifier)
                {
                    thatNPC.giftCount = Mathf.Max(0, thatNPC.giftCount - 1);
                    UpdateOnGiftCollected(thatNPC);
                    print("NPC scared away a gift! Other NPC's total gifts: " + thatNPC.giftCount);
                }
            }
        }
        // UNTESTED CODE ABOVE

        public void HandleGiftInteraction(GameObject gift, teamEnum teamOfGiver)
        {
            NPCBehaviour npcBehaviour = GetComponent<NPCBehaviour>();
            if(npcBehaviour.team != teamOfGiver)
            {
                if (npcBehaviour.team == teamEnum.Nix)
                {
                    npcBehaviour.SetTeam(teamOfGiver);
                }
                else
                {
                    //TODO different team gift
                }
                
            }
            else
            {
                npcBehaviour.giftCount += 1;
                UpdateOnGiftCollected(npcBehaviour);
                print("Gift collected! Total gifts: " + npcBehaviour.giftCount);
                Destroy(gift);
            }

        }

    }

}
