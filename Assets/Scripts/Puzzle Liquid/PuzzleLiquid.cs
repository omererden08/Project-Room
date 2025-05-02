using UnityEngine;

public class PuzzleLiquid : MonoBehaviour
{
    public Tube[] tubes;
    public Tube t_8l;
    public Tube t_6l;
    public Tube t_5l;
    public Tube t_3l;

    public Tube chosenTube;
    public Tube targetTube;
    public PuzzleManager pm;
    public bool isSolved = false;

    void Start()
    {
        pm=GetComponentInChildren<PuzzleManager>();
        EvntManager.StartListening("CheckLevelLiq",CheckLevelLiq);
        // Initialize tubes array by finding all Tube components in the scene
        tubes = FindObjectsOfType<Tube>();
        foreach (var tube in tubes)
        {
            if (tube.capacity == 8) t_8l = tube;
            else if (tube.capacity == 6) t_6l = tube;
            else if (tube.capacity == 5) t_5l = tube;
            else if (tube.capacity == 3) t_3l = tube;
        }
    }

    // Fills the specified tube to its maximum capacity
    public void FillTube(Tube tube)
    {
        tube.Fill();
        Debug.Log($"Tube filled: {tube.name} now has {tube.currentLitre}L");
    }
    public void FillTube(Tube tube, int amount)
    {
        tube.Fill(amount);
        Debug.Log($"Tube filled: {tube.name} now has {tube.currentLitre}L");
    }


    // Empties the specified tube
    public void EmptyTube(Tube tube)
    {
        tube.Empty();
        Debug.Log($"Tube emptied: {tube.name} now has {tube.currentLitre}L");
    }

    // Transfers liquid from chosen tube to target tube
    public bool TransferLiquid(Tube chosen, Tube target)
    {
        if (chosen == null || target == null)
        {
            Debug.LogWarning("Transfer failed: Chosen or target tube is null");
            return false;
        }

        if (chosen == target)
        {
            Debug.LogWarning("Transfer failed: Cannot transfer to the same tube");
            return false;
        }

        if (chosen.currentLitre == 0)
        {
            Debug.LogWarning("Transfer failed: Chosen tube is empty");
            return false;
        }

        if (target.currentLitre >= target.capacity)
        {
            Debug.LogWarning("Transfer failed: Target tube is full");
            return false;
        }

        // Calculate how much liquid can be transferred
        int targetEmptyCapacity = target.capacity - target.currentLitre;
        int transferAmount = Mathf.Min(chosen.currentLitre, targetEmptyCapacity);

        // Perform the transfer
        chosen.currentLitre -= transferAmount;
        target.currentLitre += transferAmount;

        Debug.Log($"Transferred {transferAmount}L from {chosen.name} ({chosen.currentLitre}L left) to {target.name} ({target.currentLitre}L now)");

        return true;
    }

    // Sets the chosen tube
    public void SetChosen(Tube ch)
    {
        chosenTube = ch;
        Debug.Log(chosenTube != null ? $"Chosen tube set: {chosenTube.name}" : "Chosen tube cleared");
    }

    // Sets the target tube
    public void SetTarget(Tube tar)
    {
        targetTube = tar;
        Debug.Log(targetTube != null ? $"Target tube set: {targetTube.name}" : "Target tube cleared");
    }


    public void CheckLevelLiq()
    {
        
        Debug.Log("controlling CheckLevelLiq");
        if (!isSolved&&t_8l.currentLitre == 4 && t_6l.currentLitre == 4 && t_5l.currentLitre == 4)
        {
            isSolved=true;
            pm.PuzzleSolved();
            Debug.Log("Puzzle Solved!");
        }
    }
}