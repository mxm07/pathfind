using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Coord {
    public int x;
    public int y;

    public Coord(int x, int y) {
        this.x = x;
        this.y = y;    
    }
}

public class PTile : ScriptableObject {
    protected int type;
    private GameObject tile;

    public void init() { type = 1; spawnTile(); }
    public void init( int type, Vector3 pos ) { this.type = type; spawnTile(); setPos(pos); }

    private void spawnTile() {
        if (tile != null) Destroy(tile);

        GameObject prefab_container = GameObject.Find("ScriptContainer");
        if (!prefab_container) throw new System.Exception("Cannot find PrefabContainer!");
        GameObject[] prefabs = prefab_container.GetComponent<PrefabScript>().prefabs;

        //To make the level grids easier to read
        Dictionary<int, int> mappings = new Dictionary<int, int>() { { -1, 100 } };
        Dictionary<int, float> angles = new Dictionary<int, float>() { { 2, 90 }, { 3, 90 }, { 4, 90 }, { 5, 180 }, { 6, 270 } };


        if (mappings.ContainsKey(type))
            type = mappings[type];

        if( prefabs[type] != null )
            tile = (GameObject)Instantiate(prefabs[type], new Vector3(), Quaternion.Euler( 0, angles.ContainsKey(type) ? angles[type] : 0, 0 ) );
    }

    public GameObject getTile() { return tile; }
    public void setType( int type ) { this.type = type; }
    public int getType() { return type; }
    public void setPos( Vector3 pos ) {
        if (tile == null) return;
        tile.transform.position = pos;
    }
    public Vector3 getPos() {
        if (tile == null) return new Vector3();
        return tile.transform.position;
    }
}


public class PObject : ScriptableObject {
    protected int type;
    protected PTile tile;
    protected GameObject obj;
    protected float offsetY;

    public void init() { type = 1; offsetY = 0; }
    public void init( int type, PTile tile ) { this.type = type; this.tile = tile; }
    public void init( int type, PTile tile, float offsetY ) { this.type = type; this.tile = tile; this.offsetY = offsetY; }

    public void setType(int type) {
        if (obj != null) Destroy(obj);

        GameObject prefab_container = GameObject.Find("PrefabContainer");
        if (!prefab_container) throw new System.Exception("Cannot find PrefabContainer!");
        GameObject[] prefabs = prefab_container.GetComponent<PrefabScript>().prefabs;

        //To make the level grids easier to read
        Dictionary<int, int> mappings = new Dictionary<int, int>() { { -1, 100 } };

        if (mappings.ContainsKey(type))
            type = mappings[type];

        obj = (GameObject)Instantiate(prefabs[type], tile.getTile().transform.position + new Vector3( 0, offsetY, 0 ), Quaternion.identity);
    }
    public int getType() { return type; }
}

public class PLevel : ScriptableObject {
    public int[,] level;
    public PTile[,] tileReferences;
    public PTile startTile;

    public int[,] objects;
    public PObject[,] objectReferences;

    protected string levelName;
    protected int difficulty;
    protected int pack;
    protected string author;
    protected int bestScore;
    protected bool userLevel = false;

    //Public get/set methods
    public void SetLevelName(string levelName) { this.levelName = levelName; }
    public string GetLevelName() { return levelName; }
    public void SetDifficulty(int difficulty) { this.difficulty = difficulty; }
    public int GetDifficulty() { return difficulty; }
    public void SetPack(int pack) { this.pack = pack; }
    public int GetPack() { return pack; }
    public void SetAuthor(string author) { this.author = author; }
    public string GetAuthor() { return author; }
    public void SetBestScore(int bestScore) { this.bestScore = bestScore; }
    public int GetBestScore() { return bestScore; }
    public bool IsUserLevel() { return userLevel; }
    public void SetIsUserLevel(bool userLevel) { this.userLevel = userLevel; }




    //Initialization
    public void init(int sizeX, int sizeY) {
        level = new int[sizeX, sizeY]; objects = new int[sizeX, sizeY]; levelName = ""; difficulty = 1; author = ""; bestScore = -1;
    }
    public void init(int[,] level) {
        this.level = level; objects = new int[level.GetLength(0), level.GetLength(1)]; levelName = ""; difficulty = 1; author = ""; bestScore = -1;
    }
    public void init(int[,] level, int[,] objects) {
        this.level = level; this.objects = objects; levelName = ""; difficulty = 1; author = ""; bestScore = -1;
    }
    public void init(int[,] level, int[,] objects, string levelName, int difficulty, int pack, string author, int bestScore, bool user_level) {
        this.level = level; this.objects = objects; this.levelName = levelName; this.difficulty = difficulty; this.pack = pack; this.author = author; this.bestScore = bestScore; this.userLevel = user_level;
    }

}