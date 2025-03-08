//copyrights Abdulaziz Alonizi 2025

using System;
using System.Collections.Generic;
using Abdulaziz_File.Scripts;
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
    private Dictionary<int, Transform> TilesTransforms;
    private Stack<GameObject> AddedItems; 
    
    private void Awake()
    {
        AddedItems = new Stack<GameObject>();
        TilesTransforms = new Dictionary<int, Transform>();
        SelectedItem = null; 
        World = FindAnyObjectByType<Grid>();
        Tiles = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        foreach (var tile in Tiles)
        {
            TilesTransforms.Add(tile.gameObject.layer,tile.transform);
        }
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
                PerformAction(cellPosition);
            }
        }
    }

    /// <summary>
    /// Spawn Item on Grid
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
    /// </summary>
    /// <param name="item"></param>
    private void SelectItem(Mechanic? item)
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
            foreach (Transform item in tile.transform)
            { 
                itemExist = World.WorldToCell(item.position) == cellPosition ;
                if (itemExist)
                {
                    Debug.Log($"Check at Position {World.CellToLocal(cellPosition)} in World");
                    Debug.LogWarning($"Item {item.name} Exists at cell position {World.WorldToCell(item.position)} !");
                    return true;
                }
            }
        }
        return false;
    }

    private void SpawnMirror(Vector3Int cellPosition)
    {
        Transform parentTile = TilesTransforms[LayerMask.NameToLayer("Mirror")];
        var item = Instantiate(Mirror, World.GetCellCenterWorld(cellPosition), Quaternion.Euler(new Vector3(0,0,45)),parent:parentTile);
        AddedItems.Push(item.gameObject);
    }
    
    private void SpawnSplitter(Vector3Int cellPosition , bool isRGB)
    {
        Transform parentTile = TilesTransforms[LayerMask.NameToLayer("Splitter")];

        if (isRGB)
        {
            var item = Instantiate(Splitter_RGB, World.GetCellCenterWorld(cellPosition), Quaternion.identity, parent: parentTile);
            AddedItems.Push(item.gameObject);
        }
        else
        {
            var item = Instantiate(Splitter, World.GetCellCenterWorld(cellPosition), Quaternion.identity, parent: parentTile);
            AddedItems.Push(item.gameObject);
        }
    }

    private void DeleteItem()
    {
        Destroy(AddedItems.Pop());
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
