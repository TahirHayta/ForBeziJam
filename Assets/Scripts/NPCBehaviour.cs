using UnityEngine;
using System.Collections;
using NPCWalk;

[RequireComponent(typeof(NPCWalk.NPCWalk))]
public class NPCBehaviour : MonoBehaviour
{
    [SerializeField] NPCWalk.NPCWalk npcWalk;
    void Awake()
    {
        npcWalk = GetComponent<NPCWalk.NPCWalk>();
    }
}
