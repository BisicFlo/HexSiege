using System;

public static class GameEvents {    

    public static event Action<Turret, Enemy> OnEnemyKilled;
    public static event Action<Turret, Enemy> OnEnemyHit;
    public static event Action<Enemy, int, bool> OnPlayerHit;
    public static event Action OnShopRerolled;
    public static event Action OnDefeat;
    public static event Action OnVictory;


    //public static event Action OnPurchase;
    //public static event Action OnLevelUp;

    public static void EnemyKilled(Turret t, Enemy e) => OnEnemyKilled?.Invoke(t,e);
    public static void EnemyHit(Turret t, Enemy e) => OnEnemyHit?.Invoke(t, e);
    public static void PlayerHit(Enemy e, int dmg, bool canBeFatal) => OnPlayerHit?.Invoke(e, dmg, canBeFatal);
    public static void ShopRerolled() => OnShopRerolled?.Invoke();
    public static void Defeat() => OnDefeat?.Invoke();
    public static void Victory() => OnVictory?.Invoke();



}
