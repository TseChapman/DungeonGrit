﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapScript : MonoBehaviour
{
    public Transform player;

    private void LateUpdate()
    {
        Vector3 newPos = player.position;
        newPos.z = -10;
        transform.position = newPos;
    }
}
