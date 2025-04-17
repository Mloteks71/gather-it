namespace Domain.Enums;

[Flags]
public enum RemoteType
{
    None = 0,
    Stationary = 1 << 0,
    Hybrid = 1 << 1,
    Remote = 1 << 2,
}
