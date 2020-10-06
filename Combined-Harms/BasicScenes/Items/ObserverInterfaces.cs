using Godot;

public delegate void NotifyProviderEnd();

public interface IProvider
{
    Node GenerateObserver(string name);
    event NotifyProviderEnd ProviderEnd;
}

public interface IObserver<T> where T: IProvider
{
    void Init(T provider);
}
