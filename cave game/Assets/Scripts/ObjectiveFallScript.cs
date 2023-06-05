using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveFallScript : MonoBehaviour
{

    public Vector3 DefaultPosToTeleport;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "player")
        {
            other.gameObject.GetComponent<CharacterController>().enabled = false;
            other.gameObject.transform.position = DefaultPosToTeleport;
            other.gameObject.GetComponent<CharacterController>().enabled = true;
        }
    }
}
