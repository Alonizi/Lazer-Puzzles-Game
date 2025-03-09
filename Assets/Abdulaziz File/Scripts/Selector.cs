using System;
using System.Collections.Generic;
using Abdulaziz_File.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{
    [SerializeField] private Button SplitterRgbButton;
    [SerializeField] private Button SplitterButton;
    [SerializeField] private Button MirrorButton;
    public event Action<Mechanic?> OnItemSelected;
    private Mechanic? CurrentSelection;

    public void SelectMechanic(int mechanic)
    {
        switch ((Mechanic) mechanic)
        {
            case Mechanic.Mirror:
                CurrentSelection = Mechanic.Mirror;
                break;
            case Mechanic.Splitter:
                CurrentSelection = Mechanic.Splitter;
                break;
            case Mechanic.Splitter_RGB:
                CurrentSelection = Mechanic.Splitter_RGB;
                break; 
            case Mechanic.Rotator:
                CurrentSelection = Mechanic.Rotator;
                break; 
            case Mechanic.Delete:
                CurrentSelection = Mechanic.Delete;
                break; 
        }
        OnItemSelected(CurrentSelection);
    }

    public void UpdateButtons(int mirrorsCount , int splittersCount , int splittersRgbCount)
    {
        MirrorButton.transform.GetChild(0).GetComponent<Text>().text = $"Mirrors ({mirrorsCount})";
        MirrorButton.enabled = mirrorsCount > 0 ;
        
        SplitterButton.transform.GetChild(0).GetComponent<Text>().text = $"Splitters ({splittersCount})";
        SplitterButton.enabled = splittersCount > 0;
        
        SplitterRgbButton.transform.GetChild(0).GetComponent<Text>().text = $"Splitters_RGB ({splittersRgbCount})";
        SplitterRgbButton.enabled = splittersRgbCount > 0; 
    }
    
}
