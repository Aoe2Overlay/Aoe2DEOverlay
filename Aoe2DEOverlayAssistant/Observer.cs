namespace Aoe2DEOverlayAssistant
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