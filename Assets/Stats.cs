using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Stats : MonoBehaviour {
    public Text hpText;
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
    void Start()
    {
        hpText = GameObject.Find("HP Text").GetComponent<Text>();
    }
    void Update()
    {

        hpText.text = health+"";
        if (health <= 0 || Input.GetKeyDown(KeyCode.Z))
        {
            System.Random r = new System.Random();
            gameObject.transform.position = new Vector3(3 - (float)r.NextDouble() * 6, 3 - (float)r.NextDouble() * 6, .5f);
            health = 100;
        }
    }
}
