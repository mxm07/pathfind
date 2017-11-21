using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Enums;

public class PUtility : MonoBehaviour {
    //Shorthand way to spawn levels, tiles, and objects
    public static PLevel nlev(int a, int b) { PLevel t = (PLevel)ScriptableObject.CreateInstance(typeof(PLevel)); t.init(a, b); return t; }
    public static PLevel nlev(int[,] a) { PLevel t = (PLevel)ScriptableObject.CreateInstance(typeof(PLevel)); t.init(a); return t; }
    public static PLevel nlev(int[,] a, int[,] b) { PLevel t = (PLevel)ScriptableObject.CreateInstance(typeof(PLevel)); t.init(a, b); return t; }
    public static PLevel nlev(int[,] a, int[,] b, string c, int d, int e, string f, int g, bool h) { PLevel t = (PLevel)ScriptableObject.CreateInstance( typeof( PLevel ) ); t.init(a, b, c, d, e, f, g, h); return t; }

    public static PTile ntile() { PTile t = (PTile)ScriptableObject.CreateInstance(typeof(PTile)); t.init(); return t; }
    public static PTile ntile( int type, Vector3 pos ) { PTile t = (PTile)ScriptableObject.CreateInstance(typeof(PTile)); t.init(type, pos); return t; }

    public static PObject nobj() { PObject t = (PObject)ScriptableObject.CreateInstance(typeof(PObject)); t.init(); return t; }
    public static PObject nobj(int type, PTile tile) { PObject t = (PObject)ScriptableObject.CreateInstance(typeof(PObject)); t.init(type, tile); return t; }
    public static PObject nobj(int type, PTile tile, float offsetY) { PObject t = (PObject)ScriptableObject.CreateInstance(typeof(PObject)); t.init(type, tile, offsetY); return t; }
    

    public static Vector3 dirToVec(Dir dir) {
        return new Vector3(dir == Dir.Left ? -1 : (dir == Dir.Right ? 1 : 0), 0, dir == Dir.Down ? -1 : (dir == Dir.Up ? 1 : 0));
    }
    public static Quaternion dirToAng(Dir dir) {
        return Quaternion.Euler((dir == Dir.Up ? 90 : (dir == Dir.Down ? -90 : 0)), 0, (dir == Dir.Left ? 90 : (dir == Dir.Right ? -90 : 0)));
    }

    public static bool loadFiles() {
        Dictionary<string, object> s; PLevel[] l;
        return loadFiles(out s, out l);
    }
    public static bool loadFiles( out Dictionary<string, object> newSettings, out PLevel[] newLevels ) {
        newSettings = null;
        newLevels = null;
        
        if (!File.Exists(Application.persistentDataPath + "/settings.gd") || !File.Exists(Application.persistentDataPath + "/levels.gd")) return false;

        BinaryFormatter bf;
        FileStream file;

        bf = new BinaryFormatter();
        file = File.Open(Application.persistentDataPath + "/settings.gd", FileMode.Open);
        newSettings = (Dictionary<string, object>)bf.Deserialize(file);
        file.Close();
        
        file = File.Open(Application.persistentDataPath + "/levels.gd", FileMode.Open);
        newLevels = levelsFromDeserialized((List<Dictionary<string, object>>)bf.Deserialize(file));
        file.Close();

        return true;
    }

    public static void saveFiles( Dictionary<string, object> newSettings, PLevel[] newLevels ) {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/settings.gd");
        bf.Serialize(file, newSettings);
        file.Close();

        bf = new BinaryFormatter();
        file = File.Create(Application.persistentDataPath + "/levels.gd");
        bf.Serialize(file, levelsToSerializable( newLevels ));
        file.Close();
    }
	
    public static List<Dictionary<string, object>> levelsToSerializable( PLevel[] levels ) {
        List<Dictionary<string, object>> levels_s = new List<Dictionary<string, object>>();

        for( int i = 0; i < levels.Length; i++ ) {
            PLevel l = levels[i];
            levels_s.Add(new Dictionary<string, object> {
                { "level", l.level },
                { "objects", l.objects },
                { "name", l.GetLevelName() },
                { "difficulty", l.GetDifficulty() },
                { "pack", l.GetPack() },
                { "author", l.GetAuthor() },
                { "bestscore", l.GetBestScore() },
                { "user_level", l.IsUserLevel() }
            });
        }

        return levels_s;
    }
    public static PLevel[] levelsFromDeserialized( List<Dictionary<string, object>> levels_s ) {
        PLevel[] level = new PLevel[levels_s.Count];

        for( int i = 0; i < levels_s.Count; i++ ) {
            Dictionary<string, object> dict = levels_s[i];

            level[i] = nlev((int[,])dict["level"], (int[,])dict["objects"], (string)dict["name"], (int)dict["difficulty"], (int)dict["pack"], (string)dict["author"], (int)dict["bestscore"], (bool)dict["user_level"]);
        }

        return level;
    }

    
    public void gameInit() {
        if (loadFiles()) return;

        Dictionary<string, object> settings = new Dictionary<string, object> {
            { "quality", 0 },
            { "particles", false },
            { "movespeed", 3f },
            { "music", true },
            { "sounds", true },
            { "zoom", 1f }
        };
        //public PLevel(int[,] tiles, string levelName, int difficulty, int pack, string author, int bestScore, bool user_level) {
        PLevel[] levels = new PLevel[] {
            nlev(
                new int[,] {
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 2, 1, 4, 1, 1, 6, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 5, 1, 1, 1, 1, 7, 1, 1, 3 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
                },
                new int[4,12],"easy00", 0, 1, "Built-in", -1, false),
            nlev(
                new int[,] {
                    { 0, 0, 0, 0, 3, 0, 0, 0, 0 },
                    { 0, 1, 1, 1, 1, 8, 1, 7, 1 },
                    { 1, 8, 1, 7, 5, 5, 6, 7, 1 },
                    { 1, 5, 1, 8, 5, 1, 1, 1, 1 },
                    { 1, 1, 1, 8, 8, 8, 1, 5, 1 },
                    { 1, 1, 1, 8, 1, 6, 1, 5, 1 },
                    { 1, 5, 1, 1, 5, 1, 1, 1, 1 },
                    { 1, 1, 1, 6, 1, 8, 1, 1, 1 },
                    { 0, 0, 0, 0, 2, 6, 1, 1, 1 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                },
                new int[9,9],"easy01", 0, 1, "Built-in", -1, false),
            nlev(
                new int[,] {
                    { 0, 0, 1, 1 },
                    { 0, 1,28,17 },
                    { 0, 1,28, 0 },
                    { 0,19,28, 1 },
                    { 0, 0,28, 1 },
                    { 1, 1,28,17 },
                    { 1, 0, 1, 0 },
                    { 1, 0, 1, 1 },
                    { 1, 0, 1, 1 },
                    { 1, 0, 1, 1 },
                    { 3, 0, 2, 0 }
                },
                new int[11,4],"easy02", 0, 1, "Built-in", -1, false)
        };
        saveFiles(settings, levels);
    }

    void Awake() {
        gameInit();
    }
}
