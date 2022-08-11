using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFightParty : FightParty {
    public GameObject[] LavaPuddlePosses;
    public GameObject[] LavaPuddles;
    void Start () {
		for(int i=0;i<LavaPuddles.Length;i++){
            LavaPuddles[i]=Instantiate(LavaPuddlePosses[Random.Range(0,LavaPuddlePosses.Length)]);
            LavaPuddles[i].transform.localScale=new Vector3(0.1f,0.1f,1);
            LavaPuddles[i].AddComponent<PolygonCollider2D>();
            do{    
                LavaPuddles[i].transform.position=Player.mainPlayer.transform.position+new Vector3(Random.Range(-4f,4f),Random.Range(-4f,4f),-2);
                LavaPuddles[i].transform.Rotate(new Vector3(0,0,Random.value*360f));
            }while(LavaPuddles[i].GetComponent<Collider2D>().IsTouching(monsters[0].GetComponent<Collider2D>())
                 ||LavaPuddles[i].GetComponent<Collider2D>().IsTouching(monsters[1].GetComponent<Collider2D>()));
        }
	}

    public override void getMovementInput(){
        base.getMovementInput();
        for(int i=0;i<LavaPuddles.Length;i++){
            if(LavaPuddles[i].transform.localScale.x<1f)
                LavaPuddles[i].transform.localScale*=1.001f;
            if(LavaPuddles[i].GetComponent<Collider2D>().IsTouching(enemy.GetComponent<Collider2D>())){
                if(Random.value<1){
                    enemy.stats[(int)Monster.Stats.health]--;
                }
            }
            for(int m=0;m<monsters.Length;m++){
                if(LavaPuddles[i].GetComponent<Collider2D>().IsTouching(monsters[m].GetComponent<Collider2D>())){
                    if(Random.value<.33f)
                        monsters[m].stats[(int)Monster.Stats.health]--;
                }
            }
        }
    }
    public override void endFight(){
        base.endFight();
        for(int i=0;i<LavaPuddles.Length;i++){
            DestroyImmediate(LavaPuddles[i]);
        }
    }
}
