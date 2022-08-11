using UnityEngine;
using System.Collections;
public class Background : MonoBehaviour {
    public float encounterChance;
    public FightParty fightPartyPrefab;
    //public int[] Monsters;
    public Monster[] Monsters;
    public AudioClip music;
    public float size;//if size is 0, we're not a boss. Otherwise, we are, and the enemy's scale is changed to size.
    public static bool FirstEncounterIsDone=true;
	public Background Create(float e,Monster[] M,FightParty f,Sprite s){
		encounterChance=e;
		Monsters=M;
		fightPartyPrefab=f;
		GetComponent<SpriteRenderer>().sprite=s;
        GetComponent<BoxCollider2D>().size=s.bounds.size;
		return this;
	}
    public void FixedUpdate(){
        foreach(Monster i in Monsters){
            i.transform.position=new Vector3(-1000,0,0);
        }
    }
    public Monster getEnemy(){
        int d = Random.Range(0,Monsters.Length);
        Monster toRet = Instantiate(FirstEncounterIsDone?Monsters[d]:MenuAndGlobals.Monsters[1]);

		if(size!=0){
			for(int i=0;i<toRet.stats.Length;i++){
				toRet.stats[i]*=5;
			}
			toRet.transform.localScale=new Vector3(size,size,1);
		}
        else{
            toRet.transform.localScale=new Vector3(1,1,1);
        }
		for(int i=0;i<toRet.stats.Length;i++){
            toRet.stats[i]=(int)(toRet.stats[i]*Mathf.Sqrt(Player.mainPlayer.level+1)/2);
			if(toRet.stats[i]==0)toRet.stats[i]++;
		}
        toRet.spriteDex=FirstEncounterIsDone?Monsters[d].spriteDex:1;
        if(!FirstEncounterIsDone){
            FirstEncounterIsDone=true;
            toRet.create(1,1,100,toRet.GetComponent<SpriteRenderer>().sprite);
        }
        toRet.isWild=true;
        return toRet;
    }
    public void OnTriggerEnter2D(Collider2D that){
        if(fightPartyPrefab&&that.gameObject.GetComponent("Player")){
            that.gameObject.GetComponent<Player>().fightPartyPrefab=this.fightPartyPrefab;
        }
    }
    public Collectible addCollectible(int special){
        Collectible collectible = Instantiate(MenuAndGlobals.menu.collectiblePrefab,this.transform.position+new Vector3(0,0,-1),this.transform.rotation).GetComponent<Collectible>();
        collectible.behavior=special;
        collectible.transform.parent=transform;
        return collectible;
    }
}
