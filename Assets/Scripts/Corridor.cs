using UnityEngine;

public class Corridor : MonoBehaviour
{
    public bool isVertical = false;

    public void SetOrientation(bool vertical)
    {
        isVertical = vertical;
        if (vertical)
            transform.rotation = Quaternion.Euler(0, 0, 90);
        else
            transform.rotation = Quaternion.identity;
    }
}
