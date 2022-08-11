using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudFightParty : FightParty {
    public GameObject[] MudPuddlePosses;
    public GameObject[] MudPuddles;
	// Use this for initialization
	void Start () {
		for(int i=0;i<MudPuddles.Length;i++){
            MudPuddles[i]=Instantiate(MudPuddlePosses[Random.Range(0,MudPuddlePosses.Length)]);
            MudPuddles[i].AddComponent<PolygonCollider2D>();
            MudPuddles[i].transform.position=Player.mainPlayer.transform.position+new Vector3(Random.Range(-5,5),Random.Range(-5,5),-.1f);
            MudPuddles[i].transform.Rotate(0,0,Random.Range(0,360));
        }
	}
	
    public override void getMovementInput(){
        for(int i=0;i<monsters.Length;i++){
            Vector3 dir=new Vector3(0,0,0);
            if(Input.GetKey(controls[i,0]))dir+=new Vector3(0,0+monsters[i].stats[(int)Monster.Stats.speed]/(768f*Mathf.Sqrt(monsters[i].healthPercent())),0);
            if(Input.GetKey(controls[i,1]))dir+=new Vector3(0,0-monsters[i].stats[(int)Monster.Stats.speed]/(768f*Mathf.Sqrt(monsters[i].healthPercent())),0);
            if(Input.GetKey(controls[i,2]))dir+=new Vector3(0-monsters[i].stats[(int)Monster.Stats.speed]/(768f*Mathf.Sqrt(monsters[i].healthPercent())),0,0);
            if(Input.GetKey(controls[i,3]))dir+=new Vector3(0+monsters[i].stats[(int)Monster.Stats.speed]/(768f*Mathf.Sqrt(monsters[i].healthPercent())),0,0);
            for(int c=0;c<MudPuddles.Length;c++){
                if(monsters[i].GetComponent<Collider2D>().IsTouching(MudPuddles[c].GetComponent<Collider2D>())){
                    dir*=.2f;
                }
            }
            monsters[i].transform.Translate(dir);
        }
    }
    public override void enemyMove(){
        float speed=1;
        for(int c=0;c<MudPuddles.Length;c++){
            if(enemy.GetComponent<Collider2D>().IsTouching(MudPuddles[c].GetComponent<Collider2D>())){
                speed*=.2f;
            }
        }
        if((Mathf.Abs(enemy.transform.position.x-enemyDestX))<1 
        && (Mathf.Abs(enemy.transform.position.y-enemyDestY)<1)){
            float v=Random.value;
            if(v<.25f){
                enemyDestX=Random.Range(-5,5f)+Player.mainPlayer.transform.position.x;
                enemyDestY=Random.Range(-5,5f)+Player.mainPlayer.transform.position.y;
            }
            else if(v<.5f){
                enemyDestX=monsters[0].transform.position.x;
                enemyDestY=monsters[0].transform.position.y;
            }
            else if(v<.75f){
                enemyDestX=monsters[1].transform.position.x;
                enemyDestY=monsters[1].transform.position.y;
            }
            else{
                enemyDestX=Mathf.Pow(Random.Range(-1.7f,1.7f),3)+Player.mainPlayer.transform.position.x;
                enemyDestY=Mathf.Pow(Random.Range(-1.7f,1.7f),3)+Player.mainPlayer.transform.position.y;
            }
        }
        if(enemy.transform.position.x<enemyDestX)
            enemy.transform.Translate(speed*enemy.stats[(int)Monster.Stats.speed]/768f,0,0);
        else //(enemy.transform.position.x>enemyDestX)
            enemy.transform.Translate(-speed*enemy.stats[(int)Monster.Stats.speed]/768f,0,0);
        if(enemy.transform.position.y<enemyDestY)
            enemy.transform.Translate(0,speed*enemy.stats[(int)Monster.Stats.speed]/768f,0);
        else //(enemy.transform.position.y>enemyDestY)
            enemy.transform.Translate(0,-speed*enemy.stats[(int)Monster.Stats.speed]/768f,0);
    }
    public override void endFight(){
        base.endFight();
        for(int i=0;i<MudPuddles.Length;i++){
            DestroyImmediate(MudPuddles[i]);
        }
    }
}
