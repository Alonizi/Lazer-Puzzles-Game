//copyrights Abdulaziz Alonizi 2025
using Abdulaziz_File.Scripts;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField]private MirrorRotator Mirror; 
    [SerializeField]private LaserSplitter Splitter;
    [SerializeField]private LaserSplitter Splitter_RGB;
    
    [SerializeField] private int Mirrors_MaxCount;
    [SerializeField] private int Splitter_RGB_MaxCount;
    [SerializeField] private int Splitter_MaxCount;
    
    private Grid World;
    private Tilemap[] Tiles;
    private Mechanic? SelectedItem;
    
    private void Awake()
    {
        SelectedItem = null; 
        World = FindAnyObjectByType<Grid>();
        Tiles = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
    }
    
    private void OnEnable()
    {
        Selector.OnItemSelected += (mechanic) => SelectItem(mechanic);
    }
    
    /// <summary>
    /// check for user input , and spawn items accordingly
    /// </summary>
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int cellPosition = MousePositionToGrid();
            bool cellAvailable = !ItemExistInGrid(cellPosition);
            bool cellWithinBorders = WithinGridBorders(cellPosition);
            
            if (SelectedItem is not null && cellAvailable && cellWithinBorders)
            {
                Debug.Log($"Spawning {SelectedItem} Item");
                SpawnItem(cellPosition);
                //SelectedItem = null;
            }
        }
    }

    /// <summary>
    /// Spawn Item on Grid
    /// </summary>
    private void SpawnItem(Vector3Int cellPosition)
    {
        switch (SelectedItem)
        {
            case Mechanic.Mirror: 
                SpawnMirror(cellPosition);
                break;
            case Mechanic.Splitter:
                SpawnSplitter(cellPosition,false);
                break;
            case Mechanic.Splitter_RGB:
                SpawnSplitter(cellPosition,true);
                break; 
            case Mechanic.Rotator:
                break; 
            case Mechanic.Delete:
                break; 
        }
    }
    /// <summary>
    /// Get mouse position and convert it to Grid Position
    /// </summary>
    /// <returns></returns>
    private Vector3Int MousePositionToGrid()
    {
        var worldposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return World.WorldToCell(worldposition);
    }

    /// <summary>
    /// Store the item that was selected by the user 
    /// </summary>
    /// <param name="item"></param>
    private void SelectItem(Mechanic? item)
    {
        SelectedItem = item; 
        Debug.LogWarning($"{SelectedItem} Was Selected");
    }

    /// <summary>
    /// Check if a GameObject already exist at specified location
    /// </summary>
    /// <param name="gridPosition"> cell position </param>
    /// <returns> true if item exit , false otherwise</returns>
    private bool ItemExistInGrid(Vector3Int cellPosition)
    {
        bool itemExist = false; 
        foreach (Tilemap tile in Tiles)
        {
            foreach (Transform item in tile.transform)
            { 
                itemExist = World.WorldToCell(item.position) == cellPosition ;
                if (itemExist)
                {
                    Debug.Log($"Check at Position {World.CellToLocal(cellPosition)} in World");
                    Debug.LogError($"Item {item.name} Exists at cell position {World.WorldToCell(item.position)} !");
                    return true;
                }
            }
        }
        return false;
    }

    private void SpawnMirror(Vector3Int cellPosition)
    {
        Transform spawnTile = null;
        foreach (var tile in Tiles)
        {
            if (tile.gameObject.layer == LayerMask.NameToLayer("Mirror"))
            {
                spawnTile = tile.transform; 
            }
        }
        Instantiate(Mirror, World.GetCellCenterWorld(cellPosition), Quaternion.Euler(new Vector3(0,0,45)),parent:spawnTile);
    }
    
    private void SpawnSplitter(Vector3Int cellPosition , bool isRGB)
    {
        Transform spawnTile = null;
        foreach (var tile in Tiles)
        {
            if (tile.gameObject.layer == LayerMask.NameToLayer("Splitter"))
            {
                spawnTile = tile.transform; 
            }
        }
        if (isRGB)
        {
            Instantiate(Splitter_RGB, World.GetCellCenterWorld(cellPosition), Quaternion.identity, parent: spawnTile);
        }
        else
        {
            Instantiate(Splitter, World.GetCellCenterWorld(cellPosition), Quaternion.identity, parent: spawnTile);
        }
    }
    
    private bool WithinGridBorders(Vector3Int cellPosition)
    {
        if ((cellPosition.x < 12 && cellPosition.x > -8) && (cellPosition.y > -3 && cellPosition.y < 8))
        {
            return true;
        }
        return false; 
    }
    
    private void OnDisable()
    {
        Selector.OnItemSelected -= SelectItem;
    }
}
