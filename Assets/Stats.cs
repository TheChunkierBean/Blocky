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
        if (health <= 0 || Input.GetKeyDown(KeyCode.Z))
        {
            System.Random r = new System.Random();
            gameObject.transform.position = new Vector3(3 - (float)r.NextDouble() * 6, 3 - (float)r.NextDouble() * 6, .5f);
            health = 100;
        }
    }
}
