using UnityEngine;

[System.Serializable]
public class Slot : MonoBehaviour
{
    public bool isCorrect;          // Doğru pozisyonda mı
    public GearSize[] acceptedSizes;// Kabul edilen çark boyutları
    public Gear currentGear;        // Şu anki çark
    public bool isFakeSlot;         // Fake slot mu
    public bool isOccupied;         // Slot dolu mu
}