using UnityEngine;
using System.Collections;
public class WaterFightParty : FightParty {
    public float[] yVelocities;
    void Start(){
        yVelocities=new float[monsters.Length];
    }
    public override void getMovementInput(){
        for(int i = 0; i < monsters.Length; i++)
        {
            if(monsters[i].stats[(int)Monster.Stats.health]>0)
            {
                //if(Input.GetKey(controls[i,0]))monsters[i].transform.Translate(0,0+monsters[i].speed/(768f*Mathf.Sqrt(monsters[i].healthPercent())),0);
                //if(Input.GetKey(controls[i,1]))monsters[i].transform.Translate(0,0-monsters[i].speed/(768f*Mathf.Sqrt(monsters[i].healthPercent())),0);
                if(Input.GetKey(controls[i,2]))monsters[i].transform.Translate(0-monsters[i].stats[(int)Monster.Stats.speed]/(768f*Mathf.Sqrt(monsters[i].healthPercent())),0,0);
                if(Input.GetKey(controls[i,3]))monsters[i].transform.Translate(0+monsters[i].stats[(int)Monster.Stats.speed]/(768f*Mathf.Sqrt(monsters[i].healthPercent())),0,0);
                if(Input.GetKeyDown(controls[i,0]))yVelocities[i]+=1/(4*Mathf.Sqrt(monsters[i].healthPercent()));
                yVelocities[i]-=1/96f;
                monsters[i].transform.Translate(0,yVelocities[i],0);
            }
        }
        enemyMove();
        keepMonstersWithinFrame();
    }
    public override void keepMonstersWithinFrame(){
        base.keepMonstersWithinFrame();
        for(int i = 0; i < monsters.Length; i++){
            if(Mathf.Abs(monsters[i].transform.position.y-Player.mainPlayer.transform.position.y)>=5){
                yVelocities[i]=0;
            }
        }
    }
}