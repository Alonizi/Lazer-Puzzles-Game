using System;
using System.Collections.Generic;
using Abdulaziz_File.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class Selector : MonoBehaviour
{
    public static event Action<Mechanic> OnItemSelected;
    private static Mechanic? CurrentSelection;

    public static void SelectMechanic(int mechanic)
    {
        switch ((Mechanic) mechanic)
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

    public void HI(int i)
    {
        
    }
}
