using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarBackground : MonoBehaviour {

	// Use this for initialization
	public void Init () {
		transform.Translate(0,(GetComponentInParent<BoxCollider2D>().size.y)/2+.125f,0);
	}

	/*f(height,scale)
		f(1,1)=.625
		f(3,1)=
	
	*/
}
