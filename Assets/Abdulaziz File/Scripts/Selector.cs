// Copyrights Abdulaziz Alonizi 2025

using System;
using Abdulaziz_File.Scripts;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Responsible for Selecting Mechanics from the Items Menu 
/// </summary>
public class Selector : MonoBehaviour
{
    [SerializeField] private Button SplitterRgbButton;
    [SerializeField] private Button SplitterButton;
    [SerializeField] private Button MirrorButton;
    public event Action<Mechanic?> OnItemSelected;
    private Mechanic? CurrentSelection;

    /// <summary>
    /// identify and cache the selected mechanic , and trigger an event signaling an item was selected 
    /// </summary>
    /// <param name="mechanic"></param>
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

    /// <summary>
    /// update menu item buttons to reflect available items and their count
    /// </summary>
    /// <param name="mirrorsCount"> number of available mirrors </param>
    /// <param name="splittersCount"> number of available Splitters</param>
    /// <param name="splittersRgbCount"> number of available Splitter_RGB</param>
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
