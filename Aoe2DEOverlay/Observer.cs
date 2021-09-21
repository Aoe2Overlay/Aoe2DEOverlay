namespace Aoe2DEOverlay
{
    public interface IServiceObserver
    {
        void Update(Data data);
    }
    
    public interface ISettingObserver
    {
        void Changed();
    }
}