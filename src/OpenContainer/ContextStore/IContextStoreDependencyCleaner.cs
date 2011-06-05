namespace OpenContainer.ContextStore
{
    public interface IContextStoreDependencyCleaner
    {
        void Destruct(string key, object instance);
    }
}