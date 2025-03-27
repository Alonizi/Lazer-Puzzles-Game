// Copyright © Abdulaziz Alonizi 2025

using System;
using System.Collections.Generic;
using Abdulaziz_File.Scripts;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Responsible for Spawning/Deleting Items from Grid
/// </summary>
public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private MirrorRotator Mirror; 
    [SerializeField] private LaserSplitter Splitter;
    [SerializeField] private LaserSplitter Splitter_RGB;
    
    [SerializeField] private int Mirrors_MaxCount = 3;
    [SerializeField] private int Splitter_RGB_MaxCount = 3;
    [SerializeField] private int Splitter_MaxCount = 3;

    // Reference to your smoke animation prefab
    [SerializeField] private GameObject SmokePrefab;

    // Sound to play when an item is placed
    [SerializeField] private AudioClip PlaceSound;
    // Volume for the placement sound (0 to 1)
    [SerializeField] private float SoundVolume = 1f;

    // Error feedback sound for wrong placement (on Blockers)
    [SerializeField] private AudioClip ErrorSound;
    [SerializeField] private float ErrorSoundVolume = 1f;

    // Serialized reference for the CameraShake component
    [SerializeField] private CameraShake cameraShake;
    // Shake settings override (update the cameraShake component before triggering shake)
    [SerializeField] private float shakeDurationOverride = 0.5f;
    [SerializeField] private float shakeMagnitudeOverride = 0.1f;
    // Cooldown between error feedback triggers (in seconds)
    [SerializeField] private float shakeCooldown = 1f;
    private float lastErrorFeedbackTime = -Mathf.Infinity;

    private Grid World;
    private Tilemap[] Tiles;
    private Mechanic? SelectedItem;
    private Dictionary<int, Transform> TilesTransforms;
    private Selector ItemSelector;
    private Dictionary<Vector3Int, GameObject> AddedItemsDictionary; 
    
    /// <summary>
    /// Cache/instantiate necessary objects
    /// </summary>
    private void Awake()
    {
        ItemSelector = FindAnyObjectByType<Selector>();
        TilesTransforms = new Dictionary<int, Transform>();
        AddedItemsDictionary = new Dictionary<Vector3Int, GameObject>();
        SelectedItem = null; 
        World = FindAnyObjectByType<Grid>();
        Tiles = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        foreach (var tile in Tiles) 
        {
            // Save each <tilemap,layer> combination in the scene in a Dictionary
            TilesTransforms.Add(tile.gameObject.layer, tile.transform);
        }
    }
    
    /// <summary>
    /// Subscribe to item selected event
    /// </summary>
    private void OnEnable()
    {
        ItemSelector.OnItemSelected += (mechanic) => ItemSelected(mechanic);
    }
    
    /// <summary>
    /// Update item menu buttons
    /// </summary>
    private void Start()
    {
        ItemSelector.UpdateButtons(AddedItemsDictionary.Count, Mirrors_MaxCount, Splitter_MaxCount, Splitter_RGB_MaxCount);
    }
    
    /// <summary>
    /// Check for user input, and spawn items on selected cell
    /// </summary>
    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) // right click (Remove)
        {
            Vector3Int cellPosition = MousePositionToGrid();
            RemoveSelectedItem(cellPosition);
        }
        if (Input.GetMouseButtonDown(0)) // left click (Spawn)
        {
            Vector3Int cellPosition = MousePositionToGrid();
            
            bool noWallExist       = !CheckWallExist(cellPosition);    // Check if no wall was drawn on cell
            bool cellAvailable     = !ItemExistInGrid(cellPosition);   // Check if no item is available on cell
            bool cellWithinBorders = WithinGridBorders(cellPosition);  // Check if cell is within allowed borders
            
            if (SelectedItem == Mechanic.Delete)
            {
                RemoveSelectedItem(cellPosition);
            }
            // Check for valid placement conditions.
            if (SelectedItem is not null && cellAvailable && cellWithinBorders && noWallExist)
            {
                Debug.Log($"Spawning {SelectedItem} Item");
                bool spawned = PerformAction(cellPosition);
                // Only play the effects if an item was successfully spawned.
                if (spawned)
                {
                    PlaySmokeEffect(cellPosition);
                    PlayPlaceSound(cellPosition);
                }
            }
            // Trigger error feedback ONLY if the cell contains a blocker.
            else if (CheckBlockerExist(cellPosition))
            {
                TriggerErrorFeedback(cellPosition);
            }
        }
    }
    
    /// <summary>
    /// Spawn an Item on Grid and return true if the item was placed.
    /// </summary>
    private bool PerformAction(Vector3Int cellPosition)
    {
        bool spawned = false;
        switch (SelectedItem)
        {
            case Mechanic.Mirror: 
                spawned = SpawnMirror(cellPosition);
                break;
            case Mechanic.Splitter:
                spawned = SpawnSplitter(cellPosition, false);
                break;
            case Mechanic.Splitter_RGB:
                spawned = SpawnSplitter(cellPosition, true);
                break;
        }
        return spawned;
    }
    
    /// <summary>
    /// Instantiates the Smoke effect at the same position as the placed item.
    /// </summary>
    private void PlaySmokeEffect(Vector3Int cellPosition)
    {
        if (SmokePrefab != null)
        {
            Vector3 spawnPosition = World.GetCellCenterWorld(cellPosition);
            Instantiate(SmokePrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("SmokePrefab is not assigned in the Inspector!");
        }
    }
    
    /// <summary>
    /// Plays the placement sound at the specified cell position.
    /// </summary>
    private void PlayPlaceSound(Vector3Int cellPosition)
    {
        if (PlaceSound != null)
        {
            Vector3 spawnPosition = World.GetCellCenterWorld(cellPosition);
            AudioSource.PlayClipAtPoint(PlaceSound, spawnPosition, SoundVolume);
        }
        else
        {
            Debug.LogWarning("PlaceSound AudioClip is not assigned in the Inspector!");
        }
    }
    
    /// <summary>
    /// Get mouse position and convert it to Grid Position.
    /// </summary>
    private Vector3Int MousePositionToGrid()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return World.WorldToCell(worldPosition);
    }
    
    /// <summary>
    /// Store the item that was selected by the user.
    /// </summary>
    private void ItemSelected(Mechanic? item)
    {
        SelectedItem = item;
        Debug.LogWarning($"{SelectedItem} Was Selected");
    }
    
    /// <summary>
    /// Check if a GameObject already exists at the specified location.
    /// </summary>
    private bool ItemExistInGrid(Vector3Int cellPosition)
    {
        foreach (Tilemap tile in Tiles)
        {
            // Loop through all tilemaps in the scene.
            foreach (Transform item in tile.transform)
            {
                // Loop through all items in a tilemap.
                if (World.WorldToCell(item.position) == cellPosition)
                {
                    Debug.Log($"Check at Position {World.CellToLocal(cellPosition)} in World");
                    Debug.LogWarning($"Item {item.name} Exists at cell position {World.WorldToCell(item.position)}!");
                    return true;
                }
            }
        }
        return false;
    }
    
    /// <summary>
    /// Checks if the cell contains a GameObject with the tag "Blockers".
    /// </summary>
    private bool CheckBlockerExist(Vector3Int cellPosition)
    {
        foreach (Tilemap tile in Tiles)
        {
            foreach (Transform child in tile.transform)
            {
                if (World.WorldToCell(child.position) == cellPosition && child.CompareTag("Blockers"))
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    /// <summary>
    /// Spawn a Mirror item in the Grid.
    /// Returns true if the item is successfully spawned.
    /// </summary>
    private bool SpawnMirror(Vector3Int cellPosition)
    {
        if (Mirrors_MaxCount > 0)
        {
            Transform parentTile = TilesTransforms[LayerMask.NameToLayer("Mirror")];
            var item = Instantiate(Mirror, World.GetCellCenterWorld(cellPosition),
                Quaternion.Euler(new Vector3(0, 0, 0)), parent: parentTile);
            item.transform.GetChild(0).gameObject.SetActive(false); // deactivate background noise sprite 
            AddedItemsDictionary.Add(cellPosition, item.gameObject);
            Mirrors_MaxCount--;             // Reduce available mirrors count
            ItemSelector.UpdateButtons(AddedItemsDictionary.Count, Mirrors_MaxCount, Splitter_MaxCount, Splitter_RGB_MaxCount);
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Spawn a Splitter or Splitter_RGB item in the Grid.
    /// Returns true if the item is successfully spawned.
    /// </summary>
    private bool SpawnSplitter(Vector3Int cellPosition, bool isRGB)
    {
        Transform parentTile = TilesTransforms[LayerMask.NameToLayer("Splitter")];
        if (isRGB && Splitter_RGB_MaxCount > 0)
        {
            var item = Instantiate(Splitter_RGB, World.GetCellCenterWorld(cellPosition), Quaternion.identity, parent: parentTile);
            AddedItemsDictionary.Add(cellPosition, item.gameObject);
            Splitter_RGB_MaxCount--;
            ItemSelector.UpdateButtons(AddedItemsDictionary.Count, Mirrors_MaxCount, Splitter_MaxCount, Splitter_RGB_MaxCount);
            return true;
        }
        else if (!isRGB && Splitter_MaxCount > 0)
        {
            var item = Instantiate(Splitter, World.GetCellCenterWorld(cellPosition), Quaternion.identity, parent: parentTile);
            AddedItemsDictionary.Add(cellPosition, item.gameObject);
            Splitter_MaxCount--;
            ItemSelector.UpdateButtons(AddedItemsDictionary.Count, Mirrors_MaxCount, Splitter_MaxCount, Splitter_RGB_MaxCount);
            return true;
        }
        return false;
    }
    
    private void RemoveSelectedItem(Vector3Int ItemPositionInGrid)
    {
        try
        {
            var item = AddedItemsDictionary[ItemPositionInGrid];
            Debug.Log($"Delete item {item.tag}");
            switch (item.tag)
            {
                case "Mirror":
                    Mirrors_MaxCount++;
                    break;
                case "Splitter":
                    Splitter_MaxCount++;
                    break;
                case "Splitter_RGB":
                    Splitter_RGB_MaxCount++;
                    break;
            }
            AddedItemsDictionary.Remove(ItemPositionInGrid);
            Destroy(item);
            ItemSelector.UpdateButtons(AddedItemsDictionary.Count, Mirrors_MaxCount, Splitter_MaxCount, Splitter_RGB_MaxCount);
        }
        catch (KeyNotFoundException e)
        {
            Debug.LogWarning("Selected item was not spawned by user");
        }
    }
    
    /// <summary>
    /// Check if cellPosition is within the Grid borders.
    /// </summary>
    private bool WithinGridBorders(Vector3Int cellPosition)
    {
        return (cellPosition.x < 12 && cellPosition.x > -8) &&
               (cellPosition.y > -3 && cellPosition.y < 8);
    }
    
    /// <summary>
    /// Check if a wall or tile was drawn on the cell.
    /// </summary>
    private bool CheckWallExist(Vector3Int cellPosition)
    {
        foreach (var tile in Tiles)
        {
            var wallSprite = tile.GetSprite(cellPosition);
            if (wallSprite != null)
            {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Trigger error feedback (sound and camera shake) when an invalid placement is attempted on a Blockers cell.
    /// </summary>
    private void TriggerErrorFeedback(Vector3Int cellPosition)
    {
        if (Time.time - lastErrorFeedbackTime >= shakeCooldown)
        {
            // Trigger camera shake if a reference is set.
            if (cameraShake != null)
            {
                cameraShake.shakeDuration = shakeDurationOverride;
                cameraShake.shakeMagnitude = shakeMagnitudeOverride;
                cameraShake.TriggerShake();
            }
            else
            {
                Debug.LogWarning("CameraShake reference is not set in the Inspector!");
            }
            // Play error sound at the cell's center position.
            if (ErrorSound != null)
            {
                Vector3 spawnPosition = World.GetCellCenterWorld(cellPosition);
                AudioSource.PlayClipAtPoint(ErrorSound, spawnPosition, ErrorSoundVolume);
            }
            else
            {
                Debug.LogWarning("ErrorSound AudioClip is not assigned in the Inspector!");
            }
            lastErrorFeedbackTime = Time.time;
        }
    }
    
    /// <summary>
    /// Unsubscribe from the item selected event.
    /// </summary>
    private void OnDisable()
    {
        ItemSelector.OnItemSelected -= ItemSelected;
    }
}
