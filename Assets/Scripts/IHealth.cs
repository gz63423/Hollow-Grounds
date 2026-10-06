public interface IHealth
{
    int CurrentHealth { get; }
    int MaxHealth { get; }
    bool IsAlive();
}