using UnityEngine;
using System.Collections;

public class HealthBar : HealthBarBackground {
    public GameObject text;
	// Use this for initialization
	void Start () {
        text=Instantiate(MenuAndGlobals.menu.textMeshPrefab);
        text.GetComponent<TextMesh>().characterSize/=2f;
        text.GetComponent<TextMesh>().anchor=TextAnchor.MiddleCenter;
        text.GetComponent<TextMesh>().alignment=TextAlignment.Center;
	}
	
	// Update is called once per frame
	void Update () {
		if(text){
			text.transform.position=this.transform.position+new Vector3(0,0,-1f);
			text.GetComponent<TextMesh>().text=""+GetComponentInParent<Monster>().stats[(int)Monster.Stats.health]+"/"
			+GetComponentInParent<Monster>().stats[(int)Monster.Stats.maxHealth];
		}
        if(GetComponentInParent<Monster>().healthPercent()>=0){
            this.transform.localScale=new Vector3(GetComponentInParent<Monster>().healthPercent(),1/4f,1);
        }
        if(GetComponentInParent<Monster>().healthPercent()<=0&&GetComponentInParent<Monster>().isWild){
            Destroy(text);
            Destroy(gameObject);
        }
	}
    void OnDestroy(){
        Destroy(text.gameObject);
    }
}
