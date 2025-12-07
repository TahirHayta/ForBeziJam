using UnityEngine;
using System.Collections;
using NPCWalk;
using NPCGift;
using System.Collections.Generic;

namespace NPC
{
[RequireComponent(typeof(NPCWalk.NPCWalk))]
[RequireComponent(typeof(NPCGift.NPCGift))]
    
public class NPCBehaviour : MonoBehaviour
{
    [SerializeField] NPCWalk.NPCWalk npcWalk;
    [SerializeField] NPCGift.NPCGift npcGift;

    public float npc_speed
    {
        get => npcWalk.npc_speed;
        set => npcWalk.npc_speed = value;
    }

    public teamEnum team = teamEnum.Nix;
    public void SetTeam(teamEnum newTeam) => team = newTeam;

    public float npc_scarability { get; set; } = 1.0f;

    // Track gifts per team
    private Dictionary<teamEnum, int> giftsByTeam = new Dictionary<teamEnum, int>
    {
        { teamEnum.Red, 0 },
        { teamEnum.Blue, 0 },
        { teamEnum.Green, 0 },
        { teamEnum.Yellow, 0 }
    };

    public int GetGiftCount(teamEnum t) => giftsByTeam[t];
    public int GetTotalGifts() => giftsByTeam[teamEnum.Red] + giftsByTeam[teamEnum.Blue] + giftsByTeam[teamEnum.Green] + giftsByTeam[teamEnum.Yellow];

    public void AddGift(teamEnum giver)
    {
        if (giftsByTeam.ContainsKey(giver))
            giftsByTeam[giver]++;
    }

    public void RemoveGift(teamEnum team_to_remove)
    {
        if (giftsByTeam.ContainsKey(team_to_remove) && giftsByTeam[team_to_remove] > 0)
            giftsByTeam[team_to_remove]--;
    }

    void Awake()
    {
        npcWalk = GetComponent<NPCWalk.NPCWalk>();
        npcGift = GetComponent<NPCGift.NPCGift>();
    }
}
}
