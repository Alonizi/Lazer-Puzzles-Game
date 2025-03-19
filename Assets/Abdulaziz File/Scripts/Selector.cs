// Copyrights Abdulaziz Alonizi 2025

using System;
using Abdulaziz_File.Scripts;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Responsible for Selecting Mechanics from the Items Menu 
/// </summary>
public class Selector : MonoBehaviour
{
    [SerializeField] private Button SplitterRgbButton;
    [SerializeField] private Button SplitterButton;
    [SerializeField] private Button MirrorButton;
    [SerializeField] private Button UndoButton;
    public event Action<Mechanic?> OnItemSelected;
    private Mechanic? CurrentSelection;

    private void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            MirrorButton.Select();
            CurrentSelection = Mechanic.Mirror;
            OnItemSelected(CurrentSelection);
        }

        if (Input.GetKeyDown("2"))
        {
            SplitterButton.Select();
            CurrentSelection = Mechanic.Splitter;
            OnItemSelected(CurrentSelection);
        }
        if (Input.GetKeyDown("3"))
        {
            SplitterRgbButton.Select();
            CurrentSelection = Mechanic.Splitter_RGB;
            OnItemSelected(CurrentSelection);
        }
    }

    /// <summary>
    /// identify and cache the selected mechanic , and trigger an event signaling an item was selected 
    /// </summary>
    /// <param name="mechanic"></param>
    public void SelectMechanic(int mechanic)
    {
        switch ((Mechanic)mechanic)
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
    /// <param name="undoStackCounter"> number of Items spawned by user</param>

    public void UpdateButtons(int undoStackCounter,int mirrorsCount, int splittersCount, int splittersRgbCount)
    {
        if (undoStackCounter <= 0)
        {
            UndoButton.interactable = false; 
        }
        else
        {
            UndoButton.interactable = true; 
        }
        if (mirrorsCount <= 0)
        {
            MirrorButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            MirrorButton.interactable = false;
        }
        else
        {
            MirrorButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Mathf.Max(mirrorsCount, 1).ToString();
            MirrorButton.interactable = true;
        }

        if (splittersCount <= 0)
        {
            SplitterButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            SplitterButton.interactable = false;
        }
        else
        {
            SplitterButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Mathf.Max(splittersCount, 1).ToString();
            SplitterButton.interactable = true;
        }

        if (splittersRgbCount <= 0)
        {
            SplitterRgbButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            SplitterRgbButton.interactable = false;
        }
        else
        {
            SplitterRgbButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Mathf.Max(splittersRgbCount, 1).ToString();
            SplitterRgbButton.interactable = true;
        }
        Canvas.ForceUpdateCanvases();
    }
}
