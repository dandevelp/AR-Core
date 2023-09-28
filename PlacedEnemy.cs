using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedEnemy
{
    public GameObject Obj;
    public Vector3 Vector3;
    public Enemy Enemy;

    public PlacedEnemy() { }

    public PlacedEnemy(GameObject obj, Vector3 vector3, Enemy enemy)
    {
        this.Obj = obj;
        this.Vector3 = vector3;
        this.Enemy = enemy;
    }

    public PlacedEnemy(GameObject obj, Vector3 vector3)
    {
        this.Obj = obj;
        this.Vector3 = vector3;
        this.Enemy = new Enemy();
    }
}

