using UnityEngine;
using System.Collections;

public class Speaker : MonoBehaviour {
	// Use this for initialization
    public Sprite[] sprites;
	GameObject text;
    public string words;
	public Font f;
    int spriteDex=0;
    bool isSpeaking=false;
	void Start(){
		words=words.Replace("\\n","\n");
	}
	void Update () {
	    if(isSpeaking){
            if(Random.Range(0,100)<5){
                spriteDex++;
                if(spriteDex>=sprites.Length){
                    spriteDex=0;
                }
                GetComponent<SpriteRenderer>().sprite=sprites[spriteDex];
            }
        }
        else{
            this.GetComponent<SpriteRenderer>().sprite=sprites[0];
        }
	}
    public void OnTriggerEnter2D(Collider2D other){
        isSpeaking=true;
		/*white.transform.position=transform.position+new Vector3(words.Length/-32f,3,0);
		white.transform.localScale=new Vector3(words.Length/16f,1,1);*/
		text=Instantiate(MenuAndGlobals.menu.textMeshPrefab);
		text.transform.position=transform.position+new Vector3(0,3,-1);
		text.GetComponent<TextMesh>().text=words;
		if(f)
			text.GetComponent<TextMesh>().font=f;
		text.GetComponent<TextMesh>().alignment=TextAlignment.Center;
		text.GetComponent<TextMesh>().anchor=TextAnchor.UpperCenter;
    }
    public void OnTriggerExit2D(Collider2D other){
		Destroy(text);
        isSpeaking=false;
    }
    public bool IsTouching(Player that)
    {
        return this.GetComponent<Collider2D>().IsTouching(that.GetComponent<Collider2D>());
    }
}
