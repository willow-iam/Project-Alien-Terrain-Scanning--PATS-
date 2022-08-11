using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;
using System.Collections.Generic; 

public class UpgradeOption : MonoBehaviour {
    public int stat;//0 for speed, 1 for power, 2 for durability, 3 for repairs
    public int index;
    public int amount;
    public int[] requiredRobots;//the Monster IDs of the robots we need to kill
    public float[] requiredParts;//the number of parts from each of the robots we need to kill
    public Sprite button;
    public static bool tutorialGiven;
    GameObject whiteBackground;
    GameObject text;
    List<GameObject> mySprites = new List<GameObject>();
    bool isVisible;
    static Color selectable= new Color(1,1,1);
    static Color disabled = new Color(.5f,.5f,.5f);
    static Color dissSel = new Color(.55f,.5f,.5f);
    static Color selected = new Color(.5f,.6f,.6f);
    public AudioClip success;
    public AudioClip fail;
    public UpgradeOption create(int s, int a, int[] requiredRs, float[] requiredPs, int i){
        stat=s;
        amount=a;
        requiredRobots=requiredRs;
        requiredParts=requiredPs;
        index=i;
        /*if(!GetComponent<Collider2D>())
			gameObject.AddComponent<BoxCollider2D>();*/
        return this;
    }
    void Start(){
        isVisible=false;
        tutorialGiven=false;
    }
    // Update is called once per frame
    void Update () {

        if(!isVisible&&available())
            visiblify();
        else{
            if(text!=null)
                text.GetComponent<TextMesh>().text=getText();
        }
    }
    void setColor(Color c)
    {
        foreach(GameObject i in mySprites){
            if(i.GetComponent<SpriteRenderer>()!=null)
                i.GetComponent<SpriteRenderer>().color=c;
        }
    }
    public void UseCorrectColor(){
        if(hasEnoughParts())setColor(selectable);
        else setColor(disabled);
    }
    public void OnMouseOver(){
        Player.mainPlayer.mouseOnUpgrade=index;
        if(hasEnoughParts())setColor(selected);
        else setColor(dissSel);
    }
    public void OnMouseExit(){
        UseCorrectColor();
    }
    public bool hasEnoughParts(){
        for(int i=0;i<requiredRobots.Length;i++){
            if(Player.mainPlayer.parts[(int)requiredRobots[i]]<(int)requiredParts[i]){
                return false;
            }
        }
        return true;
    }
    public void OnMouseDown(){
        if(Monster.choosing)return;
        GetComponent<AudioSource>().clip=hasEnoughParts()?success:fail;
        GetComponent<AudioSource>().volume=MenuAndGlobals.menu.GetComponentInChildren<AudioSource>().volume;
        this.GetComponent<AudioSource>().Play();
        if(!hasEnoughParts())return;
        StartCoroutine(use());

        /*for(int i=0;i<requiredRobots.Length;i++){
            Player.mainPlayer.parts[(int)requiredRobots[i]]-=(int)requiredParts[i];
			if(stat<=2)	requiredParts[i]*=1.5f;
        }
        foreach(Monster m in Player.mainPlayer.myMonsters){
            switch(stat){
                case 1:
                    m.stats[1]+=amount;
                    Player.mainPlayer.initLazers();
                    continue;
                case 2:
                    m.stats[3]+=amount;
                    continue;
                case 3:
                    m.stats[2]+=(m.stats[(int)Monster.Stats.maxHealth]*amount)/100;
                    continue;
                default:
                    m.stats[stat]+=amount;
                    break;
            }
        }*/
        Player.mainPlayer.refreshUpgradeOptionColors();
    }
    public IEnumerator use(){
        Monster.choosing=true;
        foreach(Monster i in Player.mainPlayer.myMonsters)i.chosen=false;
        Monster m=null;
        while(m==null){
            yield return null;
            foreach(Monster i in Player.mainPlayer.myMonsters){
                if(i.chosen){
                    m=i;
                    break;
                }
            }
        }
        switch(stat){
            case 1:
                m.stats[1]+=amount;
                Player.mainPlayer.initLazers();
                break;
            case 2:
                m.stats[3]+=amount;
                break;
            case 3:
                m.stats[2]+=(m.stats[(int)Monster.Stats.maxHealth]*amount)/100;
                break;
            default:
                m.stats[stat]+=amount;
                break;
        }
        for(int i=0;i<requiredRobots.Length;i++){
            Player.mainPlayer.parts[(int)requiredRobots[i]]-=(int)requiredParts[i];
            if(stat<=2) requiredParts[i]*=1.2f;
        }
        foreach(UpgradeOption i in Player.mainPlayer.UpgradeOptions){
            i.UseCorrectColor();
        }
        Monster.choosing=false;
    }
    public bool available(){
        foreach(int i in requiredRobots){
            if(Player.mainPlayer.killCount[i]==0)return false;
        }
        return true;
    }
    string getText(){
        string toRet="";
        switch(stat){
            case 0:
                toRet+="Speed";
                break;
            case 1:
                toRet+="Power";
                break;
            case 2:
                toRet+="Durability";
                break;
            case 3:
                toRet+="Repairs";
                break;
            default:
                break;
        }
        toRet+="\n+"+amount;
        if(stat==3){
            toRet+="%";
        }
        else if(stat==1){
            toRet+="00";
        }
		else if(stat==0){
            toRet+="0";
        }
        for(int i=0;i<requiredParts.Length;i++){
            toRet+="\n\t  "+MenuAndGlobals.pad(""+Player.mainPlayer.parts[(int)requiredRobots[i]],3)+"/"+(int)requiredParts[i];
        }
        return toRet;
    }
    public void visiblify(){
        whiteBackground=Instantiate(MenuAndGlobals.menu.whitePixelPrefab);
        whiteBackground.transform.localScale=new Vector3(3,1.1f+requiredRobots.Length*.6f,1);
        whiteBackground.transform.position=this.transform.position;
        GetComponent<BoxCollider2D>().size=whiteBackground.transform.localScale;
        GetComponent<BoxCollider2D>().offset=new Vector2(1.5f,-(1.1f+requiredRobots.Length*.6f)/2);
        for(int i=0;i<requiredRobots.Length;i++){
            mySprites.Add((GameObject)Instantiate(new GameObject()));
            
            mySprites[i].AddComponent<SpriteRenderer>();
            mySprites[i].GetComponent<SpriteRenderer>().sprite=MenuAndGlobals.Monsters[(int)requiredRobots[i]].GetComponent<SpriteRenderer>().sprite;
            
            mySprites[i].transform.position=
                this.transform.position+
                new Vector3(.1f,-1.6f-(i*.6f),-.5f)+
                mySprites[i].GetComponent<SpriteRenderer>().sprite.bounds.size*.125f;/**/
            mySprites[i].transform.localScale=new Vector3(1/8f,1/8f,1);
        }
        
        text=Instantiate(MenuAndGlobals.menu.textMeshPrefab);
        text.GetComponent<TextMesh>().text=getText();
        text.transform.position=transform.position+new Vector3(0,0,-1);
        
        mySprites.Add(whiteBackground);
        mySprites.Add(text);
        
        foreach(GameObject sprite in mySprites){
            sprite.transform.parent=this.transform;
        }
        isVisible=true;
        UseCorrectColor();
        /*
		Vector3 temp = Player.mainPlayer.UpgradeOptions[Player.mainPlayer.UnlockedUpgradeOptions].transform.position;
		Player.mainPlayer.UpgradeOptions[Player.mainPlayer.UnlockedUpgradeOptions].transform.position=transform.position;
		transform.position=temp;
		++Player.mainPlayer.UnlockedUpgradeOptions;*/
        if(!tutorialGiven&&stat!=3){
            Player.mainPlayer.AddText(
                "Using data from the life forms you've\n"+
                "scanned, you can upgrade your scanners.\n"+
                "To do so, once you have enough data,\n"+
                "click the upgrades on the right side\n"+
                "of the screen, then click on one of\n"+
                "your scanners.",time:20);
            tutorialGiven=true;
        }
    }
}
