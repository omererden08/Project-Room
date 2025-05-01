using System.Data;
using JetBrains.Annotations;
using UnityEngine;

public class PuzzleLiquid : MonoBehaviour
{
    public Tube[] tubes;
    public Tube t_8l;
    public Tube t_6l;
    public Tube t_5l;
    public Tube t_3l;

    public  Tube chosenTube;
    public Tube targetTube;
    void Start()
    {
        tubes = FindObjectsOfType<Tube>();
        for (int t = 0; t <= 3; t++)
        {
            if (tubes[t].capacity == 8)
            {
                t_8l = tubes[t];
            }
            if (tubes[t].capacity == 6)
            {
                t_6l = tubes[t];
            }
            if (tubes[t].capacity == 5)
            {
                t_5l = tubes[t];
            }
            if (tubes[t].capacity == 3)
            {
                t_3l = tubes[t];
            }
        }
    }

    public void FillTube(Tube t)
    {
        t.Fill();
    }
    public void EmptyTube(Tube t)
    {
        t.Empty();
    }
    public void TransferLiquid(Tube chosen, Tube target)
    {
        int c_capacity = chosen.capacity;
        int c_current = chosen.currentLitre;
        int c_emptyCapacity = chosen.capacity - chosen.currentLitre;
        int t_capacity = target.capacity;
        int t_current = target.currentLitre;
        int t_emptyCapacity = target.capacity - chosen.currentLitre;

        int transfer_litre = t_current - c_current;
        int transfer_dec_litre = c_capacity - transfer_litre;
        if (transfer_litre <= t_emptyCapacity)
        {
            Debug.Log("transfer succesful added + " + transfer_litre + " litre ");
            target.Fill(transfer_litre);
            chosen.Empty(transfer_dec_litre);
        }
        else
        {
            Debug.Log("something went wrong");
        }

    }
}
