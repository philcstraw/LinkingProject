using UnityEngine;

public class SoundBoard : MonoBehaviour
{
    public PlaySound columnPowerupSound;
    public PlaySound rowPowerupSound;
    public PlaySound nukePowerupSound;
    public PlaySound depletePersistancePowerupSound;
    public PlaySound deletePersistancePowerupSound;
    public PlaySound allPersistanceClearedSound;
    public PlaySound selectionSound;
    public PlaySound scoreSound;
    public PlaySound glowSound;
    public PlaySound spread;
    public PlaySound persistentScoreFail;
    public PlaySound ratingUpSound;
    public PlaySound gameOver;
    public PlaySoundSequence musicalSequence;

    internal static SoundBoard instance;

    void Awake()
    {
        instance = this;
    }
}
