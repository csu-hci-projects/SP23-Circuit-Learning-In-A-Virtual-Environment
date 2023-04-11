using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemTray
{
    private List<GameObject> itemTemplates;

    private int numItemVariations = Enum.GetValues(typeof(CircuitComponent.Direction)).Length;

    public ItemTray(List<GameObject> itemTemplates)
    {
        this.itemTemplates = itemTemplates;
        initializeeTray();
    }

    private void initializeeTray()
    {
        for (int i=0; i<itemTemplates.Count; i++)
        {
            for (int j=0; j<numItemVariations; j++)
            {
                createTrayItem(i, j);
            }
        }
    }

    private void createTrayItem(int row, int col)
    {
        // find bounds of breadboard

        Vector3 itemPosition = calculateTrayItemPosition(row, col);
        Quaternion itemOrientation = Quaternion.Euler(new Vector3(0, 90*col, 0));

        GameObject item = GameObject.Instantiate(itemTemplates[row], itemPosition, itemOrientation) as GameObject;
        item.GetComponentInChildren<CircuitComponent>().direction = (CircuitComponent.Direction)col;
        string name = itemTemplates[row].name + "_" + Enum.GetName(typeof(CircuitComponent.Direction), col) + "_TrayItem";
        item.name = name;
        item.transform.SetParent(GameObject.Find("ComponentTray").transform);
        item.transform.localPosition = itemPosition;
        item.transform.localRotation = itemOrientation;

        item.GetComponentInChildren<MouseDrag>().gameObject.AddComponent<TrayItem>();

        return;
    }

    private Vector3 calculateTrayItemPosition(int row, int col)
    {
        GameObject boardObject = GameObject.Find("ComponentTray").gameObject;
        Vector3 boardSize = boardObject.GetComponent<MeshFilter>().mesh.bounds.size;

        float boardPosX = -boardSize.x / 2;
        float boardPosY = boardSize.y / 2;
        float boardPosZ = boardSize.z / 2;

        float itemPosX = boardPosX + ((2*col+.5f) / (boardSize.x * (numItemVariations + 1)));
        float itemPosZ = boardPosZ - ((row+.375f) / (boardSize.z * (itemTemplates.Count))) - col * .05f;

        return new Vector3(itemPosX, boardPosY, itemPosZ);
    }
}