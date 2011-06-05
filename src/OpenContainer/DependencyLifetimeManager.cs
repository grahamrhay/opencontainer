using OpenContainer.Internal;

namespace OpenContainer
{
	public abstract class DependencyLifetimeManager
	{
		protected DependencyLifetimeManager(DefaultContainer resolver)
		{
			Resolver = resolver;
		}

		protected DefaultContainer Resolver { get; private set; }

		public virtual bool IsRegistrationAvailable(DependencyRegistration registration)
		{
			return true;
		}

		public virtual object Resolve(ResolveContext context, DependencyRegistration registration)
		{
			return context.Builder.CreateObject(registration);
		}

		public virtual void VerifyRegistration(DependencyRegistration registration)
		{
		}
	}
}