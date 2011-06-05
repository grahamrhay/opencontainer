namespace OpenContainer.Internal
{
    internal class TransientLifetimeManager : DependencyLifetimeManager
    {
        public TransientLifetimeManager(DefaultContainer builder)
            : base(builder)
        {
        }
    }
}