using UnityEngine;
using System.Collections;

public class SteamcorePanel : IInteractable
{
    [Header("Nuts")]
    [SerializeField] private GameObject[] nuts;          // Animator içeren objeler
    [SerializeField] private GameObject nutsObject;      // Bitiş nesnesi
    private Animator[] nutsAnimators;
    private Animator panelAnimator;

    [Header("Animation Settings")]
    [SerializeField] private string animationTriggerName = "Open";

    private bool hasPlayed = false;

    private void Start()
    {
        // Animator dizisini doldur
        nutsAnimators = new Animator[nuts.Length];
        for (int i = 0; i < nuts.Length; i++)
        {
            nutsAnimators[i] = nuts[i].GetComponent<Animator>();
        }
        panelAnimator = GetComponent<Animator>();
        nutsObject.SetActive(false); // Başta gizli
    }

    public override void Interact()
    {
        if (hasPlayed) return; // Eğer animasyon zaten oynatıldıysa, tekrar oynatma
        hasPlayed = true;
        PlayNutsAnimations();
    }


    private void PlayNutsAnimations()
    {
        foreach (Animator animator in nutsAnimators)
        {
            animator.SetTrigger(animationTriggerName);
        }

        StartCoroutine(WaitForAllAnimationsToFinish());
    }

    private IEnumerator WaitForAllAnimationsToFinish()
    {
        float maxDuration = 0f;

        foreach (Animator animator in nutsAnimators)
        {
            AnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(0);
            if (clipInfos.Length > 0)
            {
                float length = clipInfos[0].clip.length;
                if (length > maxDuration)
                    maxDuration = length;
            }
        }

        if (maxDuration == 0f)
            maxDuration = 1f;

        yield return new WaitForSeconds(maxDuration);

        for (int i = 0; i < nuts.Length; i++)
        {
            nuts[i].SetActive(false);
        }

        nutsObject.SetActive(true);
        Debug.Log("Tüm somunlar animasyonu tamamladı. Obje aktif!");

        // 👇 Panelin kendi animasyonunu tetikle
        if (panelAnimator != null)
        {
            panelAnimator.SetTrigger(animationTriggerName);
        }
    }

}
