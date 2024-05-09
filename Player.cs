using Game.Enemies;
using Game.Effects;

namespace Game;

class Player
{
    public static float MaxHP = 100;
    public static float CurrentHP = 100;
    public static int MaxMP = 200;
    public static int CurrentMP = 200;
    public static int PlayerLVL = 1;
    public static float PlayerXP = 0; // PlayerXP per Level, not overall
    private static float XPThreshold = 10;
    public static int Happiness = 0;

    public List<IEffect> Effects = new List<IEffect>();

    // params: Current Enemy
    // returns: amount of gained xp
    // Computes the amount of xp gained and adjust the level of the player
    public static float HandleXP(IEnemy CurrentEnemy)
    {
        float Quot = (CurrentEnemy.Level / PlayerLVL) * 2;
        float PlayerXPNew = (Quot * PlayerLVL);
        PlayerXP += PlayerXPNew;

        float XPThresholdNew = XPThreshold * 2 + 2;
        if (PlayerXP > XPThreshold)
        {
            PlayerLVL++;
            PlayerXP = PlayerXP - XPThreshold;
            XPThreshold = XPThresholdNew;
            LevelUp();
        }

        return PlayerXPNew;
    }

    public static void LevelUp()
    {
        CurrentHP = MaxHP;
        CurrentMP = MaxMP;
        MaxHP += (float)Math.Round(MaxHP * 0.04, 1);
        MaxMP += (int)(MaxMP * 0.01);
        Happiness += 1;
        GameActions.LevelUp();
    }
}