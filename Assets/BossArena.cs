using UnityEngine;
using System.Collections;
using System.IO;
[System.Serializable]
public class BossArena : MonoBehaviour {
    public static Vector2 center;
    static bool tutorialGiven=false;
	// Update is called once per frame
    void Start(){
        center = new Vector2(transform.position.x,transform.position.y);
    }
	void Update () {
        Transform p=Player.mainPlayer.transform;
        if(Mathf.Abs(center.x-p.position.x)<10&&Mathf.Abs(center.y-p.position.y)<10&&!tutorialGiven){
            tutorialGiven=true;
            Player.mainPlayer.AddText(
            "It seems the leader of this area\n"+
            "inhabits that darker tile. I\n"+
            "highly suggest having at least two\n"+
            "upgrades before trying to scan it.\n",
            time:20f);
        }
        if(Mathf.Abs(center.x-p.position.x)<5&&Mathf.Abs(center.y-p.position.y)<5){
            Player.mainPlayer.disable();
            //Player.mainPlayer.stepsSinceLastEncounter=900;
            if(p.position.x<center.x)
                p.Translate(1/24f,0,0);
            else
                p.Translate(-1/24f,0,0);
            if(p.position.y<center.y)
                p.Translate(0,1/24f,0);
            else
                p.Translate(0,-1/24f,0);
        }
        if(1/24f>Mathf.Abs(center.x-p.position.x)&&1/24f>Mathf.Abs(center.y-p.position.y)){
            Player.mainPlayer.enable();
            Player.mainPlayer.generateRandomEncounter(GetComponent<Background>());
            Destroy(this);
        }
	}
}
