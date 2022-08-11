using UnityEngine;
using System.Collections;
public class Thing : MonoBehaviour {
    public bool isTeleport;
    public float x;
    public float y;
	// Use this for initialization
	void Start () {
	
	}

    void OnTriggerEnter2D(Collider2D that){
        if(that.GetComponent<Player>()){
            that.transform.position=new Vector3(x,y,that.transform.position.z);
        }
    }
}
