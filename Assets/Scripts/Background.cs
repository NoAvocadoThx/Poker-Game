using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    /// <summary>
    /// Awake function called by Unity
    /// </summary>
    void Awake()
    {
        //scale the background according to the size of the screen
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;
        float width = sr.sprite.bounds.size.x;
        float height = sr.sprite.bounds.size.y;
        sr.size = new Vector2(Screen.width / (2*width), Screen.height /(2* height));
       
    }
}
