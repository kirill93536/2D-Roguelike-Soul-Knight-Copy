using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    /*public List<Room> rooms;
    public float miniMapScale;
    public Vector2 miniMapOffset;
    public Vector2 screenOffset;
    public Image miniMapRoomPrefab;

    public Transform miniMapContainer;

    private void Start()
    {
        //CreateMiniMap();
    }

    private Vector2 CalculateOffset(List<Room> rooms, float minimalXOffset, float minimalYOffset)
    {
        float maxX = 0;
        float maxY = 0;

        List<float> xPositions = new List<float>();
        for (int i = 0; i < rooms.Count; i++)
        {
            Room currentRoom = rooms[i];
            if (i == 0)
            {
                xPositions.Add(currentRoom.currentPosition.x);
                maxX++;
            }
            else
            {
                if (currentRoom.currentPosition.y == rooms[i - 1].currentPosition.y)
                {
                    if (!xPositions.Contains(currentRoom.currentPosition.x))
                    {
                        xPositions.Add(currentRoom.currentPosition.x);
                        maxX++;
                    }
                }
                else
                {
                    xPositions.Clear();
                    xPositions.Add(currentRoom.currentPosition.x);
                    maxX = 1;
                }
            }

            if (currentRoom.currentPosition.x == rooms[i - 1].currentPosition.x)
            {
                maxY++;
            }
        }

        float xLength = (miniMapRoomPrefab.rectTransform.rect.width + miniMapScale) * maxX + minimalXOffset;
        float yLength = (miniMapRoomPrefab.rectTransform.rect.height + miniMapScale) * maxY + minimalYOffset;

        return new Vector2(xLength, yLength);
    }

    public void CreateMiniMap(List<Room> rooms)
    {
        screenOffset = CalculateOffset(rooms, 20, 20);
        //miniMapContainer = new GameObject("MiniMapContainer").transform;
        miniMapContainer.SetParent(transform);
        RectTransform rect = miniMapContainer.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        miniMapContainer.position = new Vector3(Screen.width - miniMapOffset.x, Screen.height - miniMapOffset.y, 0);
        rect.anchoredPosition = screenOffset;
        foreach (Room room in rooms)
        {
            Image miniMapRoom = Instantiate(miniMapRoomPrefab, miniMapContainer);
            miniMapRoom.rectTransform.localPosition = (Vector2)room.transform.position * miniMapScale;
        }
    }*/
}

/*
 * foreach (Room room in rooms)
        {
            Image miniMapRoom = Instantiate(miniMapRoomPrefab, transform);
            miniMapRoom.rectTransform.localPosition = (Vector2)room.transform.position * miniMapScale + miniMapOffset;
        }
*/