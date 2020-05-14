using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchRightClick : MonoBehaviour
{

    public Sprite[] sprites;

    private SpriteRenderer spriteRenderer;

    private int switchCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer.sprite == null)
        {
            Debug.Log("sprite null detected");
            spriteRenderer.sprite = sprites[0];
        }
        else
        {
            Debug.Log("sprite found detected");
            sprites[0] = spriteRenderer.sprite;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (switchCounter >= 3)
        {
            switchCounter = -1;
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            switchCounter++;
            spriteRenderer.sprite = sprites[switchCounter];
        }
    }
}
