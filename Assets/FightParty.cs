using UnityEngine;
using System.Collections;
public class FightParty : MonoBehaviour {
    public Monster[] monsters = new Monster[2];
    public Monster enemy;
    public float enemyDestX;
    public float enemyDestY;
    public string[,] controls = new string[2,4];
    float escapeTime=0;
    public AudioClip doneScanning;
    public static AudioClip end;
    Background bg;
    public void init(Monster[] good, Monster bad, 
            string[,] c=null/*new string[,]{
            {"w","s","a","d","q","e","z","c"},
            {"i","k","j","l","u","o","m","."}
            }*/,
            Background b=null
        )
    {
        if(b==null)bg=Player.mainPlayer.getBackground();
        else bg=b;
        bg.transform.localScale*=50;
        bg.transform.Translate(0,0,-.1f);
        bg.GetComponent<Collider2D>().enabled=false;
        Player.mainPlayer.disable();
        monsters = good;
        enemy = bad;
        if(doneScanning!=null)
            end=doneScanning;
        //put the enemy at a random position at a random edge of the screen
        if(Random.value>.5f)
            bad.transform.position=Player.mainPlayer.transform.position+new Vector3(Random.value>.5f?-5:5,Random.Range(-5f,5f),-.2f);
        else
            bad.transform.position=Player.mainPlayer.transform.position+new Vector3(Random.Range(-5f,5f),Random.value>.5f?-5:5,-.2f);

        controls=
            (c==null)?
            new string[,]{
            {"w","s","a","d","q","e","z","c"},
            {"i","k","j","l","u","o","m","."}
            }
            :
            c
            ;
        //monsters[0].transform.position=new Vector3(Player.mainPlayer.transform.position.x-3,Player.mainPlayer.transform.position.y-3,-.1f);//-1 is a layer above 0. It's counter intuitive, I know.
        //monsters[1].transform.position=new Vector3(Player.mainPlayer.transform.position.x+3,Player.mainPlayer.transform.position.y-3,-.1f);
        foreach(Monster i in good){
            i.GetComponent<Rigidbody2D>().simulated=true;
        }
        bad.GetComponent<Rigidbody2D>().simulated=true;
        enemyDestX=enemy.transform.position.x;
        enemyDestY=enemy.transform.position.y;
        monsters[0].transform.Translate(-.1f,0,0);
        monsters[1].transform.Translate(.1f,0,0);
    }
	// Update is called once per frame
	void Update () {
        bool restarting=true;
        for(int i=0;i<monsters.Length;i++){
            monsters[i].GetComponent<SpriteRenderer>().color=Color.white;
            if(monsters[i].IsTouching(enemy)&&!MenuAndGlobals.isInMenu){
                monsters[i].stats[(int)Monster.Stats.health]-=enemy.stats[(int)Monster.Stats.power];
                if(Random.Range(0,10)<8)
                    monsters[i].GetComponent<SpriteRenderer>().color=Color.red;
            }
            if(monsters[i].stats[(int)Monster.Stats.health]>0)restarting=false;
        }
        if(restarting){
            Player.mainPlayer.restart();
            return;
        }
        if(!MenuAndGlobals.isInMenu){
            getMovementInput();
            keepMonstersWithinFrame();
            if(enemy.stats[(int)Monster.Stats.health]>0){
                enemyMove();
            }
        }

        if(enemy.stats[(int)Monster.Stats.health]<=0)
        {
            //Encounter is over
            endFight();
        }
    }
    public virtual void getMovementInput()
    {
        for(int i = 0; i < monsters.Length; i++)
        {
            if(monsters[i].stats[(int)Monster.Stats.health]>0)
            {
				//float rad2=0.70710678118f;//the square root of a half
                if(Input.GetKey(controls[i,0]))monsters[i].transform.Translate(0,0+monsters[i].stats[(int)Monster.Stats.speed]/(768f*Mathf.Sqrt(monsters[i].healthPercent())),0);
                if(Input.GetKey(controls[i,1]))monsters[i].transform.Translate(0,0-monsters[i].stats[(int)Monster.Stats.speed]/(768f*Mathf.Sqrt(monsters[i].healthPercent())),0);
                if(Input.GetKey(controls[i,2]))monsters[i].transform.Translate(0-monsters[i].stats[(int)Monster.Stats.speed]/(768f*Mathf.Sqrt(monsters[i].healthPercent())),0,0);
                if(Input.GetKey(controls[i,3]))monsters[i].transform.Translate(0+monsters[i].stats[(int)Monster.Stats.speed]/(768f*Mathf.Sqrt(monsters[i].healthPercent())),0,0);
			}
        }

    }
    public virtual void enemyMove(){
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
            enemy.transform.Translate(enemy.stats[(int)Monster.Stats.speed]/768f,0,0);
        else //(enemy.transform.position.x>enemyDestX)
            enemy.transform.Translate(-enemy.stats[(int)Monster.Stats.speed]/768f,0,0);
        if(enemy.transform.position.y<enemyDestY)
            enemy.transform.Translate(0,enemy.stats[(int)Monster.Stats.speed]/768f,0);
        else //(enemy.transform.position.y>enemyDestY)
            enemy.transform.Translate(0,-enemy.stats[(int)Monster.Stats.speed]/768f,0);

    }
    public virtual void keepMonstersWithinFrame(){

        if(
            (monsters[0].transform.position.x-Player.mainPlayer.transform.position.x<-5&&monsters[1].transform.position.x-Player.mainPlayer.transform.position.x<-5)||
            (monsters[0].transform.position.y-Player.mainPlayer.transform.position.y<-5&&monsters[1].transform.position.y-Player.mainPlayer.transform.position.y<-5)||
            (monsters[0].transform.position.x-Player.mainPlayer.transform.position.x>5&&monsters[1].transform.position.x-Player.mainPlayer.transform.position.x>5)||
            (monsters[0].transform.position.y-Player.mainPlayer.transform.position.y>5&&monsters[1].transform.position.y-Player.mainPlayer.transform.position.y>5)
        )escapeTime+=Time.deltaTime;
        else escapeTime=0;
        if(escapeTime>5)endFight(true);
        //.Log(escapeTime);
        for(int i = 0; i < monsters.Length; i++)
        {
            if(monsters[i].transform.position.x-Player.mainPlayer.transform.position.x<-5)
                monsters[i].transform.position=new Vector3(Player.mainPlayer.transform.position.x-5,monsters[i].transform.position.y,monsters[i].transform.position.z);
            if(monsters[i].transform.position.y-Player.mainPlayer.transform.position.y<-5)
                monsters[i].transform.position=new Vector3(monsters[i].transform.position.x,Player.mainPlayer.transform.position.y-5,monsters[i].transform.position.z);
            if(monsters[i].transform.position.x-Player.mainPlayer.transform.position.x>5)
                monsters[i].transform.position=new Vector3(Player.mainPlayer.transform.position.x+5,monsters[i].transform.position.y,monsters[i].transform.position.z);
            if(monsters[i].transform.position.y-Player.mainPlayer.transform.position.y>5)
                monsters[i].transform.position=new Vector3(monsters[i].transform.position.x,Player.mainPlayer.transform.position.y+5,monsters[i].transform.position.z);
        }
    }
    public virtual void endFight(){
        Player.isFighting=false;
        MenuAndGlobals.playSound(end,supercede:true);//not working rn for some reason...
        if(Player.mainPlayer.parts[enemy.spriteDex]==0){
            GameObject doll=(GameObject)Instantiate(new GameObject(),MenuAndGlobals.menu.transform.position
                +new Vector3(-14.5f,8.5f-(enemy.spriteDex*1.7f),1),Quaternion.identity);
            doll.transform.localScale=new Vector3(.5f,.5f,1);
            doll.AddComponent<SpriteRenderer>();
            doll.GetComponent<SpriteRenderer>().sprite=MenuAndGlobals.Monsters[enemy.spriteDex].GetComponent<SpriteRenderer>().sprite;
        }
        Player.mainPlayer.killCount[enemy.spriteDex]++;
        Player.mainPlayer.parts[enemy.spriteDex]++;
        foreach(Monster i in monsters){
            i.stats[(int)Monster.Stats.health]+=i.stats[(int)Monster.Stats.maxHealth]/50;
            if(i.stats[(int)Monster.Stats.health]>i.stats[(int)Monster.Stats.maxHealth])
                i.stats[(int)Monster.Stats.health]=i.stats[(int)Monster.Stats.maxHealth];
            //i.GetComponent<Rigidbody2D>().simulated=false;
        }
        enemy.transform.Translate(0,0,1);
        Player.mainPlayer.enable();
        Player.mainPlayer.refreshUpgradeOptionColors();
        monsters[0].transform.localPosition=new Vector3(-1,0,0);
        monsters[1].transform.localPosition=new Vector3(1,0,0);/**/
        enemy.GetComponent<SpriteRenderer>().color=Color.blue;
        enemy.stats[(int)Monster.Stats.maxHealth]=0;
        while(enemy.GetComponentInChildren<HealthBarBackground>())
            DestroyImmediate(enemy.GetComponentInChildren<HealthBarBackground>().gameObject);
        if(enemy.transform.localScale.x!=1){
            Player.mainPlayer.goingToNextLevel=true;
        }
        bg.transform.localScale/=50;
        bg.transform.Translate(0,0,.1f);
        bg.GetComponent<Collider2D>().enabled=true;
        DestroyImmediate(gameObject);//after the fight is over, we don't need the FightParty anymore.
    }
    public void endFight(bool escaped){
        Player.isFighting=false;
        Vector3 backup=Player.mainPlayer.transform.position;
        Player.mainPlayer.transform.position=Player.mainPlayer.lazers[Player.mainPlayer.lazers.Length/2].transform.position;
        if(Player.mainPlayer.getBackground()==null){
            Player.mainPlayer.transform.position=backup;
        }
        monsters[0].transform.localPosition=new Vector3(-1,0,0);
        monsters[1].transform.localPosition=new Vector3(1,0,0);/**/
        enemy.transform.Translate(0,0,1);
        Player.mainPlayer.enable();
        Player.mainPlayer.refreshUpgradeOptionColors();
        while(enemy.GetComponentInChildren<HealthBarBackground>())
            DestroyImmediate(enemy.GetComponentInChildren<HealthBarBackground>().gameObject);
        bg.transform.localScale/=50;
        bg.GetComponent<Collider2D>().enabled=true;
        bg.transform.Translate(0,0,.1f);
        DestroyImmediate(gameObject);
    }
}
