using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using System.Linq;
[System.Serializable]
public class Monster : MonoBehaviour {
	public enum Stats{
		speed,power,health,maxHealth
	};
    public int[] stats=new int[4];
	public int spriteDex;
    public bool isWild;
    public bool isHurting;
    public bool isBoss;
    private bool doubled;
    public static bool choosing;
    public bool chosen;
    private int healthLastFrame;
    // Use this for initialization
    void Start () {
        
        healthLastFrame=stats[(int)Stats.health];
    }

	public void InitHealthBars(){
		GetComponentInChildren<HealthBarBackground>().Init();
		GetComponentInChildren<HealthBar>().Init();
	}
    public Monster create(int s, int p, int h, Sprite i)
    {
		if(stats.Length!=4)stats=new int[4];
        stats[(int)Stats.speed]=s;
        stats[(int)Stats.power]=p;
        stats[(int)Stats.health]=h;
        stats[(int)Stats.maxHealth]=h;
		GetComponent<SpriteRenderer>().sprite=i;
        GetComponent<BoxCollider2D>().size=i.bounds.size;
		InitHealthBars();
		return this;
    }
	public Monster create(int[]s, Sprite i){
		stats=s;
		GetComponent<SpriteRenderer>().sprite=i;
        GetComponent<BoxCollider2D>().size=i.bounds.size;
		InitHealthBars();
		return this;
	}
    public float healthPercent()
    {
        if(stats[(int)Stats.maxHealth]!=0)
            return stats[(int)Stats.health]/(float)stats[(int)Stats.maxHealth];
        else
            return 1/1000f;
    }
    // Update is called once per frame
    void Update () {
        if(stats[(int)Stats.health]<=0)
        {
            if(isWild){
                transform.Translate(new Vector3(0,.3f,0));
                Destroy(gameObject,5);
            }
            else{
                stats[(int)Stats.health]=0;
            }
        }
        else if(healthLastFrame-stats[(int)Stats.health]==0){
            //this.GetComponent<SpriteRenderer>().color=Color.white;
        }
        else{
            float colorVal=Mathf.Pow(.8f,healthLastFrame-stats[(int)Stats.health]);
            this.GetComponent<SpriteRenderer>().color=
            isWild?
                new Color(0,colorVal,0)
                    :
                new Color(
                    colorVal,
                    colorVal,
                    colorVal
                    )
                ;
        }

        if(isBoss&&System.DateTime.Now.Millisecond%300==0){
            if(Random.Range(0,1)<.5){
                if(doubled){
                    stats[(int)Stats.speed]/=2;
                }
                else{
                    stats[(int)Stats.speed]*=2;
                }
                doubled=!doubled;
            }
        }
        healthLastFrame=stats[(int)Stats.health];
    }
    public void OnMouseDown(){
        chosen=true;
    }
    public void OnMouseOver(){
        if(choosing&&!isWild){
            GetComponent<SpriteRenderer>().color=new Color(0.8f,0.8f,0.9f);
        }
    }
    public void OnMouseExit(){
        GetComponent<SpriteRenderer>().color=Color.white;
    }
    public bool IsTouching(Monster that)
    {
        return this.GetComponent<Collider2D>().IsTouching(that.GetComponent<Collider2D>());
    }
}
