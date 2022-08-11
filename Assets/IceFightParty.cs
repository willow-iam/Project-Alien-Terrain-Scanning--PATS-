using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceFightParty : FightParty {
    public float[] yVelocities;
    public float[] xVelocities;
	// Use this for initialization
	void Start () {
		yVelocities=new float[monsters.Length];
        xVelocities=new float[monsters.Length];
	}
	public override void getMovementInput(){
        for(int i = 0; i < monsters.Length; i++)
        {
            if(monsters[i].stats[(int)Monster.Stats.health]>0)
            {
                //if(Input.GetKey(controls[i,0]))monsters[i].transform.Translate(0,0+monsters[i].speed/(768f*Mathf.Sqrt(monsters[i].healthPercent())),0);
                //if(Input.GetKey(controls[i,1]))monsters[i].transform.Translate(0,0-monsters[i].speed/(768f*Mathf.Sqrt(monsters[i].healthPercent())),0);
                if(Input.GetKey(controls[i,0]))yVelocities[i]+=1/(128*Mathf.Sqrt(monsters[i].healthPercent()));
                if(Input.GetKey(controls[i,1]))yVelocities[i]-=1/(128*Mathf.Sqrt(monsters[i].healthPercent()));
                if(Input.GetKey(controls[i,2]))xVelocities[i]-=1/(128*Mathf.Sqrt(monsters[i].healthPercent()));
                if(Input.GetKey(controls[i,3]))xVelocities[i]+=1/(128*Mathf.Sqrt(monsters[i].healthPercent()));

                yVelocities[i]+=(yVelocities[i]<0) ? 1/256f:
					(yVelocities[i]==0)?Random.Range(-1/256f,1/256f):-1/256f;
                xVelocities[i]+=(xVelocities[i]<0) ? 1/256f:
					(xVelocities[i]==0)?Random.Range(-1/256f,1/256f):-1/256f;
                monsters[i].transform.Translate(xVelocities[i],yVelocities[i],0);
            }
        }
        enemyMove();
        keepMonstersWithinFrame();
    }
    public override void keepMonstersWithinFrame(){     
        for(int i = 0; i < monsters.Length; i++)
        {
            if(monsters[i].transform.position.x-Player.mainPlayer.transform.position.x<-5)
               xVelocities[i]=.01f; //monsters[i].transform.position=new Vector3(Player.mainPlayer.transform.position.x-5,monsters[i].transform.position.y,monsters[i].transform.position.z);
            if(monsters[i].transform.position.x-Player.mainPlayer.transform.position.x>5)
                xVelocities[i]=-.01f;
            if(monsters[i].transform.position.y-Player.mainPlayer.transform.position.y<-5)
                yVelocities[i]=.01f;            
            if(monsters[i].transform.position.y-Player.mainPlayer.transform.position.y>5)
                yVelocities[i]=-.01f;

        }
        base.keepMonstersWithinFrame();
    }
}
