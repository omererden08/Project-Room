using UnityEngine;

public class CursorManager : MonoBehaviour
{
    
    void Update()
    {
        if(Cursor.visible == false)
        {
            Cursor.visible = true;
        }
    }
}
