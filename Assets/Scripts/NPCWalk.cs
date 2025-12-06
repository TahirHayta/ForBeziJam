using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class NPCWalk : MonoBehaviour
{
    public float npc_speed = 2.0f;
    public float dir_change_interv = 1.0f;
    

    private CharacterController controller;
    private Vector3 move_direction = Vector3.zero;
    float heading = 0.0f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        StartCoroutine(ChangeDirection());
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        controller.SimpleMove(forward * npc_speed);
    }

    IEnumerator ChangeDirection()
    {
        while (true)
        {
            heading = Random.Range(0, 360);
            transform.eulerAngles = new Vector3(0, heading, 0);
            yield return new WaitForSeconds(dir_change_interv);
        }
    }
}
