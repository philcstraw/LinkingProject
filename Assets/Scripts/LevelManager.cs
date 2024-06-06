using UnityEngine;

// TODO: make the Game manager persistent instead of using this
public class LevelManager : MonoSingleton<LevelManager>
{
    internal GameModeType gameMode = GameModeType.None;

    public override void Awake()
    {
        MakePersistant(true);

        base.Awake();
    }
}
