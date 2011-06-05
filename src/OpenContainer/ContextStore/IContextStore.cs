namespace OpenContainer.ContextStore
{
    public interface IContextStore
    {
        object this[string key] { get; set; }
    }
}