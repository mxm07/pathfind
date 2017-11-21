using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PAnim : MonoBehaviour {
    public static IEnumerator anim(int speed, Action<float> animAction, Action afterAction = default(Action)) {
        float t = 0.0f;
        
        while (t < 1.0) {
            t += Time.deltaTime * speed;

            try {
                animAction(Mathf.Clamp(t, 0, 1));
            } catch {}

            yield return null;
        }

        if (afterAction != null)
            afterAction();
    }

    public void rotate(GameObject obj, Quaternion angOffset, Action after = default(Action)) {
        if (obj == null) return;

        Quaternion angCurrent = obj.transform.rotation;
        StartCoroutine(PAnim.anim(5, (t) => obj.transform.rotation = Quaternion.Lerp(angCurrent, angCurrent * angOffset, t), after));
    }  
    public void translate(GameObject obj, Vector3 posOffset, Action after = default(Action)) {
        if (obj == null) return;

        Vector3 posCurrent = obj.transform.position;
        StartCoroutine(PAnim.anim(5, (t) => obj.transform.position = Vector3.Lerp(posCurrent, posCurrent + posOffset, t), after));
    }


    public void arrowTileAnim(GameObject arrowTile, bool active) {
        GameObject left, right;
        if (arrowTile == null || (left = arrowTile.transform.Find("ArrowLeft").gameObject) == null || (right = arrowTile.transform.Find("ArrowRight").gameObject) == null) return;
        
        Color activeCol = new Color(120f/255f, 127f/255f, 178f/255f);
        Color inactiveCol = new Color(111f/255f, 111f/255f, 111f/255f);

        Action<float> transitionColors = delegate (float t) {
            Color col = Color.Lerp(inactiveCol, activeCol, active ? t : 1-t);

            left.GetComponent<Renderer>().material.color = col;
            right.GetComponent<Renderer>().material.color = col;
        };
        StartCoroutine(PAnim.anim(4, transitionColors));
    }
}