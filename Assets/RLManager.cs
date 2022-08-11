using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RLManager : MonoBehaviour {

    // Use this for initialization
    public Texture2D toShow;
    public Monster MonsterPrefab;
    public Background BackgroundPrefab;
    public LevelType[] levelTypes;
    public Random.State GenerateFrom;
    public UpgradeOption UpgradeOptionPrefab;
    public static RLManager Generator;
    public List<UpgradeOption> UpgradeOptions;
    public bool isGeneratingSprite;
    public bool doneGeneratingUpgrades;
    public static int LevelsThisRun=10;
    public int FirstLevel=4;
    public int[] LevelTypesThisRun;
    void Start () {
        if(Player.mainPlayer!=null&&Player.mainPlayer.level!=0)
            Random.state=GenerateFrom;
        LevelTypesThisRun=new int[LevelsThisRun];
        LevelTypesThisRun[0]=FirstLevel==-1?FirstLevel:Random.Range(0,levelTypes.Length);
        for(int i=1;i<LevelsThisRun;i++){
            LevelTypesThisRun[i]=Random.Range(0,levelTypes.Length-1);
            if(LevelTypesThisRun[i]>=LevelTypesThisRun[i-1])LevelTypesThisRun[i]++;
        }
        Generator=this;
        doneGeneratingUpgrades=false;
        //Create the Monsters
        for(int i=MenuAndGlobals.menu.MonstersEditable.Length;i<MenuAndGlobals.typesOfMonsters;i++){
            StartCoroutine(
                NewMonster(  (call) => {MenuAndGlobals.Monsters.Add(call);} )
             );
        }
        //GenerateFloor();
    }
    void Update(){
        if(!doneGeneratingUpgrades&&MenuAndGlobals.Monsters.Count>=MenuAndGlobals.typesOfMonsters){
            for(int i=0;i<MenuAndGlobals.typesOfMonsters;i++){
                MenuAndGlobals.Monsters[i].spriteDex=i;
            }
            StartCoroutine(GenerateFloor());
            UpgradeOptions=GenerateUpgrades((Monster[])(MenuAndGlobals.Monsters.ToArray()));
            doneGeneratingUpgrades=true;
            for(int i=0;i< UpgradeOptions.Count;i++){
                if(i==0){
                    UpgradeOptions[0].transform.position=new Vector3(-2005,2,0);
                }
                else{
                    UpgradeOptions[i].transform.position=UpgradeOptions[i-1].transform.position+
                    new Vector3(
                    i%3==0?-8f:4f,
                    i%3==0?
                            -(1.2f+Mathf.Max(UpgradeOptions[i-1].requiredParts.Length,UpgradeOptions[i-2].requiredParts.Length,UpgradeOptions[i-3].requiredParts.Length)*.6f)
                    :
                    0,//-1.2f+UpgradeOptions[i-1].requiredParts.Length*.6f,
                    .01f);
                }
                UpgradeOptions[i].gameObject.AddComponent(typeof(BoxCollider2D));
            }
            Player.mainPlayer.UpgradeOptions=UpgradeOptions;
        }
    }
    public IEnumerator NewMonster(System.Action<Monster> callback){
        Monster toRet;
        int sizeX=Random.Range(4,32);
        int sizeY=Random.Range(4,32);
        float[,,] colors=new float[sizeX,sizeY,4];
        yield return StartCoroutine(
                NewSprite(sizeX, sizeY, sizeX/4,sizeY/4, (call) => {colors=call;})
            );
        float magnitude=new Vector3(colors[0,0,0],colors[0,0,1],colors[0,0,2]).magnitude;
        int speed=(int)(colors[0,0,0]*MenuAndGlobals.proportions.x/magnitude+1);
        int power=(int)(colors[0,0,1]*MenuAndGlobals.proportions.y/magnitude+1);
        int health=(int)(colors[0,0,2]*MenuAndGlobals.proportions.z/magnitude+1);
        toRet=Instantiate(MonsterPrefab).create(
            speed,power,health,FloatArrayToSprite(colors));
        toRet.transform.position=new Vector3(-20000,20000,0);
        toRet.isWild=true;
        callback(toRet);
    }

    public List<UpgradeOption> GenerateUpgrades(Monster[] monsters){
        Stack<int> sPosses=new Stack<int>();
        Stack<int> pPosses=new Stack<int>();
        Stack<int> hPosses=new Stack<int>();
        Stack<int> rPosses=new Stack<int>();
        List<UpgradeOption> toRet = new List<UpgradeOption>();
        for(int i=2;i<monsters.Length;i++){

            if(monsters[i].stats[0]/MenuAndGlobals.proportions.x>monsters[i].stats[1]/MenuAndGlobals.proportions.y){//if speed>pow
                if(monsters[i].stats[0]/MenuAndGlobals.proportions.x>monsters[i].stats[2]/MenuAndGlobals.proportions.z){//if speed>health
                    sPosses.Push(i);
                    continue;
                }
                else{
                    if(Random.value>.5f)rPosses.Push(i);
                    hPosses.Push(i);
                    continue;
                }
            }
            //pow>speed
            if(Random.value>.5f)rPosses.Push(i);
            if(monsters[i].stats[1]/MenuAndGlobals.proportions.y>monsters[i].stats[2]/MenuAndGlobals.proportions.z){//if pow>health
                pPosses.Push(i);
                continue;
            }
            hPosses.Push(i);
        }
        Stack<int>[] sphr=new Stack<int>[]{sPosses,pPosses,hPosses,rPosses};
        for(int i=2;i<monsters.Length;i++)
        {
            foreach(Stack<int> s in sphr){
                if(Random.value<.2f)s.Push(i);
            }
        }
        foreach(Stack<int> s in sphr){
            s.Push(Random.Range(2,monsters.Length));
            s.Push(Random.Range(2,monsters.Length));
        }
        int UpgradeLength=1;
        int[]requiredMonsters;
        float[]requiredAmounts;
        byte stat=0;
        int amount=0;
        int index=0;
        while(sPosses.Count>0||pPosses.Count>0||hPosses.Count>0||rPosses.Count>0){
            /**/ if(sPosses.Count>0)stat=0;
            else if(pPosses.Count>0)stat=1;
            else if(hPosses.Count>0)stat=2;
            else if(rPosses.Count>0)stat=3;
            if(stat==3)UpgradeLength=1;
            else{
                switch(sphr[stat].Count){
                    case 1:
                        UpgradeLength=1;
                        break;
                    case 2:case 4:
                        UpgradeLength=2;
                        break;
                    case 3:
                        UpgradeLength=3;
                        break;
                    case 5:
                        UpgradeLength=Random.Range(2,3);
                        break;
                    default:
                        UpgradeLength=Random.Range(2,4);
                        break;
                }
            }
            /*while(
                stat!=3&&
                Random.value>.5f&&
                UpgradeLength<sphr[stat].Count()
                )UpgradeLength++;/**/



            HashSet<int> tempRequiredMonsters=new HashSet<int>();
            for(int i=0;i<UpgradeLength;i++)
                tempRequiredMonsters.Add(sphr[stat].Pop());
            UpgradeLength=tempRequiredMonsters.Count;
            requiredMonsters=new int[UpgradeLength];
            tempRequiredMonsters.CopyTo(requiredMonsters);
            requiredAmounts=new float[UpgradeLength];
            do{
                for(int i=0;i<UpgradeLength;i++)
                    requiredAmounts[i]=Random.value*2+1;
                amount=
                    stat==0?
                        Mathf.CeilToInt(Random.value*MenuAndGlobals.proportions.x*requiredAmounts.Sum()/4f)
                        :
                    stat==1?
                        Mathf.CeilToInt(Random.value*MenuAndGlobals.proportions.y*requiredAmounts.Sum()/4f)
                        :
                    stat==2?
                        Mathf.CeilToInt(Random.value*MenuAndGlobals.proportions.z*requiredAmounts.Sum()/4f)
                        :
                    //stat=3
                        requiredAmounts[0]<2?
                            10
                            :
                        requiredAmounts[0]<3?
                            Random.Range(20,40)
                            :
                        requiredAmounts[0]<4?
                            Random.Range(60,75)
                            :
                            100
                        ;
            }while(
                stat!=3&&
            amount<(
            stat==0?
                1
                :
            stat==1?
                0
                :
            //stat==2
                20)
            );
            toRet.Add(
            Instantiate(UpgradeOptionPrefab.create(stat,amount,requiredMonsters,requiredAmounts,index++)));
        }
        return toRet;
    }
    public void GenerateBossOnlyFloor(){
        if(Player.mainPlayer!=null&&Player.mainPlayer.level!=0)
            Random.state=GenerateFrom;
        int levelType=Random.Range(0,levelTypes.Length);
        
        Sprite backgroundSprite=Sprite.Create(new Texture2D(1,5),new Rect(new Vector2(0,0),new Vector2(1,5)),new Vector3(0.5f,0.5f),pixelsPerUnit:.1f);
        Background path=Instantiate(BackgroundPrefab).Create(0,new Monster[]{MenuAndGlobals.Monsters[0]},levelTypes[levelType].fightPartyPrefab,backgroundSprite);
        path.transform.position=transform.position+new Vector3(0,25,0);
        Sprite bossSprite=Sprite.Create(new Texture2D(1,1),new Rect(new Vector2(0,0),new Vector2(1,1)),new Vector3(0.5f,0.5f),pixelsPerUnit:.11f);
        Background boss=Instantiate(BackgroundPrefab).Create(0,new Monster[]{MenuAndGlobals.Monsters[Random.Range(2,MenuAndGlobals.typesOfMonsters)]},levelTypes[levelType].fightPartyPrefab,bossSprite);
        boss.GetComponent<SpriteRenderer>().color=levelTypes[levelType].backgroundColor;
        boss.size=2;
        boss.transform.position=transform.position+new Vector3(0,50,0);
        boss.gameObject.AddComponent<BossArena>();
        boss.music=levelTypes[levelType].bossMusic;
        GenerateFrom=Random.state;
    }
    public IEnumerator NewBackground(System.Action<Background> callback, float[] color=null,int fightPartyIndex=0,bool isSpecial=false){
        Background toRet;
        Monster[] encounters=isSpecial?new Monster[1]:new Monster[Random.Range(2,7)];

        //figure out which life forms can be encountered on this tile
        for(int i=0;i<encounters.Length;i++){
            
            encounters[i]=Instantiate(MenuAndGlobals.Monsters[Random.Range(2,MenuAndGlobals.typesOfMonsters-1)]);
            Texture2D texture=encounters[i].GetComponent<SpriteRenderer>().sprite.texture;
            float[] average=Average(texture);
            texture=EnemyTypes.enemyTypes[
                        levelTypes[fightPartyIndex].enemies[Random.Range(0,levelTypes[fightPartyIndex].enemies.Length)]
                    ]
                    .spriteTemplate(texture);
            EnemyTypes.Border(texture,average);
            encounters[i].GetComponent<SpriteRenderer>().sprite=Sprite.Create(
                texture,
                new Rect(
                    new Vector2(0,0),new Vector2(
                        encounters[i].GetComponent<SpriteRenderer>().sprite.texture.width,
                        encounters[i].GetComponent<SpriteRenderer>().sprite.texture.height)
                    ),
                new Vector2(
                    .5f,
                    .5f
                ),
                encounters[i].GetComponent<SpriteRenderer>().sprite.pixelsPerUnit
                );

        }
        int sizeX=Random.Range(40,isSpecial?50:200);
        int sizeY=Random.Range(40,isSpecial?50:200);
        float[,,] colors=new float[sizeX,sizeY,4];
        yield return StartCoroutine(NewSprite(
                    sizeX, sizeY,2,2,
                    (call) => {colors=call;},color,.06f
                ));
        toRet=Instantiate(BackgroundPrefab).Create(isSpecial?0:0.01f,encounters,levelTypes[fightPartyIndex].fightPartyPrefab,FloatArrayToSprite(colors));
        toRet.music=levelTypes[fightPartyIndex].music;
        callback(toRet);
    }
    public Background NewTile(float[] color=null,int levelTypeIndex=0,bool isSpecial=false){
        Background toRet;
        Monster[] encounters=isSpecial?new Monster[1]:new Monster[Random.Range(2,7)];
        for(int i=0;i<encounters.Length;i++){
            
            encounters[i]=Instantiate(MenuAndGlobals.Monsters[Random.Range(2,MenuAndGlobals.typesOfMonsters-1)]);
            Texture2D texture=encounters[i].GetComponent<SpriteRenderer>().sprite.texture;
            float[] average=Average(texture);
            texture=EnemyTypes.enemyTypes[
                levelTypes[levelTypeIndex].enemies[Random.Range(0,levelTypes[levelTypeIndex].enemies.Length)]
                    ]
                    .spriteTemplate(texture);
            EnemyTypes.Border(texture,average);
            encounters[i].GetComponent<SpriteRenderer>().sprite=Sprite.Create(
                texture,
                new Rect(
                    new Vector2(0,0),new Vector2(
                        encounters[i].GetComponent<SpriteRenderer>().sprite.texture.width,
                        encounters[i].GetComponent<SpriteRenderer>().sprite.texture.height)
                    ),
                new Vector2(
                    .5f,
                    .5f
                ),
                encounters[i].GetComponent<SpriteRenderer>().sprite.pixelsPerUnit
                );
        }
        int sizeX=Random.Range(40,isSpecial?50:120);
        int sizeY=Random.Range(40,isSpecial?50:120);
        toRet=Instantiate(
            BackgroundPrefab.Create(isSpecial?0:.01f,encounters,levelTypes[levelTypeIndex].fightPartyPrefab,
                Sprite.Create(
                    new Texture2D(sizeX,sizeY),
                    new Rect(0,0,sizeX,sizeY),
                    new Vector2(.5f,.5f),
                    8
                )
            )
        );
        toRet.music=levelTypes[levelTypeIndex].music;
        return toRet;
    }
    public IEnumerator NewBossArena(System.Action<Background> callback,float[] color=null,int fightPartyIndex=0){
        int encounter = Random.Range(2,MenuAndGlobals.typesOfMonsters);
        float[,,] colors = new float[75,75,4];
        yield return StartCoroutine(NewSprite(75,75,2,2,
            (call) => {colors=call;},
            color));
        Background toRet=Instantiate(BackgroundPrefab.Create(0.0f,new Monster[]{MenuAndGlobals.Monsters[encounter]},levelTypes[fightPartyIndex].fightPartyPrefab,FloatArrayToSprite(colors)));
        toRet.size=Mathf.Pow(Random.Range(.71f,1.4f),2);
        toRet.gameObject.AddComponent<BossArena>();
        toRet.music=levelTypes[fightPartyIndex].bossMusic;
        callback(toRet);
    }

    public IEnumerator GenerateFloor(){
        if(Player.mainPlayer!=null&&Player.mainPlayer.level!=0)
            Random.state=GenerateFrom;
        int tiles=Random.Range(10,30);
        int specialTile=Random.Range(3,tiles-2);
        List<Background> tileList=new List<Background>();
        Vector3 prevTile=
            Player.mainPlayer!=null?
                new Vector3(Player.mainPlayer.level*1000,0,100)
                :
                new Vector3(0,0,100);
        Background currTile=null;

        int levelType=
            (Player.mainPlayer==null)?
            LevelTypesThisRun[0]
            :
            LevelTypesThisRun[Player.mainPlayer.level%levelTypes.Count()];//Player.mainPlayer.level%levelTypes.Count();
        float[] color=new float[]{levelTypes[levelType].backgroundColor.r,levelTypes[levelType].backgroundColor.g,levelTypes[levelType].backgroundColor.b};
            /*yield return StartCoroutine(NewBackground(
                (call) => {currTile=call;},
                color:color,
                fightPartyIndex:levelType
                ));*/
        currTile=NewTile(color,levelType,false);    
        currTile.transform.position=prevTile;
        tileList.Add(currTile);
        for(int i=0;i<=tiles;i++){
            prevTile=currTile.transform.position;
            if(i==tiles){
                yield return StartCoroutine(NewBossArena(
                    (call) => {currTile=call;},
                    color:new float[]{color[0]*2/3f,color[1]*2/3f,color[2]*2/3f},
                    fightPartyIndex:levelType
                    ));
                currTile.transform.position=prevTile+new Vector3(0,0,-.01f);
            }
            else{
                currTile=NewTile(color,levelType,isSpecial:i==specialTile); 
                currTile.transform.position=prevTile+new Vector3(0,0,-.01f);
                if(i!=specialTile)
                    tileList.Add(currTile);
            }
            //currTile.transform.rotation=prevTile.rotation;
            currTile.transform.Rotate(0,0,90*Random.Range(0,4));
            while(currTile.GetComponent<Collider2D>().OverlapCollider((new ContactFilter2D()).NoFilter(),new Collider2D[]{null})!=0){
                currTile.transform.Translate(1,0,0);
            }
            while(currTile.GetComponent<Collider2D>().OverlapCollider((new ContactFilter2D()).NoFilter(),new Collider2D[]{null})==0){
                currTile.transform.Translate(-.1f,0,0);
            }
            currTile.transform.Translate(-.1f,0,0);
            if(i==specialTile){
                currTile.addCollectible(Random.Range(0,Player.mainPlayer.collecteds.Length));
            }
        }
        float[,,]colors=new float[1,1,4];
        foreach(Background i in tileList){
            yield return StartCoroutine(NewSprite(i.GetComponent<SpriteRenderer>().sprite.texture.width,i.GetComponent<SpriteRenderer>().sprite.texture.height,3,3,(call) => {colors=call;},baseColor:color,itersTilNextYield:1000,itersPerYield:1000));
            i.GetComponent<SpriteRenderer>().sprite=FloatArrayToSprite(colors);
        }
        GenerateFrom=Random.state;
    }

    Sprite FloatArrayToSprite(float[,,] colors){
        Texture2D texture = new Texture2D(colors.GetLength(0),colors.GetLength(1));
        for(int x=0;x<colors.GetLength(0);x++){
            for(int y=0;y<colors.GetLength(1);y++){
                texture.SetPixel(x,y,new Color(colors[x,y,0],colors[x,y,1],colors[x,y,2],colors[x,y,3]));
            }
        }
        texture.filterMode=FilterMode.Point;
        texture.Apply();
        Sprite toRet = Sprite.Create(
            texture,
            new Rect(0,0,colors.GetLength(0),colors.GetLength(1)),
            new Vector2(.5f,.5f),
            8
        );

        return toRet;
        
    }
    /*sizeX is the width of the sprite
    * sizeY is the height
    * each pixel is generated by looking at the 2*xVariance+1 by 2*yVariance+1 square centered at that pixel, and takes the average color
    * unless there are no pixels around it, in which case it takes a random color.
    */
    public IEnumerator NewSprite(int sizeX, int sizeY, int xVariance, int yVariance, System.Action<float[,,]> callback,float[]baseColor=null,float colorVariance=.1f,int itersTilNextYield=2000,int itersPerYield=2000){
        float[,,] toRet=new float[sizeX,sizeY,4];
        int x=0;//iterative x
        int y=0;//iterative y
        int a=0;//iterative color-component (r or g or b)
        Random.State current;
        for(x=0;x<sizeX;x++){
            for(y=0;y<sizeY;y++){
                for(a=0;a<3;a++){
                    toRet[x,y,a]=0;
                }
                toRet[x,y,3]=1;
            }
        }
        List<int[]> order = new List<int[]>();
        for(x=0;x<sizeX;x++){
            for(y=0;y<sizeY;y++){
                for(a=0;a<3;a++){
                    order.Add(new int[]{x,y,a});
                }
            }
        }
        int rand;
        for(int i=0;i<order.Count-1;i++){
            rand = Random.Range(i,order.Count);
            int[] temp = order[i];
            order[i]=order[rand];
            order[rand]=temp;
        }
        
        foreach(int[]i in order){
            
            //toRet[i[0],i[1],i[2]]=.5f;
            int count=0;
            float total=0;//finding the average
            if(toRet[i[0],i[1],i[2]]!=0)continue;
            for(x=-xVariance;x<=xVariance;x++){
                for(y=-yVariance;y<=yVariance;y++){
                    try{
                        if(toRet[i[0]+x,i[1]+y,i[2]]!=0){
                            total+=toRet[i[0]+x,i[1]+y,i[2]];
                            count++;
                        }
                        
                    }
                    catch(System.Exception){
                        //Debug.Log(e);
                    }

                }
            }

            /*if(total==0){
                toRet[i[0],i[1],i[2]]=Random.value;
            }
            else{
                toRet[i[0],i[1],i[2]]=total/count;
            }
            //toRet[i[0],i[1],i[2]]=(baseColor==null?toRet[i[0],i[1],i[2]]:(toRet[i[0],i[1],i[2]]+3*baseColor[i[2]])/4);
            
            /**/
            toRet[i[0],i[1],i[2]]=
                total==0?
                    baseColor==null?
                        Random.value
                        :
                        baseColor[i[2]]
                    :
                    baseColor==null?
                        total/count
                        :
                        (total/count)*Random.Range(1-colorVariance,1+colorVariance)
                ;
            if(itersTilNextYield--<0){
                isGeneratingSprite=true;
                itersTilNextYield=itersPerYield;
                current=Random.state;
                yield return null;
                Random.state=current;
            }/**/

        }
        
        float[]border=Average(toRet);
        /*for(a=0;a<3;a++){
            for(y=0;y<sizeY;y+=sizeY-1)
                for(x=0;x<sizeX;x++)
                    toRet[x,y,a]=border[a];
            for(x=0;x<sizeX;x+=sizeX-1)
                for(y=0;y<sizeY;y++)
                    toRet[x,y,a]=border[a];
        }/**/

        for(a=0;a<3;a++)toRet[0,0,a]=border[a];
        isGeneratingSprite=false;
        callback(toRet);
        yield return null;
    }
    public static float[] Average(float[,,] list){
        int count=0;//list.GetLength(0)*list.GetLength(1);
        float totalR=0,totalG=0,totalB=0,totalA=1;
        for(int x=0;x<list.GetLength(0);x++){
            for(int y=0;y<list.GetLength(1);y++){
                if(list[x,y,3]==1){
                    totalR+=list[x,y,0];
                    totalG+=list[x,y,1];
                    totalB+=list[x,y,2];
                    count++;
                }
            }
        }
        return new float[]{totalR/count,totalG/count,totalB/count,totalA/count};
    }
    public static float[] Average(Texture2D list){
        int count=0;//list.GetLength(0)*list.GetLength(1);
        float totalR=0,totalG=0,totalB=0,totalA=1;
        for(int x=0;x<list.width;x++){
            for(int y=0;y<list.height;y++){
                if(list.GetPixel(x,y).a==1){
                    totalR+=list.GetPixel(x,y).r;
                    totalG+=list.GetPixel(x,y).g;
                    totalB+=list.GetPixel(x,y).b;
                    count++;
                }
            }
        }
        return new float[]{totalR/count,totalG/count,totalB/count,totalA/count};
    }
}
