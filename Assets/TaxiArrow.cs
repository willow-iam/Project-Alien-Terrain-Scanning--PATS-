using UnityEngine;
using System.Collections;

public class TaxiArrow : MonoBehaviour {
    public static TaxiArrow Arrow;
    public Sprite arrow;
    public bool isActive=false;
    public Vector3 pointingSpot;
    //public char direction = 'n';
    /*
        n=none
        u=up
        d=down
        l=left
        r=right
        v=upright
        t=upleft
        e=downright
        c=downleft
    */
    void Start () {
        Arrow=this;
        arrow=GetComponent<SpriteRenderer>().sprite;
	}
	
	// Update is called once per frame
	void Update () {

        if(!isActive)
            this.GetComponent<SpriteRenderer>().sprite=null;
        else {
            transform.rotation = Quaternion.Euler(0f, 0f, 
            Mathf.Atan2((pointingSpot.y-GetComponentInParent<Player>().transform.position.y),(pointingSpot.x-GetComponentInParent<Player>().transform.position.x))*Mathf.Rad2Deg
            +180
            );
            this.GetComponent<SpriteRenderer>().sprite=arrow;
        }
	}
}
