using UnityEngine;

public class Tube : MonoBehaviour
{
    public int capacity;
    public int currentLitre;
    public Material material;
    //ELLE AYARLA
    void Start()
    {
        ApplyLitre();
        EvntManager.StartListening("ApplyLitre", ApplyLitre);
    }
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
            ApplyLitre();
        }
    }

    public void Fill(int litre)
    {
        currentLitre += litre;
        CheckCurrentLitre();
        ApplyLitre();

    }
    public void Empty()
    {
        Debug.Log("Tube is empty now>> " + gameObject.name);
        currentLitre = 0;
        CheckCurrentLitre();
        ApplyLitre();

    }
    public void Empty(int litre)
    {
        currentLitre -= litre;
        Debug.Log("decr + " + litre + " from tube");
        ApplyLitre();

    }
    public void CheckCurrentLitre()
    {
        if (currentLitre < 0)
        {
            Debug.Log("capacity is lower then 0 >> " + gameObject.name);
            currentLitre = 0;
            ApplyLitre();

        }
        if (currentLitre > capacity)
        {
            Debug.Log("current is higher than capacity >> " + gameObject.name);
            currentLitre = capacity;
            ApplyLitre();

        }
        if (currentLitre > 0 && currentLitre < capacity)
        {
            Debug.Log("normal >> " + gameObject.name);
        }
    }

    public void TransferLiquid()
    {

    }

    public void ApplyLitre()
    {

        float fillValue = currentLitre / (float)capacity;
        float corrected = fillValue;;
        if (fillValue > 0.5)
        {
            corrected = fillValue * 2;
        }
        if(fillValue < 0.5)
        {
            corrected = fillValue - 1;
        }
        Debug.Log("fillalueeeee   " + fillValue);
        material.SetFloat("_fill", corrected);

    }
}
