using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {
    public int behavior;
    void OnTriggerEnter2D(Collider2D that){
        if(that.GetComponent<Player>()){
            that.GetComponent<Player>().collecteds[behavior]++;
            Destroy(gameObject);
            if(behavior==0){
                that.GetComponent<Player>().AddText(
                    "New Tech Found:Data Collection\n"+
                    "Press Space to instantly find\n"+
                    "a life form, up to "+5*that.GetComponent<Player>().collecteds[behavior]+" times per area.");
            }
            else if(behavior==1){
                that.GetComponent<Player>().AddText(
                    "New Tech Found:Exploration\n"+
                    "In traversal mode, you now move faster.");
            }
            else if(behavior==2){
                if(that.GetComponent<Player>().collecteds[behavior]==1)
                    that.GetComponent<Player>().AddText(
                    "New Tech Found:Navigation\n"+
                    "The arrow points to the end\n"+
                    "of the area.");
                else if(that.GetComponent<Player>().collecteds[behavior]==2)
                    that.GetComponent<Player>().AddText(
                    "New Tech Found:Navigation\n"+
                    "The life forms that inhabit\n"+
                    "this tile are visible in the\n"+
                    "lower left corner of the screen.");
                else{
                    that.GetComponent<Player>().AddText(
                    "New Tech Found:Navigation\n"+
                    "A map of the current area\n"+
                    "is shown when you press Escape.\n");
                }
            }
        }
    }
}
