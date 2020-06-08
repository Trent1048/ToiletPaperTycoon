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
    protected List<AddTiles> additionalTiles;

    public bool deleted = false;
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

        if (additionalTiles == null)
        {
            additionalTiles = new List<AddTiles>();
        }
        additionalTiles.Add(this);

        gridLayout = transform.parent.GetComponentInParent<GridLayout>();
        cellPosition = gridLayout.WorldToCell(transform.position);
    }

    private void OnDestroy()
    {
        additionalTiles.Remove(this);
    }
    private void OnMouseExit()
    {
        spriteRenderer.color = startingColor;
    }

    private void OnMouseEnter()
    {
        spriteRenderer.color = hoverColor;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            spriteRenderer.color = startingColor;
            MakeGroundTiles();
            Destroy(gameObject);
        }
    }

    private void MakeGroundTiles()
    {

        int cellX = cellPosition.x;
        Vector3Int saveCellPos = cellPosition;
        
        for(int i=0; i<4; i++)
        {
            Vector3Int checkPos = cellPosition + directionalCheck[i];
            Vector3 worldPos= gridLayout.CellToWorld(checkPos) + new Vector3(0f, 0.25f, 0f);
            if (!currentMap.HasTile(checkPos))
            {
                Instantiate(this, worldPos, new Quaternion(0, 0, 0, 0), transform.parent);
            }
        }

        foreach (TileBase tile in tileSetupArray)
        {
            if(cellPosition.x > cellX+9)
            {
                cellPosition.x = cellX;
                cellPosition.y--;
            }

            currentMap.SetTile(cellPosition, tile);
            cellPosition.x++;

            Vector3 worldPos = gridLayout.CellToWorld(cellPosition) + new Vector3(-0.5f, 0f, 0f);
            Instantiate(groundTile, worldPos, new Quaternion(0,0,0,0), transform.parent.parent.GetChild(0));
        }

        cellPosition = saveCellPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentMap.HasTile(cellPosition))
        {
            Destroy(gameObject);
        }
    }
}
