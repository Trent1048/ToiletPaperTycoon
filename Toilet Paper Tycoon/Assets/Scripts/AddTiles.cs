using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AddTiles : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color startingColor;
    private Color hoverColor;

    public Tilemap currentMap;
    public TileBase dirtTile;
    public TileBase grassTile;

    protected static TileBase[] tileSetupArray;

    public GameObject groundTile;

    private Vector3Int cellPosition;
    private GridLayout gridLayout;

    private Dictionary<int, Vector3Int> directionalCheck = new Dictionary<int, Vector3Int>
    {
        {0, new Vector3Int(-10,0,0)},
        {1, new Vector3Int(0,10,0)},
        {2, new Vector3Int(10,0,0)},
        {3, new Vector3Int(0,-10,0)}
    };

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        startingColor = spriteRenderer.color;
        hoverColor = new Color(0f, 0f, 0f, 0.5f);
        
        //saves the original map design to be used and copied
        if (tileSetupArray == null)
        {
            tileSetupArray = new TileBase[100];
            for(int i=0; i<tileSetupArray.Length; i++)
            {
                if (i < 10 || i%10 == 0 || (i - 9) % 10 == 0 || i > 89)
                {
                    tileSetupArray[i] = dirtTile;
                }else
                {
                    tileSetupArray[i] = grassTile;
                }
            }
        }

        if (currentMap == null)
        {
            currentMap = transform.parent.parent.GetComponent<Tilemap>();
        }
        
        gridLayout = transform.parent.GetComponentInParent<GridLayout>();
        cellPosition = gridLayout.WorldToCell(transform.position);
    }

    private void OnMouseExit()
    {
        spriteRenderer.color = startingColor;
    }

    private void OnMouseEnter()
    {
        if (!GameController.instance.GameIsPaused())
        {
            spriteRenderer.color = hoverColor;
        }
    }

    private void OnMouseOver()
    {
        //do only if game is not paused
        if (!GameController.instance.GameIsPaused())
        {
            if (Input.GetMouseButtonDown(0))
            {
                spriteRenderer.color = startingColor;
                MakeGroundTiles();
                Destroy(gameObject);
            }
        }
    }

    /*fills in a 10x10 tile area with pre-made tileSetupArray
    and place a groundspace on top of each one*/
    private void MakeGroundTiles()
    {
        //create this gameobject around this to allow more expansion of map
        for(int i=0; i<4; i++)
        {
            Vector3Int checkPos = cellPosition + directionalCheck[i];
            Vector3 worldPos= gridLayout.CellToWorld(checkPos) + new Vector3(0f, 0.254f, 0f);
            if (!currentMap.HasTile(checkPos)) //if there is a 10x10 tile area, don't do this
            {
                Instantiate(this, worldPos, new Quaternion(0, 0, 0, 0), transform.parent);
            }
        }
        Vector3Int tempCellPos = cellPosition + new Vector3Int(-4,5,0); //new var, so cellPosition does not get altered
        int cellX = tempCellPos.x; //to reset x-coordinate after doing a for loop 10 times

        //loop to fill 10x10 area
        foreach (TileBase tile in tileSetupArray)
        {
            if(tempCellPos.x > cellX+9)
            {
                tempCellPos.x = cellX;
                tempCellPos.y--;
            }

            currentMap.SetTile(tempCellPos, tile);
            Vector3 worldPos = gridLayout.CellToWorld(tempCellPos) + new Vector3(0f,0.25f,0f);

            tempCellPos.x++;

            Instantiate(groundTile, worldPos, new Quaternion(0,0,0,0), transform.parent.parent.GetChild(0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if current position has a tile, destroy 'this'
        if (currentMap.HasTile(cellPosition))
        {
            Destroy(gameObject);
        }
    }
}
