public interface InputInterface
{
    void Register(InputListener il);
    bool Unregister(InputListener il);
}