using UnityEngine;

public class Countdown : MonoBehaviour
{

    [Header("Digits")]
    private Transform[] digits = new Transform[4];
    private int[] values = new int[4]; // Her digit'in sayısal değeri
    private Quaternion[] targetRotations = new Quaternion[4]; // Her digit için hedef rotasyon
    private Quaternion[] initialRotations = new Quaternion[4]; // Başlangıç rotasyonları
    private int[] rotationSteps = new int[4]; // Kaç adım döndüğünü sayar
    private float timer = 0f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private int stepCounter1 = 0;
    [SerializeField] private int stepCounter2 = 0;
    [SerializeField] private int stepCounter3 = 0;
    [Header("Gear")]
    [SerializeField] private GameObject gear;
    [SerializeField] private float gearRotationSpeed = 90f; // derece/saniye
    [SerializeField] private float slerpSpeed = 5f;

    private float targetX = 0f;
    private Quaternion targetRotation;
    [SerializeField] private bool isPaused = false; // Oyun duraklatıldı mı? 
    public bool isStarted = false; // Oyun başladı mı?
    public AudioClip bombSound;

    private int[] maxValues = new int[4] { 9, 9, 5, 9 };

    void Start()
    {
        targetRotation = transform.localRotation;

        for (int i = 0; i < 4; i++)
        {
            digits[i] = transform.GetChild(i);
            initialRotations[i] = digits[i].localRotation;
            targetRotations[i] = initialRotations[i];
            rotationSteps[i] = 0;
        }

        // Başlangıç değeri: 09:59
        values[0] = 0;
        values[1] = 9;
        values[2] = 5;
        values[3] = 9;

        EvntManager.StartListening("pauseTimer", OnPause); // Oyun duraklatma olayını dinle
        EvntManager.StartListening("startTimer", StartCountdown); // Oyun başlama olayını dinle

    }

    void Update()
    {
        if (isPaused) return; // Oyun duraklatıldıysa güncellemeleri atla

        if (isStarted)
        {
            for (int i = 0; i < 4; i++)
            {
                digits[i].localRotation = Quaternion.Slerp(
                    digits[i].localRotation,
                    targetRotations[i],
                    Time.deltaTime * rotationSpeed
                );
            }

            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                timer = 0f;
                Tick();
            }
            RotateGear();
        }
    }

    void StartCountdown()
    {
        isStarted = true;
    }


    void OnPause()
    {

        isPaused = !isPaused;

    }

    void RotateGear()
    {
        // Euler x değerini zamanla artır
        targetX += rotationSpeed * Time.deltaTime;

        // Sonsuz artış için sınır yok (Quaternion kendisi normalize olur)
        targetRotation = Quaternion.Euler(targetX, -90f, 90f);

        // Smooth dönüş (Slerp)
        gear.transform.localRotation = Quaternion.Slerp(
            gear.transform.localRotation,
            targetRotation,
            Time.deltaTime * slerpSpeed
        );
    }

    void RotateDigitSpecialFor2()
    {
        float currentX = digits[2].localEulerAngles.x;
        float setX = NormalizeAngle(1f); // üzerine 126° ekle, normalize et
        rotationSteps[2] = 0;
        targetRotations[2] = initialRotations[2] * Quaternion.Euler(setX, 0f, 0f);

    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }

    void Tick()
    {

        if (stepCounter1 == 10)
        {
            GameEnding();
        }


        values[3]--;
        stepCounter3++;

        if (stepCounter3 == 10)
        {

            stepCounter3 = 0;
            values[2]--;
            RotateDigit(3, 36); // normal dönüş
            stepCounter2++;
            if (stepCounter2 == 6)
            {
                stepCounter2 = 0;
                values[1]--;
                stepCounter1++;
                RotateDigitSpecialFor2();
                RotateDigit(1, 36);
                RotateDigit(3, 36); // normal dönüş
                return;
            }
            else
            {
                RotateDigit(2, 36); // normal dönüş
            }
        }
        else
        {
            RotateDigit(3, 36); // normal dönüş
        }

    }

    void GameEnding()
    {
        AudioManager.Instance.audioSource.volume = 0.3f;
        AudioManager.Instance.audioSource.PlayOneShot(bombSound); 
        FadeManager.Instance.BlackScene("MainMenu");
    }


    void RotateDigit(int index, int amount)
    {
        rotationSteps[index]++;
        float totalRotation = amount * rotationSteps[index];
        targetRotations[index] = initialRotations[index] * Quaternion.Euler(totalRotation, 0f, 0f);
    }

}