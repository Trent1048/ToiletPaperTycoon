using UnityEngine;

public class GroundController : MonoBehaviour {

    private GameObject[,] grid;
    private const int GRID_SIZE = 10;

    private void Start() {
        grid = new GameObject[GRID_SIZE, GRID_SIZE];
    }

    public void Add(int x, int y, GameObject gameObject) {
        // make sure the spot is empty and is within the grid
        if (x < GRID_SIZE && y < GRID_SIZE && grid[x, y] == null) {
            grid[x, y] = gameObject;

            // place the game object in the corresponding location
            float objectX = 0f;
            float objectY = -2.25f;

            // factor in x location
            objectX += 0.5f * x;
            objectY += 0.25f * x;

            // factor in y location
            objectX -= 0.5f * y;
            objectY += 0.25f * y;

            gameObject.transform.position = new Vector2(objectX, objectY);
        } else {
            Debug.Log("invalid location");
        }
    }

}
