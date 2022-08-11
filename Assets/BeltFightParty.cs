using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltFightParty : FightParty {
    Vector3 start;
    float length;
    float angle;
    public GameObject beltPrefab;
    GameObject belt;
    void Start(){
        start=new Vector3(Random.Range(-5f,5f),Random.Range(-5f,5f),.1f);
        length=Random.Range(1,7f);
        angle=Random.Range(0,360f);
        belt=Instantiate(beltPrefab);
        belt.transform.position=Player.mainPlayer.transform.position+start;
        belt.transform.localScale=new Vector3(length,1,1);
        belt.transform.Rotate(new Vector3(0,0,angle));
    }
	public override void getMovementInput() {
        base.getMovementInput();
		foreach(Monster i in monsters){
            if(i.GetComponent<Collider2D>().IsTouching(belt.GetComponent<Collider2D>())){
                i.transform.Translate(new Vector3(Mathf.Cos(Mathf.Deg2Rad*angle),Mathf.Sin(Mathf.Deg2Rad*angle),0)/(i.healthPercent()*96f));
            }
        }
	}
    public override void endFight(){
        base.endFight();
        Destroy(belt);
    }
}
