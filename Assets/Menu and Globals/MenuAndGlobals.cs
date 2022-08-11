using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
public class MenuAndGlobals : MonoBehaviour {
    public static bool isInMenu;
    public static MenuAndGlobals menu;
    public static int typesOfMonsters=11;
	public static List<Monster> Monsters;
	public Monster[] MonstersEditable;
    public GameObject whitePixelPrefab;
    public GameObject textMeshPrefab;
    public GameObject collectiblePrefab;
	public static Vector3 proportions=new Vector3(30,3,300);
    public AudioSource audioSource;
    Vector3 normalSpot;
    void Start(){
        normalSpot=transform.position;
		menu=this;
        Monsters=new List<Monster>();
		Monsters.AddRange(MonstersEditable);
    }
	void Update () {
        if(Input.GetKeyDown(KeyCode.Escape)){
            isInMenu=!isInMenu;
            if(isInMenu){
                Player.mainPlayer.disable();
                openMenu();
            }
            else{
                if(!Player.isFighting)
                    Player.mainPlayer.enable();
                closeMenu();
            }
        }
        GetComponentInChildren<TextMesh>().text=getText();
	}
	public static int MiddleOfThree(float a, float b, float c){
		return a>b?
            (c>a?
                1:
                c>b?
                    3:
                    1):
            (c>b?
                2:
                c>a?
                    2:
                    1);
	}
    public static void playSound(AudioClip a,float pitch=1,bool supercede=false){
        if(menu.audioSource.isPlaying&&!supercede)return;
        menu.audioSource.volume=menu.GetComponentInChildren<Camera>().GetComponent<AudioSource>().volume;
        menu.audioSource.pitch=pitch;
        menu.audioSource.clip=a;
        menu.audioSource.Play();
    }
    public static string pad(string x, int i){
        string toRet=x;
        while(toRet.Length<i)toRet+=' ';
        return toRet;
    }
    string getText(){
        string toRet="\t\tData:\n";
        for(int i=0;i<Player.mainPlayer.killCount.Length;i++){
            toRet+=(Player.mainPlayer.killCount[i]==0)? "\n  ? ":"\n    ";
            toRet+="    "+pad(""+Player.mainPlayer.parts[i],5)+"/"+Player.mainPlayer.killCount[i]+"\n\n";
        }
        return toRet;
        
    }
    public void openMenu(){
        GetComponentInChildren<UpgradeCamera>().gameObject.transform.position=normalSpot+new Vector3(-15,0,0);
        if(Player.mainPlayer.collecteds[2]>=3)Camera.main.orthographicSize=50*Player.mainPlayer.collecteds[2];
    }
    void closeMenu(){
        GetComponentInChildren<UpgradeCamera>().gameObject.transform.position=normalSpot;//Translate(15,0,0);
        if(Player.mainPlayer.collecteds[2]>=3)Camera.main.orthographicSize=5;
    }

}
