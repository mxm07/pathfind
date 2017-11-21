using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Enums;

public class Main : MonoBehaviour {
    public GameObject tile;
    public GameObject player;

    protected Dictionary<string, object> settings;
    protected PLevel[] levels;
    protected int currentLevel = 0;
    protected Coord playerPos;

	// Use this for initialization
	void Start () {
        PUtility.loadFiles(out settings, out levels);

        generateLevel(levels[currentLevel], new Vector3());
    }


    public void generateLevel(PLevel L, Vector3 startPosition) {
        L.startTile = null;

        L.tileReferences = new PTile[L.level.GetLength(0), L.level.GetLength(1)];
        L.objectReferences = new PObject[L.level.GetLength(0), L.level.GetLength(1)];

        for (int i = 0; i < L.level.GetLength(0); i++) {
            for (int j = 0; j < L.level.GetLength(1); j++) {
                int currentTile = L.level[i, j];
                int currentObject = L.objects[i, j];

                if (currentTile == 0) continue; //Don't instantiate any tiles if empty

                PTile tile = PUtility.ntile(currentTile, startPosition + new Vector3(i, 0, j));
                L.tileReferences[i, j] = tile;
                
                //UV mapping
                Mesh mesh = tile.getTile().GetComponent<MeshFilter>().mesh;
                Vector2[] uvs = new Vector2[mesh.uv.Length];
                uvs[4] = new Vector2(0.0f, 1.0f);
                uvs[5] = new Vector2(1.0f, 1.0f);
                uvs[8] = new Vector2(0.0f, 0.0f);
                uvs[9] = new Vector2(1.0f, 0.0f);

                mesh.uv = uvs;

                //Set start tile, throw exception if there are more than one
                if (currentTile == 2) {
                    if (L.startTile == null) {
                        L.startTile = tile;
                        playerPos = new Coord(i, j);
                    } else
                        throw new System.Exception("Multiple start tiles provided");
                }

                PObject obj = PUtility.nobj(currentObject, tile);
                L.objectReferences[i, j] = obj;
            }
        }

        player.transform.position = L.startTile.getTile().transform.position + new Vector3(0,0.35f,0);
    }

    public void destroyLevel(PLevel L) {
        if (L.tileReferences == null || L.objectReferences == null)
            return;

        for (int i = 0; i < L.tileReferences.Length; i++) {
            for (int j = 0; j < L.objectReferences.Length; j++) {
                Destroy(L.objectReferences[i, j]);
                Destroy(L.tileReferences[i, j]);

                L.tileReferences[i, j] = null;
                L.objectReferences[i, j] = null;
            }
        }
    }

    bool animPlaying = false;
    void playerMove(Dir dir, bool preMove=true) {
        if (player == null) return;

        Vector3 dirVec = PUtility.dirToVec(dir);
        Quaternion dirAng = PUtility.dirToAng(dir);
        Coord newPos = new Coord(playerPos.x + (int)dirVec.x, playerPos.y + (int)dirVec.z);
        PLevel level = levels[currentLevel];

        ETile currentTile = (ETile)level.level[playerPos.x, playerPos.y];
        
        PAnim anim = GetComponent<PAnim>(); //anim object

        
        if (preMove) {
            //pre-move 
            if (newPos.x < 0 || newPos.y < 0 || newPos.x >= level.level.GetLength(0) || newPos.y >= level.level.GetLength(1))
                return;

            if (animPlaying) return;
            animPlaying = true;

            anim.rotate(player, dirAng); //Rotate the player
            //anim.translate(player, dirVec, () => playerMove(dir, false)); //Move the player

            //We have to implement a custom translate animation here to add a slight y offset so the bottom corner of the cube doesn't clip the level
            Vector3 startPos = player.transform.position;
            StartCoroutine(PAnim.anim(5, (t) => player.transform.position = Vector3.Lerp(startPos, startPos + dirVec, t) + new Vector3(0,Mathf.Sin(t*Mathf.PI)*0.08f,0), () => playerMove(dir, false)));

            GetComponent<CameraScript>().updatePos(dirVec); //Move the camera
        } else {
            animPlaying = false;
            
            if (currentTile == ETile.ArrowUp || currentTile == ETile.ArrowRight || currentTile == ETile.ArrowDown || currentTile == ETile.ArrowLeft)
                anim.arrowTileAnim(level.tileReferences[playerPos.x, playerPos.y].getTile(), false);

            //post-move
            player.transform.rotation = Quaternion.identity;
            playerPos = newPos;

            ETile nextTile = (ETile)level.level[newPos.x, newPos.y];


            switch (nextTile) {
                case ETile.ArrowUp: {
                    playerMove(Dir.Up);
                    anim.arrowTileAnim(level.tileReferences[newPos.x, newPos.y].getTile(), true);
                    break;
                }
                case ETile.ArrowRight: {
                    playerMove(Dir.Right);
                    anim.arrowTileAnim(level.tileReferences[newPos.x, newPos.y].getTile(), true);
                    break;
                }
                case ETile.ArrowDown: {
                    playerMove(Dir.Down);
                    anim.arrowTileAnim(level.tileReferences[newPos.x, newPos.y].getTile(), true);
                    break;
                }
                case ETile.ArrowLeft: {
                    playerMove(Dir.Left);
                    anim.arrowTileAnim(level.tileReferences[newPos.x, newPos.y].getTile(), true);
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update() {
        Dictionary<KeyCode, Dir> wasd = new Dictionary<KeyCode, Dir>() {{KeyCode.W, Dir.Up}, {KeyCode.A, Dir.Left}, {KeyCode.S, Dir.Down}, {KeyCode.D, Dir.Right}};

        foreach (KeyValuePair<KeyCode, Dir> entry in wasd) {
            if (Input.GetKey(entry.Key))
                playerMove(entry.Value);
        }
	}
}
