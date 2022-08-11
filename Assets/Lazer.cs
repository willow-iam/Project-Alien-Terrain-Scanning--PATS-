using UnityEngine;
public class Lazer : MonoBehaviour {
    public AudioClip scanning;
    void Update()
    {
        Monster takingDamageMonster = touchingMonster();
        if(takingDamageMonster!=null&&!MenuAndGlobals.isInMenu){
            takingDamageMonster.stats[(int)Monster.Stats.health]--;

            MenuAndGlobals.playSound(scanning,takingDamageMonster.healthPercent()<.5f?2:1);
        }
         
    }
    public Monster touchingMonster()
    {
        Collider2D[] possBs = Physics2D.OverlapAreaAll(    new Vector2(transform.position.x-.05f,transform.position.y-.05f),
                                                           new Vector2(transform.position.x+.05f,transform.position.y+.05f));
        foreach(Collider2D x in possBs)
        {
            if(x.GetComponent<Monster>()&&x.GetComponent<Monster>().isWild&&x.GetComponent<Monster>().stats[(int)Monster.Stats.health]>0&&x.transform.position.y!=Player.mainPlayer.transform.position.y-4)
            {
                return x.GetComponent<Monster>();
            }
        }
        return null;
    }
    public bool IsTouching(MonoBehaviour that) {
        return this.GetComponent<Collider2D>().IsTouching(that.GetComponent<Collider2D>());
    }
}