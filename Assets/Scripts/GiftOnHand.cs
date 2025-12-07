using UnityEngine;
using NPC;

public class GiftOnHand : MonoBehaviour
{
    [SerializeField] public teamEnum team;
    public NPC.NPCBehaviour holderNPC;
    void Start()
    {
        switch(team)
        {
            case teamEnum.Red:
                GetComponent<SpriteRenderer>().color = Color.red;
                break;
            case teamEnum.Blue:
                GetComponent<SpriteRenderer>().color = Color.blue;
                break;
            case teamEnum.Green:
                GetComponent<SpriteRenderer>().color = Color.green;
                break;
            case teamEnum.Yellow:
                GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
            default:
                GetComponent<SpriteRenderer>().color = Color.white;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
