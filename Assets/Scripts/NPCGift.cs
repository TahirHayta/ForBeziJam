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
                if (collision.gameObject.GetComponent<Gift>().canNPCTakeThisGift) HandleGiftInteraction(collision.gameObject, collision.gameObject.GetComponent<Gift>().team); 
            }
        }
        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("NPC"))
            {
                HandleNPCCollision(collision.gameObject);
            }
        }



        private void UpdateOnGiftChanged(NPCBehaviour NPC, teamEnum teamOfGiver)
        {
            switch(NPC.giftCount)
            {
                case 0:
                    NPC.SetTeam(teamEnum.Nix);
                    print("team nix!");
                    NPC.npc_speed = 1.0f;
                    NPC.npc_scarability -= 0.25f;
                    break;
                case 1:
                    NPC.SetTeam(teamOfGiver);
                    print("team " + teamOfGiver + "!");
                    NPC.npc_speed = 2.0f;
                    NPC.npc_scarability = 0.5f;
                    break;
                case 2:
                    NPC.npc_speed = 3.0f;
                    NPC.npc_scarability = 0.75f;
                    break;
                case 3:
                    NPC.npc_speed = 4.0f;
                    NPC.npc_scarability = 1.0f;
                    break;
                case 4:
                    NPC.giftCount=3; // 3ten fazla olmasın
                    break;
                default:
                    break;
            }
            print("Total gifts: " + NPC.giftCount);
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
                    UpdateOnGiftChanged(thatNPC, thatNPC.team);
                    print("NPC scared away a gift! Other NPC's total gifts: " + thatNPC.giftCount);
                }
            }
        }
        // UNTESTED CODE ABOVE

        public void HandleGiftInteraction(GameObject gift, teamEnum teamOfGiver)
        {
            if (gift.GetComponent<Gift>()) {
                gift.GetComponent<Gift>().canNPCTakeThisGift = false;}
            Destroy(gift);
            NPCBehaviour npcBehaviour = GetComponent<NPCBehaviour>();
            if(npcBehaviour.team != teamOfGiver && npcBehaviour.team != teamEnum.Nix)
            {
                npcBehaviour.giftCount-=1;
            }
            else
            {
                npcBehaviour.giftCount += 1;
            }

            UpdateOnGiftChanged(npcBehaviour,teamOfGiver);

        }

    }

}
