using System.Collections;
using System.Collections.Generic; 
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
public static class SaveLoad {
    public static GamePlayer savedPlayer;
    public static void save() {
        savedPlayer=new GamePlayer();
        savedPlayer.importFrom(Player.mainPlayer);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create ("savedGames.rb");
        bf.Serialize(file, SaveLoad.savedPlayer);
        file.Close();
    }
    public static void load() {
        if(File.Exists("savedGames.rb")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open("savedGames.rb", FileMode.Open);
            SaveLoad.savedPlayer = (GamePlayer)bf.Deserialize(file);
            savedPlayer.exportTo(Player.mainPlayer);
            file.Close();
        }
    }
}
