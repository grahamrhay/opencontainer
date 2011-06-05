using System;
using OpenContainer.ContextStore;

namespace OpenContainer.Internal
{
    public class ContextStoreDependency
    {
        public ContextStoreDependency(string key, object instance, IContextStoreDependencyCleaner cleaner)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (instance == null) throw new ArgumentNullException("instance");
            Key = key;
            Instance = instance;
            Cleaner = cleaner;
        }

        public IContextStoreDependencyCleaner Cleaner { get; set; }
        public object Instance { get; set; }
        public string Key { get; set; }
    }
}