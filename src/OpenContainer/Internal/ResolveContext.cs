using System;
using System.Collections.Generic;
using OpenLogger;

namespace OpenContainer.Internal
{
    public class ResolveContext
    {
        readonly Stack<DependencyRegistration> _recursionDefender = new Stack<DependencyRegistration>();

        ILogger _log;

        public ResolveContext(DependencyRegistrationCollection registrations, ILogger log)
        {
            Registrations = registrations;
            _log = log;
            Builder = new ObjectBuilder(this, log);
        }

        public ObjectBuilder Builder { get; private set; }
        public DependencyRegistrationCollection Registrations { get; set; }
        protected DefaultContainer Resolver { get; set; }

        public bool CanResolve(DependencyRegistration registration)
        {
            return !_recursionDefender.Contains(registration);
        }

        public object Resolve(Type serviceType)
        {
            return Resolve(Registrations.GetRegistrationForService(serviceType));
        }

        public object Resolve(DependencyRegistration registration)
        {
            if (_recursionDefender.Contains(registration))
                throw new InvalidOperationException("Recursive dependencies are not allowed.");
            try
            {
                _recursionDefender.Push(registration);

                return registration.LifetimeManager.Resolve(this, registration);
            }
            finally
            {
                _recursionDefender.Pop();
            }
        }
    }
}