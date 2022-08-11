using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfusingFightParty : FightParty {
    float size=4;
    GameObject border;
    void Start(){
        border=Instantiate(MenuAndGlobals.menu.whitePixelPrefab);
        border.transform.position=Player.mainPlayer.transform.position+new Vector3(-size,size,-1);
        border.transform.localScale*=size*2;;
        border.GetComponent<SpriteRenderer>().color=new Color(1,1,1,.3f);
        if(border.GetComponent<Collider2D>()){
            border.GetComponent<Collider2D>().enabled=false;
        }
    }
	public override void keepMonstersWithinFrame(){
		for(int i=0;i<monsters.Length;i++){
            if(monsters[i].transform.position.x-Player.mainPlayer.transform.position.x<-size)
                monsters[i].transform.Translate(size*2-.1f,0,0);
            if(monsters[i].transform.position.x-Player.mainPlayer.transform.position.x>size) 
                monsters[i].transform.Translate(size*-2+.1f,0,0);
            if(monsters[i].transform.position.y-Player.mainPlayer.transform.position.y<-size)
                monsters[i].transform.Translate(size*2-.1f,5.9f,0);
            if(monsters[i].transform.position.y-Player.mainPlayer.transform.position.y>size) 
                monsters[i].transform.Translate(0,size*-2+.1f,0);
        }
	}
    public override void endFight(){
        DestroyImmediate(border);
        base.endFight();
    }
}
