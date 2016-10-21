using UnityEngine;
using System.Collections;

public class Stats : MonoBehaviour {
    private int health= 100;
    public int Health
    {
        get
        {
            return health;
        }
        set
        {
            if (value <= 100)
            {
                health = value;
            }
        }
    }
    void Update()
    {
        if (health <= 100)
        {
            DestroyObject(gameObject);

        }
    }
}
