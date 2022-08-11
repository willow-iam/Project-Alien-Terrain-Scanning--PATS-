using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface EnemySpriteMaker{
    Texture2D spriteTemplate(Texture2D fromRect);
}
public static class EnemyTypes{
    public static EnemySpriteMaker[] enemyTypes=new EnemySpriteMaker[]{
        new Snake(),    //0
        new Bug(),      //1
        new Ellipse(),  //2
        new Rectangle() //3
    };
    public static Texture2D Transpose(Texture2D fromRect){
        Texture2D toRet=new Texture2D(fromRect.height,fromRect.width);
        for(int x=0;x<fromRect.width;x++){
            for(int y=0;y<fromRect.height;y++){
                toRet.SetPixel(y,x,fromRect.GetPixel(x,y));
            }
        }
        toRet.filterMode=FilterMode.Point;
        toRet.Apply();
        return toRet;
    }

    //changes each pixel on the edge of the sprite into the specified color.
    public static Texture2D Border(Texture2D fromRect,float[] color){
        for(int x=0;x<fromRect.width;x++){
            fromRect.SetPixel(x,0,new Color(color[0],color[1],color[2]));
            fromRect.SetPixel(x,fromRect.height-1,new Color(color[0],color[1],color[2]));
        }
        for(int y=0;y<fromRect.height;y++){
            fromRect.SetPixel(0,y,new Color(color[0],color[1],color[2]));
            fromRect.SetPixel(fromRect.width-1,y,new Color(color[0],color[1],color[2]));
        }
        fromRect.Apply();
        return fromRect;
    }
}
public class Bug : EnemySpriteMaker{
    public Texture2D spriteTemplate(Texture2D fromRect){
        fromRect.Apply();
        if(fromRect.height<fromRect.width)
            return EnemyTypes.Transpose(spriteTemplate(EnemyTypes.Transpose(fromRect)));
        int sizeX=fromRect.width;
        int sizeY=fromRect.height;
        Texture2D toRet=new Texture2D(sizeX,sizeY);
        int segments=sizeY>=20?2:Random.Range(3,sizeY/5);
        float[] centers=new float[segments];
        float[] heights=new float[segments];
        float[] widths=new float[segments];
        for(int i=0;i<segments;i++){
            centers[i]=Random.Range(0f,sizeY-2);
        }
        System.Array.Sort(centers);
        heights[0]=Mathf.Min(centers[0],centers[1]-centers[0]/2);
        widths[0]=2+Random.Range(0f,sizeX/2);
        for(int i=1;i<segments-1;i++){
            heights[i]=Random.Range(1f,Mathf.Min(centers[i]-centers[i-1],centers[i+1]-centers[i])/2);
            widths[i]=2+Random.Range(0f,sizeX/2);
        }
        heights[segments-1]=Random.Range(1f,Mathf.Min(centers[segments-1]-centers[segments-2],sizeY-centers[segments-1]));
        widths[segments-1]=2+Random.Range(0f,sizeX/2);
        int s=0;
        for(int y=0;y<sizeY;y++){
            if(y>centers[s]+heights[s]&&s<segments-1)s++;
            for(int x=0;x<sizeX;x++){
                float dist=(x-sizeX/2)*(x-sizeX/2)/(widths[s]*widths[s])+(y-centers[s])*(y-centers[s])/(heights[s]*heights[s]);
                //Debug.Log("x:"+x+"\ty:"+y+"\tsizeX:"+sizeX+"\tcenter:"+centers[s]+"\tWidth:"+widths[s]+"\tHeight:"+heights[s]+"\tdist:"+dist);
                toRet.SetPixel(x,y,new Color((1-dist)*fromRect.GetPixel(x,y).r,(1-dist)*fromRect.GetPixel(x,y).g,(1-dist)*fromRect.GetPixel(x,y).b,(1-dist)*2));
            }
        }/**/

        /*
        int sizeX=fromRect.width;
        int sizeY=fromRect.height;
        Texture2D toRet=new Texture2D(sizeX,sizeY);
        int segments=sizeY/sizeX+1;
        int[] sizes=new int[segments];
        int[] legThicks=new int[segments];
        float[] legSlopes=new float[segments];
        //the radius of each body segment in pixels
        for(int i=0;i<segments;i++){
            sizes[i]=Random.Range(sizeX/4,sizeX/2);
            legThicks[i]=Random.Range(0,sizeX/8);
            if(legThicks[i]>0){
                legSlopes[i]=Random.Range(-1f,1f)*Random.Range(-1f,1f);
            }
        }
        int currY=0;
        float r=0f;//r squared
        float scalar=0f;//for shading
        for(int i=0;i<segments;i++){
            currY+=sizes[i];
            for(int x=-sizeX/2;x<=sizeX/2;x++){
                for(int y=-sizes[i];y<=sizes[i];y++){
                    r=x*x+y*y;
                    //if(y-currY<=legSlopes[i]*(x-sizeX/2)){

                    if(r>sizes[i]*sizes[i]&&Mathf.Abs(y-legSlopes[i]*Mathf.Abs(x))>legThicks[i]){
                        toRet.SetPixel(sizeX/2+x,currY+y,new Color(0,0,0,0));
                    }
                    else if(r<=sizes[i]*sizes[i]){
                        scalar=(1-r/(sizes[i]*sizes[i]*sizes[i]));
                        toRet.SetPixel(sizeX/2+x,
                            currY+y,
                            new Color(
                                fromRect.GetPixel(sizeX/2+x,currY+y).r*scalar,
                                fromRect.GetPixel(sizeX/2+x,currY+y).g*scalar,
                                fromRect.GetPixel(sizeX/2+x,currY+y).b*scalar
                            )
                        );
                    }                    
                    else {
                        toRet.SetPixel(sizeX/2+x,currY+y,
                            new Color(
                                fromRect.GetPixel(sizeX/2,currY-1).r/2,
                                fromRect.GetPixel(sizeX/2,currY-1).g/2,
                                fromRect.GetPixel(sizeX/2,currY-1).b/2
                            )
                        );
                    }
                }
            }
            currY+=sizes[i];
        }
        for(int y=currY+1;y<sizeY;y++){
            for(int x=0;x<sizeX;x++){
                toRet.SetPixel(x,y,new Color(0,0,0,0));
            }
        }/**/
        toRet.filterMode=FilterMode.Point;
        toRet.Apply();
        return toRet;
    }
}
public class Snake: EnemySpriteMaker{
    public Texture2D spriteTemplate(Texture2D fromRect){
        if(fromRect.width<fromRect.height)
            return EnemyTypes.Transpose(spriteTemplate(EnemyTypes.Transpose(fromRect)));
        Texture2D toRet=new Texture2D(fromRect.width,fromRect.height);
        int width=Random.Range(2,fromRect.height/4);
        int currY=fromRect.height/2;
        for(int x=0;x<fromRect.width;x++){
            for(int y=0;y<fromRect.height;y++){
                if(Mathf.Abs(y-currY)<width){
                    toRet.SetPixel(x,y,
                        new Color(
                            fromRect.GetPixel(x,y).r*(1-Mathf.Abs(y-currY)/(float)width),
                            fromRect.GetPixel(x,y).g*(1-Mathf.Abs(y-currY)/(float)width),
                            fromRect.GetPixel(x,y).b*(1-Mathf.Abs(y-currY)/(float)width)
                        )
                   );
                }
                else toRet.SetPixel(x,y,new Color(0,0,0,0));
            }
            currY+=Random.Range((-width/4)-1,(width/4)+1);
            if(currY<width)currY=width;
            if(currY>fromRect.height-width)currY=fromRect.height-width;
        }
        toRet.filterMode=FilterMode.Point;
        toRet.Apply();
        return toRet;
    }
}


public class Rectangle: EnemySpriteMaker{
    public Texture2D spriteTemplate(Texture2D fromRect){
        return fromRect;
    }
}
public class Ellipse:EnemySpriteMaker{
    public Texture2D spriteTemplate(Texture2D fromRect){
        int sizeX=fromRect.width;
        int sizeY=fromRect.height;
        Texture2D toRet=new Texture2D(sizeX,sizeY);
        float r;
        for(int x=0;x<=sizeX;x++){
            for(int y=0;y<=sizeY;y++){
                //Debug.Log(""+x+"\t"+y);mj
                r=(x-sizeX/2f)*(x-sizeX/2f)/(sizeX*sizeX)+(y-sizeY/2f)*(y-sizeY/2f)/(sizeY*sizeY);
                toRet.SetPixel(x,y,
                new Color(
                    fromRect.GetPixel(x,y).r,
                    fromRect.GetPixel(x,y).g,
                    fromRect.GetPixel(x,y).b,
                    r<.2f?1:(.25f-r))
                );
            }
        }
        toRet.filterMode=FilterMode.Point;
        toRet.Apply();
        return toRet;
    }
}