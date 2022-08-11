using UnityEngine;
using System.Collections.Generic;
using System; 
using System.IO; 
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
[Serializable]
//Unfinished
public class GamePlayer{
    public int[] killCount;
    public int[] parts;
    public float positionX;
    public float positionY;
    public float positionZ;
    public int monsterCount;
    public int[][] stats;
    public int[] spriteDex;
    public int level;
    public int[] collecteds;
    public byte UnlockedUpgradeOptions;
    public int generatedEncountersThisLevel=5;
    public UnityEngine.Random.State CurrentFloorGeneratedFrom;
	public SavableUpgrade[] UpgradeOptions;
    public void importFrom(Player that)
    {
        this.killCount=that.killCount;
        this.parts=that.parts;
        this.positionX=that.transform.position.x;
        this.positionY=that.transform.position.y;
        this.positionZ=that.transform.position.z;
        this.monsterCount=that.myMonsters.Length;
        this.level=that.level;
        this.collecteds=that.collecteds;
        this.UnlockedUpgradeOptions=that.UnlockedUpgradeOptions;
        this.generatedEncountersThisLevel=that.generatedEncountersThisLevel;
        this.CurrentFloorGeneratedFrom=that.CurrentFloorGeneratedFrom;
        stats=new int[monsterCount][];
        for(int i=0;i<monsterCount;i++){
            stats[i]=that.myMonsters[i].stats;
            spriteDex[i]=that.myMonsters[i].spriteDex;
        }
		UpgradeOptions=new SavableUpgrade[that.UpgradeOptions.Count];
		for(int i=0;i<that.UpgradeOptions.Count;i++){
			UpgradeOptions[i]=new SavableUpgrade();
			UpgradeOptions[i].importFrom(that.UpgradeOptions[i]);
		}
    }
    public void exportTo(Player that)
    {
        for(int i=0;i<killCount.Length;i++){
            that.killCount[i]=this.killCount[i];
            if(this.parts[i]!=0){
                that.parts[i]=this.parts[i];
                GameObject doll=(GameObject)MenuAndGlobals.Instantiate(new GameObject(),MenuAndGlobals.menu.transform.position
                    +new Vector3(.75f,3.8f-(i*14/9f),1),Quaternion.identity);
                doll.transform.localScale=new Vector3(.5f,.5f,1);
                doll.AddComponent<SpriteRenderer>();
                doll.GetComponent<SpriteRenderer>().sprite=MenuAndGlobals.Monsters[i].GetComponent<SpriteRenderer>().sprite;
            }
        }
        that.transform.position=new Vector3(positionX,positionY,positionZ);
        
        for(int i=0;i<monsterCount;i++){
            MenuAndGlobals.Destroy(that.myMonsters[i].gameObject);
            that.myMonsters[i]=MonoBehaviour.Instantiate(that.MonsterPrefab);
            that.myMonsters[i].create(
                stats[i],MenuAndGlobals.Monsters[i].GetComponent<SpriteRenderer>().sprite);
        }
		for(int i=0;i<UpgradeOptions.Length;i++){
			UpgradeOptions[i].exportTo(that.UpgradeOptions[i]);
		}
        that.level=this.level;
        that.collecteds=this.collecteds;
        that.UnlockedUpgradeOptions=this.UnlockedUpgradeOptions;
        that.generatedEncountersThisLevel=this.generatedEncountersThisLevel;
        that.CurrentFloorGeneratedFrom=this.CurrentFloorGeneratedFrom;
        that.initLazers();
        that.getBackground().OnTriggerEnter2D(that.GetComponent<Collider2D>());
    }
}
