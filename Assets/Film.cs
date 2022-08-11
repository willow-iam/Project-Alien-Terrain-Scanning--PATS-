using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Film : MonoBehaviour {
    public static Queue<Color> colors=new Queue<Color>();
    public static Queue<float> times=new Queue<float>();
    static float timeLeft=0;
    static Color toColor;
    static float lerpT;
    static Film film;
    public void Start(){
        if(film!=null)Destroy(gameObject);
        else{
            film=this;
            DontDestroyOnLoad(this);
        }
    }
    public void FixedUpdate(){
        if(timeLeft<=0){
            GetComponent<SpriteRenderer>().color=toColor;
            if(colors.Count>0){
                toColor=colors.Dequeue();
                timeLeft=times.Dequeue();
                //lerpT=.01667f/timeLeft;
            }
            return;
        }
        GetComponent<SpriteRenderer>().color=Color.Lerp(GetComponent<SpriteRenderer>().color,toColor,.01667f/timeLeft);
        timeLeft-=Time.deltaTime;
    }
    public static void AddColor(Color c, float time=1){
        colors.Enqueue(c);
        times.Enqueue(time);
    }
}
