using System;
using System.Collections.Generic;
using Abdulaziz_File.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class Selector : MonoBehaviour
{
    public static event Action<Mechanic?> OnItemSelected;
    private static Mechanic? CurrentSelection;

    public static void SelectMechanic(int mechanic)
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
    
}
