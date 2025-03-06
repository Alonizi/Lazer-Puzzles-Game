using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class Selector : MonoBehaviour
{
    private LaserSplitter SplitterPrefab;
    private LaserSplitter SplitterRgbPrefab;
    private MirrorRotator MirrorPrefab; 
    private Grid world;
    private Mechanic? CurrentSelection;
    private Stack<GameObject> SpawnedItems; 
    public enum Mechanic
    {
        Mirror,
        Splitter_RGB,
        Splitter,
        Rotator,
        Delete,
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentSelection = null; 
        world = FindAnyObjectByType<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (CurrentSelection is not null)
            {
                //Instantiate()
            }
            Debug.Log("Pressed Left Click");
            Debug.Log($"Grid Position {CurrentPositionToGrid()}");
        }
    }

    private Vector3Int CurrentPositionToGrid()
    {
        var worldposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return world.WorldToCell(worldposition);
    }

    public void SelectMechanic(Mechanic mechanic)
    {
        switch (mechanic)
        {
            case Mechanic.Mirror:
                CurrentSelection = Mechanic.Mirror;
                Debug.Log("Mirror Selected");
                break;
            case Mechanic.Splitter:
                CurrentSelection = Mechanic.Splitter;
                Debug.Log("Splitter Selected");
                break;
            case Mechanic.Splitter_RGB:
                CurrentSelection = Mechanic.Splitter_RGB;
                Debug.Log("Splitter_RGB Selected");
                break; 
            case Mechanic.Rotator:
                CurrentSelection = Mechanic.Rotator;
                Debug.Log("Rotator Selected");
                break; 
            case Mechanic.Delete:
                CurrentSelection = Mechanic.Delete;
                Debug.Log("Delete Selected");
                break; 
        }
    }
}
