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
    void Awake()
    {
        npcWalk = GetComponent<NPCWalk.NPCWalk>();
        npcGift = GetComponent<NPCGift.NPCGift>();
    }
}
