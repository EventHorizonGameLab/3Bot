public class Singleton<T> where T : class, new()
{
    private static T instance;
    private static readonly object lockObj = new object();

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }
                }
            }
            return instance;
        }
    }

    // Constructor protected to prevent instantiation from other classes
    protected Singleton() { }
}
