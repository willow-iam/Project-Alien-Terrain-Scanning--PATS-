using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuOption : MonoBehaviour {
    public string level;
    public Sprite button;
	// Use this for initialization
	void Start () {
        if(SceneManager.GetActiveScene().name!="Main Menu"){
    	    invisiblify();
        }
	}
	
	// Update is called once per frame
	void Update () {
        if(SceneManager.GetActiveScene().name=="Main Menu"||MenuAndGlobals.isInMenu)
            visiblify();
	}
    public void visiblify(){
        this.GetComponent<SpriteRenderer>().sprite=button;
    }
    public void invisiblify(){
        this.GetComponent<SpriteRenderer>().sprite=null;
    }
}
