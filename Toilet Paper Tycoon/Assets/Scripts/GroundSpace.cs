using UnityEngine;

public class GroundSpace : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private Color startingColor;
    private Color hoverColor;

    private GameObject currentObject;
    public GameObject[] objects;

    private void Start() {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        startingColor = spriteRenderer.color;
        hoverColor = new Color(0f, 0f, 0f, 0.5f);

        int roll = Random.Range(0, 50);
        if (roll <= 25)
        {
            int rand = Random.Range(0, objects.Length);
            GameObject newtree = Instantiate(objects[rand], transform);
            ChangeCurrentObject(newtree);
        }
        else
        {
            return;
        }
    }

    public void ChangeCurrentObject(GameObject newObject) {
        if (currentObject != null) {
            Destroy(currentObject);
        }
        currentObject = newObject;
    }

    private void OnMouseEnter() {
        spriteRenderer.color = hoverColor;
    }

    private void OnMouseExit() {
        spriteRenderer.color = startingColor;
    }

    private void OnMouseDown() {
        if (GameController.instance.GetSelectedObject() != null) {
            if (currentObject == null) {
                currentObject = Instantiate(GameController.instance.GetSelectedObject(), transform);
            } else {
                ChangeCurrentObject(null);
            }
        }
        GameController.instance.ChangeSelectedSpace(gameObject);
    }
   
}
