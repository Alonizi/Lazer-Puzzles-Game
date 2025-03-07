using System;
using Abdulaziz_File.Scripts;
using UnityEditor.Tilemaps;
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
    }
    
    private void OnEnable()
    {
        Selector.OnItemSelected += SelectItem;
    }
    
    /// <summary>
    /// check for user input , and spawn items accordingly
    /// </summary>
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            ItemExistInGrid(GetCurrentPositionToGrid());
            if (SelectedItem is not null )
            {
                SpawnItem();
            }
            Debug.Log("Pressed Left Click");
            Debug.Log($"Grid Position {GetCurrentPositionToGrid()}");
        }
    }

    /// <summary>
    /// Spawn Item on Grid
    /// </summary>
    private void SpawnItem()
    {
        switch (SelectedItem)
        {
            case Mechanic.Mirror: 
                break;
            case Mechanic.Splitter:
                break;
            case Mechanic.Splitter_RGB:
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
    private Vector3Int GetCurrentPositionToGrid()
    {
        var worldposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return World.WorldToCell(worldposition);
    }

    /// <summary>
    /// Store the item that was selected by the user 
    /// </summary>
    /// <param name="item"></param>
    private void SelectItem(Mechanic item)
    {
        SelectedItem = item; 
        Debug.LogWarning($"{item} Was Selected");
    }

    /// <summary>
    /// Check if a GameObject already exist at specified location
    /// </summary>
    /// <param name="gridPosition"> cell position </param>
    /// <returns> true if item exit , false otherwise</returns>
    private bool ItemExistInGrid(Vector3Int gridPosition)
    {
        bool itemExist = false; 
        var tiles = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        foreach (Tilemap tile in tiles)
        {
            foreach (Transform item in tile.transform)
            { 
                itemExist = World.WorldToCell(item.position) == gridPosition ;
            }
        }
        return itemExist;
    }
    
    private void OnDisable()
    {
        Selector.OnItemSelected -= SelectItem;
    }
}
