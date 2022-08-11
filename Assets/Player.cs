using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Player : MonoBehaviour {
    public string[] fieldControls=new string[5];//{"up","down","left","right","space"};
    public string[,] movementControls=new string[,]{
            {"w","s","a","d","q","e","z","c"},
            {"i","k","j","l","u","o","m","."}
            };
    public Sprite[] sprites  = new Sprite[4];
    public Monster[] myMonsters = new Monster[2];
    public Lazer[] lazers = new Lazer[1];
    public int[] killCount;
    public int[] parts;
    public static Player mainPlayer;
    public int stepsSinceLastEncounter=0;
    public bool isEnabled=true;
    float speed = 1/8f;
    public Lazer lazerPrefab;
    public FightParty fightPartyPrefab;
    public Monster MonsterPrefab;
    public List<UpgradeOption> UpgradeOptions=new List<UpgradeOption>();
    public static bool isFighting=false;
	public byte UnlockedUpgradeOptions=0;
	public int level=0;
    public Random.State CurrentFloorGeneratedFrom;
	public bool goingToNextLevel=false;
    public int[] collecteds=new int[3];
    /*Each index corresponds to a level of a thing being collected
    *collecteds[0]:Data Collection
    **Push space to generate an encounter, up to 5 times per area per level
    *collecteds[1]:Exploration
    **Each level increases traversal-mode speed
    *collecteds[2]:Navigation
    **Level 1:Taxi Arrow points to the end of the level
    **Level 2:Encounters are shown for the tile you're on
    **Level 3:World Map is visible when escape key is pressed
    **/
    public int generatedEncountersThisLevel=5;
    public int mouseOnUpgrade=0;

    public string[] flavorTutorials;
    public Queue<string> text=new Queue<string>();
    public float timeTillNextDequeue=10f;
    //These are used for the tutorials. 

	void OnGUI(){
		if(MenuAndGlobals.isInMenu){
			/*if(GUI.Button(new Rect(0,0,Screen.width/3,Screen.height/4),"Save")){
				SaveLoad.save();
			}
			if(GUI.Button(new Rect(0,Screen.height/4,Screen.width/3,Screen.height/4),"Load")){
				SaveLoad.load();
			}*/
            if(GUI.Button(new Rect(0,3*Screen.height/4,Screen.width/4,Screen.height/4),"Quit")){
                Application.Quit();
            }
			MenuAndGlobals.menu.GetComponentInChildren<AudioSource>().volume=
				Mathf.Pow(
					GUI.HorizontalSlider(new Rect(0,Screen.height/2,Screen.width/3,Screen.height/8),
					Mathf.Pow(MenuAndGlobals.menu.GetComponentInChildren<AudioSource>().volume,.25f),0,1)
				,4);
            if(myMonsters[0].stats[(int)Monster.Stats.speed]>30||myMonsters[1].stats[(int)Monster.Stats.speed]>30){
                if(GUI.Button(new Rect(0,0,Screen.width/8f,Screen.height/8f),"Help! My Scanners\nmove too fast!")){
                    foreach(Monster i in myMonsters){
                        if(i.stats[(int)Monster.Stats.speed]>30)i.stats[(int)Monster.Stats.speed]-=5;
                    }
                }
            }
		}
	}/**/
    void Start(){
        Physics.queriesHitTriggers=true;
        for(int i=0;i<flavorTutorials.Length;i++){
            flavorTutorials[i]=flavorTutorials[i].Replace("\\n","\n");
        }
        Application.targetFrameRate = 60;
        mainPlayer=this;
        killCount=new int[MenuAndGlobals.typesOfMonsters];
        parts=new int[MenuAndGlobals.typesOfMonsters];
		for(int i=0;i<myMonsters.Length;i++){
			//myMonsters[i]=Instantiate(MenuAndGlobals.Monsters[i]);
			myMonsters[i].InitHealthBars();
		}


        initLazers();
        //AddText("Use the Arrow Keys to move.");
        GetComponentInChildren<Camera>().aspect=1;
    }

    public void initLazers(){
        if(lazers.Length!=myMonsters[0].stats[(int)Monster.Stats.power]+myMonsters[1].stats[(int)Monster.Stats.power]+1){
            foreach(Lazer i in lazers){
                if(i)DestroyImmediate(i.gameObject);
            }
            lazers=new Lazer[Player.mainPlayer.myMonsters[0].stats[(int)Monster.Stats.power]+Player.mainPlayer.myMonsters[1].stats[(int)Monster.Stats.power]+1];
        }
        if(lazers[0]==null){
            for(int i=0;i<lazers.Length;i++){
                lazers[i]=Instantiate(lazerPrefab);
            }
        }
    }
    public void AddText(string x, float time=10f){
        text.Enqueue(x);
        timeTillNextDequeue=time;
    }
    public void restart () {
        mainPlayer=null;
        SceneManager.LoadScene("Main Menu",LoadSceneMode.Single);
        isFighting=false;
    }
    public bool IsTouching(Thing that)
    {
        return this.GetComponent<Collider2D>().Distance(that.GetComponent<Collider2D>()).distance<=0f;
    }
    // Update is called once per frame
    void FixedUpdate () {
        if(goingToNextLevel){
            goingToNextLevel=false;
            Film.AddColor(new Color(1,1,1,1),2);
            Invoke("nextLevel",2);
            Film.AddColor(new Color(1,1,1,0),2);

        }
        if(killCount==null){
            killCount=new int[MenuAndGlobals.typesOfMonsters];
            parts=new int[MenuAndGlobals.typesOfMonsters];
        }
        if(isEnabled){
            getMovementInput();
        }
        if(!isFighting){
            myMonsters[0].transform.position=transform.position+new Vector3(-1,0,0);
            myMonsters[1].transform.position=transform.position+new Vector3(1,0,0);
        }
        doLazers();
        getUpgradeSelectionInput();

        if(getBackground()!=null&&getBackground().music!=null&&MenuAndGlobals.menu.GetComponentInChildren<AudioSource>().clip!=getBackground().music)
        {
            MenuAndGlobals.menu.GetComponentInChildren<AudioSource>().clip=getBackground().music;
            MenuAndGlobals.menu.GetComponentInChildren<AudioSource>().Play();
        }
        if(collecteds[2]>=1){
            TaxiArrow.Arrow.pointingSpot=BossArena.center;
            TaxiArrow.Arrow.isActive=true;
        }
        if(collecteds[2]>=2&&!isFighting){
            for(int i=0;i<getBackground().Monsters.Length;i++){
                getBackground().Monsters[i].transform.position=transform.position+new Vector3(-4+i,-4,0);
                getBackground().Monsters[i].transform.localScale=new Vector3(.25f,.25f,1);
            }
        }
        timeTillNextDequeue-=Time.deltaTime;
        if(timeTillNextDequeue<=0){
            try{text.Dequeue();}catch(System.Exception){};
            timeTillNextDequeue=10f;
        }
        GetComponentInChildren<TextMesh>().text=string.Join("\n",((string[])text.ToArray()));
    }
    public void enable(){
        isEnabled=true;
        this.GetComponent<SpriteRenderer>().color=Color.white;
    }
    public void disable(){
        isEnabled=false;
        this.GetComponent<SpriteRenderer>().color=Color.black;
    }
    public void getMovementInput()
    {
        /*myMonsters[0].transform.position=transform.position+new Vector3(-3,-3);
        myMonsters[1].transform.position=transform.position+new Vector3(3,-3);/**/
        if(Input.GetKey("y"))
        {
            SaveLoad.load();
        }
        if(Input.GetKey("r"))
        {
            SaveLoad.save();
        }
        Vector3 dir = new Vector3(
            (Input.GetKey(movementControls[0,2])||Input.GetKey(movementControls[1,2])? -1:0)+
            (Input.GetKey(movementControls[0,3])||Input.GetKey(movementControls[1,3])? 1:0),
            (Input.GetKey(movementControls[0,0])||Input.GetKey(movementControls[1,0])? 1:0)+
            (Input.GetKey(movementControls[0,1])||Input.GetKey(movementControls[1,1])? -1:0),
            0
        );
        Vector3 temp=transform.position;
        transform.Translate(speed*dir*Mathf.Pow(1.1f,collecteds[1]));
        if(getBackground()==null) transform.position=temp;
        dir=new Vector3(0,0,0);
        if(Input.GetKey(movementControls[0,0])||Input.GetKey(movementControls[1,0]))
        {
            if(getBackground()!=null)resolveRandomEncounter();
            GetComponent<SpriteRenderer>().sprite = sprites[0];
            stepsSinceLastEncounter++;
        }
        if(Input.GetKey(movementControls[0,1])||Input.GetKey(movementControls[1,1]))
        {
            if(getBackground()!=null)resolveRandomEncounter();

            GetComponent<SpriteRenderer>().sprite = sprites[1];
            stepsSinceLastEncounter++;
        }
        if(Input.GetKey(movementControls[0,2])||Input.GetKey(movementControls[1,2]))
        {
            if(getBackground()!=null)resolveRandomEncounter();
            GetComponent<SpriteRenderer>().sprite = sprites[2];
            stepsSinceLastEncounter++;
        }
        if(Input.GetKey(movementControls[0,3])||Input.GetKey(movementControls[1,3]))
        {
            if(getBackground()!=null)resolveRandomEncounter();
            GetComponent<SpriteRenderer>().sprite = sprites[3];
            stepsSinceLastEncounter++;
        }
        if(Input.GetKey(fieldControls[4]))
        {
            if(collecteds[0]>0&&generatedEncountersThisLevel>=1){
                generateRandomEncounter(getBackground());
                generatedEncountersThisLevel--;
            }
        }
    }
    public void getUpgradeSelectionInput(){
        int dir=-1;
        int[]change=new int[]{-3,3,-1,1};
        for(int i=0;i<=3;i++){
            if(Input.GetKeyDown(fieldControls[i])){ 
                dir=i;
                break;
            }
        }
        if(dir!=-1){
            if(UpgradeOptions.Count>mouseOnUpgrade&&mouseOnUpgrade>=0)UpgradeOptions[mouseOnUpgrade].OnMouseExit();
            do{
                mouseOnUpgrade+=change[dir];
            }while(mouseOnUpgrade>=0&&mouseOnUpgrade<UpgradeOptions.Count&&!UpgradeOptions[mouseOnUpgrade].available());
            while(mouseOnUpgrade<UpgradeOptions.Count&&(mouseOnUpgrade<0||!UpgradeOptions[mouseOnUpgrade].available()))mouseOnUpgrade++;
            while(mouseOnUpgrade>=0&&(mouseOnUpgrade>=UpgradeOptions.Count||!UpgradeOptions[mouseOnUpgrade].available()))mouseOnUpgrade--;
            if(UpgradeOptions.Count>mouseOnUpgrade&&mouseOnUpgrade>=0)UpgradeOptions[mouseOnUpgrade].OnMouseOver();
        }
        if(Input.GetKeyDown(fieldControls[5])){
            if(UpgradeOptions.Count>mouseOnUpgrade&&mouseOnUpgrade>=0)UpgradeOptions[mouseOnUpgrade].OnMouseDown();
        }
    }
    public void doLazers()
    {
        if(myMonsters[1].transform.position.x==myMonsters[0].transform.position.x)
        {
            myMonsters[0].transform.Translate(1/96f,0,0);
        }
        float slope =
            (myMonsters[1].transform.position.y-myMonsters[0].transform.position.y)/
            (myMonsters[1].transform.position.x-myMonsters[0].transform.position.x);
        Vector3 center=(myMonsters[1].transform.position+myMonsters[0].transform.position)/2f+new Vector3(0,0,-1);
        float dx = myMonsters[1].transform.position.x-center.x;
        float x;
        for(int i = 0; i < myMonsters[0].stats[(int)Monster.Stats.power]; i++)
        {
            x = (myMonsters[0].transform.position.x+dx/(myMonsters[0].stats[(int)Monster.Stats.power]+1)*(i+1));
            lazers[i].transform.position=new Vector3(x,slope*(x-myMonsters[0].transform.position.x)+myMonsters[0].transform.position.y,-1);
        }
        lazers[myMonsters[0].stats[(int)Monster.Stats.power]].transform.position=center;
        for(int i=myMonsters[0].stats[(int)Monster.Stats.power]+1;i<lazers.Length;i++){
            x = (center.x+dx/(myMonsters[1].stats[(int)Monster.Stats.power]+1)*(i-myMonsters[0].stats[(int)Monster.Stats.power]));
            lazers[i].transform.position=new Vector3(x,slope*(x-center.x)+center.y,-1);
        }
    }
	public void nextLevel(){
		//RLManager.Generator.transform.Translate(1000,1000,0);      
        level++;
        generatedEncountersThisLevel=5*collecteds[0];
        transform.position=new Vector3(level*1000,0,0);
        if(level<RLManager.LevelsThisRun){
            StartCoroutine(RLManager.Generator.GenerateFloor());
            AddText(flavorTutorials[Random.Range(0,flavorTutorials.Length)]);
        }
        else if(level==RLManager.LevelsThisRun)
            RLManager.Generator.GenerateBossOnlyFloor();
        else
            AddText("Your mission on this\n"+
                    "planet is complete.\n");
		//SaveLoad.save();
	}
    //returns a monster of the user's choice
    public Monster getMonster()
    {
        myMonsters[0].transform.Translate(transform.position+new Vector3(-2,-1,0));
        myMonsters[1].transform.Translate(transform.position+new Vector3(2,-1,0));
        myMonsters[0].chosen=false;
        myMonsters[1].chosen=false;
        if(myMonsters[0].chosen==true)
            return myMonsters[0];
        else if(myMonsters[1].chosen==true)
            return myMonsters[1];
        else return null;
    }
    public Background getBackground()
    {
        Collider2D[] possBs = Physics2D.OverlapAreaAll(    new Vector2(transform.position.x-.05f,transform.position.y-.05f),
                                                           new Vector2(transform.position.x+.05f,transform.position.y+.05f));
        foreach(Collider2D x in possBs)
        {
            if(x.GetComponent<Background>())
            {
                //Debug.Log(x.GetComponent<Collider2D>().Distance(GetComponent<Collider2D>()).distance);
                return x.GetComponent<Background>();
            }
        }
        return null;
    }
    public void resolveRandomEncounter()
    {
        Background b = getBackground();
        if(stepsSinceLastEncounter>200&&Random.Range(0f,1f)<b.encounterChance)
        {
            generateRandomEncounter(b);
        }
    }
    public void generateRandomEncounter(Background b)
    {
        stepsSinceLastEncounter=0;
        FightParty party = Instantiate(fightPartyPrefab);
        Monster temp = b.getEnemy();
        if(parts[temp.spriteDex]>=1)
        {
            Destroy(temp.gameObject);
            temp = b.getEnemy();
        }
        party.init(myMonsters,temp);
        isFighting=true;
        this.GetComponent<Rigidbody2D>().constraints=RigidbodyConstraints2D.FreezePosition;
        disable();
    }

    public void refreshUpgradeOptionColors(){
        for(int i=0;i<UpgradeOptions.Count;i++){
            UpgradeOptions[i].UseCorrectColor();
			
        }
		
    }
}