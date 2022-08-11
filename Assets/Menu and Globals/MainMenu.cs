using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour {
	bool isLoading=false;
    string rngSeedText="";
	void Update () {
		if(!isLoading){
			GetComponentInChildren<TextMesh>().text="";
		}
		else{
			GetComponentInChildren<TextMesh>().text="Generating Level. This may take a minute. Don't panic.";
		}
	}
	void OnGUI(){
        if(!isLoading){
    		if(GUI.Button(new Rect(0,0,Screen.width/2,Screen.height/2),"New Run")){
                try{
                    Random.InitState(int.Parse(rngSeedText));
                }
                catch(System.FormatException){
                    if(rngSeedText!=""){
                        int seed=0;
                        for(int i=0;i<rngSeedText.Length;i++){
                            seed+=(int)rngSeedText[i];
                        }
                        Random.InitState(seed);
                    }
                }
                Invoke("LoadScene",5);
                Film.AddColor(new Color(1,1,1,1),5);
                Film.AddColor(new Color(1,1,1,0),5);
    			isLoading=true;
    		}
            rngSeedText=GUI.TextField(new Rect(0,Screen.height/2,Screen.width/4,Screen.height/16),rngSeedText);
        }
	}
	void LoadScene(){
        SceneManager.LoadScene("RLScene");
    }
}
