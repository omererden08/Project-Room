using UnityEngine;

public class Tube : MonoBehaviour
{
    public int capacity;
    public int currentLitre;


    public void Fill()
    {
        if (capacity == currentLitre)
        {
            Debug.Log("tube is already full>> " + gameObject.name);

        }
        if (currentLitre != capacity)
        {
            Debug.Log("Tube is fulled >> " + gameObject.name);
            currentLitre = capacity;
            CheckCurrentLitre();
        }
    }

    public void Fill(int litre)
    {
        currentLitre+=litre;
        CheckCurrentLitre();
    }
    public void Empty()
    {
        Debug.Log("Tube is empty now>> " + gameObject.name);
        currentLitre = 0;
        CheckCurrentLitre();
    }
    public void Empty(int litre)
    {
        currentLitre-=litre;
        Debug.Log("decr + " + litre + " from tube");
    }
    public void CheckCurrentLitre()
    {
        if (currentLitre < 0)
        {
            Debug.Log("capacity is lower then 0 >> " + gameObject.name);
            currentLitre = 0;
        }
        if (currentLitre > capacity)
        {
            Debug.Log("current is higher than capacity >> " + gameObject.name);
            currentLitre = capacity;
        }
        if (currentLitre > 0 && currentLitre < capacity)
        {
            Debug.Log("normal >> " + gameObject.name);
        }
    }

    public void TransferLiquid()
    {

    }
}
