using System;
using Abdulaziz_File.Scripts;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    private Grid World;
    private Mechanic? SelectedItem; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created

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
            if (SelectedItem is not null)
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

    private void OnDisable()
    {
        Selector.OnItemSelected -= SelectItem;

    }
}
