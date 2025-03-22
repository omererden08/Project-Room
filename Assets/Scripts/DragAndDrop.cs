using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private float zCoordinate;

    // Fare nesnenin üzerine tıklandığında
    void OnMouseDown()
    {
        // Nesnenin dünya koordinatlarındaki Z pozisyonunu al
        zCoordinate = Camera.main.WorldToScreenPoint(transform.position).z;
        
        // Fare pozisyonu ile nesne pozisyonu arasındaki farkı hesapla
        offset = transform.position - GetMouseWorldPosition();
        isDragging = true;
    }

    // Fare bırakıldığında
    void OnMouseUp()
    {
        isDragging = false;
    }

    void Update()
    {
        // Eğer nesne sürükleniyorsa pozisyonunu güncelle
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    // Farenin dünya koordinatlarındaki pozisyonunu döndüren yardımcı metod
    private Vector3 GetMouseWorldPosition()
    {
        // Fare pozisyonunu al (x, y)
        Vector3 mousePoint = Input.mousePosition;
        
        // Z koordinatını ayarla
        mousePoint.z = zCoordinate;
        
        // Ekran koordinatlarını dünya koordinatlarına çevir
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}