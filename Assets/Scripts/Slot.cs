using UnityEngine;
[System.Serializable]
public class Slot : MonoBehaviour
{
    public bool isCorrect;          
    public GearSize[] acceptedSizes;
    public Gear currentGear;        
    public bool isFakeSlot;        
    public bool isOccupied;         
}