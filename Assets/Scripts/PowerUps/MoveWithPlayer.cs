using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithPlayer : MonoBehaviour
{
    public GameObject PlayerRef;
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        if(PlayerRef == null) return;

        transform.position = PlayerRef.transform.position;
    }
}
