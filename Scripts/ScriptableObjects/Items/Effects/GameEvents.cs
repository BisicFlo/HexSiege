using System;
public static class GameEvents {    

    public static event Action<Turret, Enemy> OnEnemyKilled;
    public static event Action<Turret, Enemy> OnEnemyHit;
    public static event Action<Enemy, float> OnPlayerHit;
    //public static event Action OnLevelUp;
    //public static event Action OnRerollShop;
    //public static event Action OnPurchase;

    public static void EnemyKilled(Turret t, Enemy e) => OnEnemyKilled?.Invoke(t,e);
    public static void EnemyHit(Turret t, Enemy e) => OnEnemyHit?.Invoke(t, e);
    public static void PlayerHit(Enemy e, float dmg) => OnPlayerHit?.Invoke(e, dmg);

}
