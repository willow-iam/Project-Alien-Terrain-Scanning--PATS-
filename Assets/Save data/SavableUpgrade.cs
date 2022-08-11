using UnityEngine;
using System.Collections;
using System; 
using System.IO; 
using System.Runtime.Serialization.Formatters.Binary;
[Serializable]
public class SavableUpgrade{
	public int stat;
	public int amount;
    public int index;
	public int[] requiredRobots;
	public float[] requiredParts;
	public void importFrom (UpgradeOption that) {
		this.stat=that.stat;
		this.amount=that.amount;
		this.requiredParts=that.requiredParts;
		this.requiredRobots=that.requiredRobots;
        this.index=that.index;
	}
	public void exportTo(UpgradeOption that){
		that.create(stat,amount,requiredRobots,requiredParts,index);
	}
}
