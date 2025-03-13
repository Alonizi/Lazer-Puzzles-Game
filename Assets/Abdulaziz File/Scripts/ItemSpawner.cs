//copyrights Abdulaziz Alonizi 2025

using System.Collections.Generic;
using Abdulaziz_File.Scripts;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Responsible for Spawning/Deleting Items from Grid
/// </summary>
public class ItemSpawner : MonoBehaviour
{
    [SerializeField]private MirrorRotator Mirror; 
    [SerializeField]private LaserSplitter Splitter;
    [SerializeField]private LaserSplitter Splitter_RGB;
    
    [SerializeField]private int Mirrors_MaxCount = 3;
    [SerializeField]private int Splitter_RGB_MaxCount = 3 ;
    [SerializeField]private int Splitter_MaxCount = 3 ;
    
    private Grid World;
    private Tilemap[] Tiles;
    private Mechanic? SelectedItem;
    private Dictionary<int, Transform> TilesTransforms;
    private Stack<GameObject> AddedItems;
    private Selector ItemSelector; 
    
    /// <summary>
    /// cache/instantiate necessary objects
    /// </summary>
    private void Awake()
    {
        ItemSelector = FindAnyObjectByType<Selector>();
        AddedItems = new Stack<GameObject>();
        TilesTransforms = new Dictionary<int, Transform>();
        SelectedItem = null; 
        World = FindAnyObjectByType<Grid>();
        Tiles = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        foreach (var tile in Tiles) 
        {
            // save each <tilemap,layer> combination in the scene in a Dictionary
            TilesTransforms.Add(tile.gameObject.layer,tile.transform);
        }
    }
    /// <summary>
    /// subscribe to item selected event 
    /// </summary>
    private void OnEnable()
    {
        ItemSelector.OnItemSelected += (mechanic) => ItemSelected(mechanic);
    }
    /// <summary>
    /// update Item menu buttons 
    /// </summary>
    private void Start()
    {
        ItemSelector.UpdateButtons(AddedItems.Count,Mirrors_MaxCount,Splitter_MaxCount,Splitter_RGB_MaxCount);
    }

    /// <summary>
    /// check for user input , and spawn items on selected cell
    /// </summary>
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int cellPosition = MousePositionToGrid();
            
            bool noWallExist = !CheckWallExist(cellPosition); // check if no wall was drawn on cell
            bool cellAvailable = !ItemExistInGrid(cellPosition); // check if no items is availble on cell
            bool cellWithinBorders = WithinGridBorders(cellPosition); // cehck if cell is within allowed borders
            
            if (SelectedItem is not null && cellAvailable && cellWithinBorders && noWallExist)
            {
                Debug.Log($"Spawning {SelectedItem} Item");
                PerformAction(cellPosition);
            }
        }
    }

    /// <summary>
    /// Spawn an Item on Grid
    /// </summary>
    private void PerformAction(Vector3Int cellPosition)
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
    /// or delete last item
    /// </summary>
    /// <param name="item"></param>
    private void ItemSelected(Mechanic? item)
    {
        SelectedItem = item; 
        Debug.LogWarning($"{SelectedItem} Was Selected");

        if (SelectedItem == Mechanic.Delete && AddedItems.Count > 0)
        {
            DeleteItem();
        }
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
            // loop through all tilemaps in scene
            foreach (Transform item in tile.transform)
            {
                // loop through all items in a tilemap
                itemExist = World.WorldToCell(item.position) == cellPosition;
                if (itemExist)
                {
                    Debug.Log($"Check at Position {World.CellToLocal(cellPosition)} in World");
                    Debug.LogWarning(
                        $"Item {item.name} Exists at cell position {World.WorldToCell(item.position)} !");
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// spawn a mirror Item in Grid
    /// </summary>
    /// <param name="cellPosition"> cell at which to spawn the mirror</param>
    private void SpawnMirror(Vector3Int cellPosition)
    {
        if (Mirrors_MaxCount > 0)
        {
            Transform parentTile = TilesTransforms[LayerMask.NameToLayer("Mirror")];
            var item = Instantiate(Mirror, World.GetCellCenterWorld(cellPosition),
                Quaternion.Euler(new Vector3(0, 0, 0)), parent: parentTile);
            AddedItems.Push(item.gameObject); // keep track of added items
            Mirrors_MaxCount--; // reduce available mirrors count 
            ItemSelector.UpdateButtons(AddedItems.Count,Mirrors_MaxCount,Splitter_MaxCount,Splitter_RGB_MaxCount); // update buttons text
        }
    }
    
    /// <summary>
    /// spawn a Splitter/Splitter_RgGB Item in Grid
    /// </summary>
    /// <param name="cellPosition"> cell position at which to spawn the item</param>
    /// <param name="isRGB"> is the splitter required of type RGB </param>
    private void SpawnSplitter(Vector3Int cellPosition , bool isRGB)
    {
        Transform parentTile = TilesTransforms[LayerMask.NameToLayer("Splitter")];
        if (isRGB && Splitter_RGB_MaxCount > 0) // spawn RGB splitter
        {
            var item = Instantiate(Splitter_RGB, World.GetCellCenterWorld(cellPosition), Quaternion.identity, parent: parentTile);
            AddedItems.Push(item.gameObject);
            Splitter_RGB_MaxCount--;
        }
        else if (!isRGB && Splitter_MaxCount > 0) // spawn non-rgb Splitter
        {
            var item = Instantiate(Splitter, World.GetCellCenterWorld(cellPosition), Quaternion.identity, parent: parentTile);
            AddedItems.Push(item.gameObject);
            Splitter_MaxCount--;
        }
        ItemSelector.UpdateButtons(AddedItems.Count,Mirrors_MaxCount,Splitter_MaxCount,Splitter_RGB_MaxCount);
    }

    /// <summary>
    /// delete last item added by the user and update items count accordingly
    /// </summary>
    private void DeleteItem()
    { 
        var item = AddedItems.Pop();
        Debug.Log($"Delete item {item.tag}");
       switch (item.tag)
       {
           case "Mirror":
               Mirrors_MaxCount++;
               break;
           case "Splitter":
               Splitter_MaxCount++;
               break;
           case "Splitter_RGB" :
               Splitter_RGB_MaxCount++;
               break;
       }
       Destroy(item);
       ItemSelector.UpdateButtons(AddedItems.Count,Mirrors_MaxCount,Splitter_MaxCount,Splitter_RGB_MaxCount);
    }
    
    /// <summary>
    /// check if cellPosition is withing Grid borders
    /// </summary>
    /// <param name="cellPosition"> cell position to check</param>
    /// <returns></returns>
    private bool WithinGridBorders(Vector3Int cellPosition)
    {
        if ((cellPosition.x < 12 && cellPosition.x > -8) && (cellPosition.y > -3 && cellPosition.y < 8))
        {
            return true;
        }
        return false; 
    }
    
    /// <summary>
    /// check if a wall or other was drawn on the cell 
    /// </summary>
    /// <param name="cellPosition">cellposition to check </param>
    /// <returns></returns>
    private bool CheckWallExist(Vector3Int cellPosition)
    {
        foreach (var tile in Tiles)
        {
            var wallSprite = tile.GetSprite(cellPosition);
            if (wallSprite is not null)
            {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// unsubscribe for item selected event 
    /// </summary>
    private void OnDisable()
    {
        ItemSelector.OnItemSelected -= ItemSelected;
    }
}
