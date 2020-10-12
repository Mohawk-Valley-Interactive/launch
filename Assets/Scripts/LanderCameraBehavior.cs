using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanderCameraBehavior : MonoBehaviour
{
    public LanderBehaviour lander;
    public TerrainGeneratorBehaviour terrain;

    // Update is called once per frame
    void Update()
    {
        Vector3 position = this.transform.position;
        position.x = lander.transform.position.x;
        this.transform.position = position;

        terrain.offset = position;
    }
}
