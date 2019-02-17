namespace HeldenServer.Handlers
{
    public interface IHandler
    {

        void HandleClient(ClientManager clientManager);

        void UnhandleClient(ClientManager clientManager);

    }
}
