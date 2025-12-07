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

        // DELETE the old UpdateOnGiftChanged method here (lines 34-67)

        // UNTESTED CODE BELOW
        void HandleNPCCollision(GameObject otherNPC)
        {
            NPCBehaviour thisNPC = GetComponent<NPCBehaviour>();
            NPCBehaviour thatNPC = otherNPC.GetComponent<NPCBehaviour>();

            if(thisNPC.team != thatNPC.team)
            {
                if(Random.value < thisNPC.npc_scarability * scarabilityModifier)
                {
                    thatNPC.RemoveGift(thatNPC.team);
                    UpdateOnGiftChanged(thatNPC, thatNPC.team);
                    print("NPC scared away a gift! Other NPC's total gifts: " + thatNPC.GetTotalGifts());
                }
            }
        }
        // UNTESTED CODE ABOVE

        public void HandleGiftInteraction(GameObject gift, teamEnum teamOfGiver)
        {
            gift.GetComponent<Gift>().canNPCTakeThisGift = false;
            Destroy(gift);
            NPCBehaviour npcBehaviour = GetComponent<NPCBehaviour>();

            // If NPC has gifts from OTHER teams, remove one
            if (npcBehaviour.team != teamOfGiver && npcBehaviour.team != teamEnum.Nix)
            {
                npcBehaviour.RemoveGift(npcBehaviour.team); // remove old team's gift
            }

            npcBehaviour.AddGift(teamOfGiver); // add new gift from giver
            
            // Update team if this is the first gift
            if (npcBehaviour.GetTotalGifts() == 1)
            {
                npcBehaviour.SetTeam(teamOfGiver);
            }

            UpdateOnGiftChanged(npcBehaviour, teamOfGiver);
        }

        private void UpdateOnGiftChanged(NPCBehaviour npc, teamEnum teamOfGiver)
        {
            int total = npc.GetTotalGifts();
            switch (total)
            {
                case 0:
                    npc.SetTeam(teamEnum.Nix);
                    npc.npc_speed = 1.0f;
                    npc.npc_scarability = 0.5f;
                    break;
                case 1:
                    npc.SetTeam(teamOfGiver);
                    npc.npc_speed = 2.0f;
                    npc.npc_scarability = 0.5f;
                    break;
                case 2:
                    npc.npc_speed = 3.0f;
                    npc.npc_scarability = 0.75f;
                    break;
                case 3:
                    npc.npc_speed = 4.0f;
                    npc.npc_scarability = 1.0f;
                    break;
                default:
                    break;
            }
            print($"NPC gifts: {npc.GetTotalGifts()}, Team: {npc.team}");
        }

    }

}
