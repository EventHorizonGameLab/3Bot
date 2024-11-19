public interface ITakeDamage
{
    /// <summary>
    /// Applies damage
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="type"></param>
    void TakeDamage(float damage, AttackType type);
}
