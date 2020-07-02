namespace MFrame.ObjectPoolSystem
{
    public interface IPoolable
    {
        void OnRestart();
        void OnRecycle();
    }
}