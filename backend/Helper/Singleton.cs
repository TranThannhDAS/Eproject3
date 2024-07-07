namespace backend.Helper
{
    public static class Singleton
    {
        private static readonly Dictionary<Type, object> _instance = new Dictionary<Type, object>();
        private static object _instanceLock = new object();

        public static object Instance<T>() where T : new()
        {
            Type type = typeof(T);
            lock (_instanceLock)
            {
                if (!_instance.TryGetValue(type, out object instance))
                {
                    instance = new T();
                    _instance[type] = instance;
                }
                return (T)instance;
            }
        }
    }
}
