using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockMouseMove : MonoBehaviour
{

    private void Update()
    {
        this.transform.position = Input.mousePosition;
    }

}
