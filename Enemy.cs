using UnityEngine;

public enum EnemyNames
{
    Bat,
    Snake,
    Oni,
    Ghost
}

public class Enemy : MonoBehaviour
{
    public float HP, AP;
    public string Name;
    public string Img;

    public Enemy(float hp, float ap, EnemyNames name)
    {
        HP = hp;
        AP = ap;
        Name = Img = name.ToString();
    }

    public Enemy() { }

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }
}
