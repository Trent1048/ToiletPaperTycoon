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

        // initial tree generation
        if (Random.Range(0, 3) == 0) {
            int treeType = Random.Range(0, objects.Length);
            ChangeCurrentObject(objects[treeType]);
        }
    }

    public void ChangeCurrentObject(GameObject newObject) {
        if (currentObject != null) {
            Destroy(currentObject);
        }
        if (newObject != null) {
            currentObject = Instantiate(newObject, transform);
        } else {
            currentObject = null;
        }
    }

    public GameObject GetCurrentObject() {
        return currentObject;
    }

    private void OnMouseEnter() {
        if (!GameController.instance.GameIsPaused()) {
            spriteRenderer.color = hoverColor;
        }
    }

    private void OnMouseExit() {
        spriteRenderer.color = startingColor;
    }

    private void OnMouseDown() {
        if (!GameController.instance.GameIsPaused()) {
            if (GameController.instance.GetSelectedObject() != null) {
                if (currentObject == null) {
                    ChangeCurrentObject(GameController.instance.GetSelectedObject());
                } else {
                    ChangeCurrentObject(null);
                }
            }
            GameController.instance.ChangeSelectedSpace(gameObject);
        }
    }
   
}
