using UnityEngine;
using System.Collections;

public class PrefabScript : MonoBehaviour {
    public GameObject[] prefabs;

    void Start() {
        DontDestroyOnLoad(gameObject);
    }
}
