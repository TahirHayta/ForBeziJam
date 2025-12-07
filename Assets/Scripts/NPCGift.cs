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

            if(thisNPC.team != thatNPC.team && thatNPC.GetTotalGifts() > 0)
            {
                if(Random.value < thisNPC.npc_scarability * scarabilityModifier)
                {
                    // Find the dominant team of the other NPC and remove one gift from it
                    teamEnum dominantTeam = FindDominantTeam(thatNPC);
                    if (dominantTeam != teamEnum.Nix)
                    {
                        thatNPC.RemoveGift(dominantTeam);
                        RecalculateTeamAndStats(thatNPC);
                        print($"NPC scared away a {dominantTeam} gift! Other NPC's total gifts: " + thatNPC.GetTotalGifts());
                    }
                }
            }
        }

        private teamEnum FindDominantTeam(NPCBehaviour npc)
        {
            teamEnum dominantTeam = teamEnum.Nix;
            int maxGifts = 0;
            foreach (var t in new[] { teamEnum.Red, teamEnum.Blue, teamEnum.Green, teamEnum.Yellow })
            {
                int count = npc.GetGiftCount(t);
                if (count > maxGifts)
                {
                    maxGifts = count;
                    dominantTeam = t;
                }
            }
            return dominantTeam;
        }

        // UNTESTED CODE ABOVE

public void HandleGiftInteraction(GameObject gift, teamEnum teamOfGiver)
{
    gift.GetComponent<Gift>().canNPCTakeThisGift = false;

    NPCBehaviour npc = GetComponent<NPCBehaviour>();

    // 1. If NPC currently has a dominant team:
    teamEnum currentTeam = npc.team;

    if (currentTeam != teamEnum.Nix && currentTeam != teamOfGiver)
    {
        // Opponent team gives a gift → erode existing team one by one
        int count = npc.GetGiftCount(currentTeam);

        npc.RemoveGift(currentTeam); // subtract one from existing team

        if (count - 1 <= 0)
        {
            npc.SetTeam(teamEnum.Nix); // eroded to zero → neutral
        }
    }
    else
    {
        // 2. Either neutral OR same team gives gift → add normally
        if (npc.GetTotalGifts() < 3)
                {
                    npc.AddGift(teamOfGiver);
                    Destroy(gift);
                }
    }

    // After any change, recalc team and stats
    RecalculateTeamAndStats(npc);
}
private void RecalculateTeamAndStats(NPCBehaviour npc)
{
    int total = npc.GetTotalGifts();

    teamEnum dominant = teamEnum.Nix;
    int maxCount = 0;

    // Iterate through ALL teams dynamically
    foreach (teamEnum t in System.Enum.GetValues(typeof(teamEnum)))
    {
        if (t == teamEnum.Nix) continue; // skip neutral

        int c = npc.GetGiftCount(t);
        if (c > maxCount)
        {
            maxCount = c;
            dominant = t;
        }
    }

    // A team is dominant ONLY if it owns all gifts
    if (maxCount == total && total > 0)
        npc.SetTeam(dominant);
    else
        npc.SetTeam(teamEnum.Nix);

    // Update stats based only on total gift count
    switch (total)
    {
        case 0:
            npc.npc_speed = 1.0f;
            npc.npc_scarability = 0.5f;
            break;
        case 1:
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
    }

    print($"NPC gifts: {npc.GetTotalGifts()}, Team: {npc.team}");
}


    }

}
