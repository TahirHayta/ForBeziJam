using UnityEngine;
using System.Collections;
using NPCWalk;
using NPCGift;

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

    public enum npc_team
    {
        Red,
        Blue,
        Green,
        Yellow
    }

    public float npc_scarability
    { get; set; } = 1.0f;

    public int giftCount
    {
        get; set;
    } = 0;

    void Awake()
    {
        npcWalk = GetComponent<NPCWalk.NPCWalk>();
        npcGift = GetComponent<NPCGift.NPCGift>();
    }
}
