namespace ProjectOOctopus.Services
{
    /// <summary>
    /// Class for helping with getting services at runtime
    /// </summary>
    public class ServicesHelper
    {
        public static IServiceProvider Services { get; private set; }

        /// <summary>
        /// Initializes the service provider
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void Init(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }

        /// <summary>
        /// Returns service by specification in generics
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Returns service as object if found, otherwise false</returns>
        public static T? GetService<T>()
        {
            if (Services is null) return default;
            return Services.GetService<T>();
        }

        /// <summary>
        /// Returns service by specification in type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Returns service as object if found, otherwise false</returns>
        public static object? GetService(Type type)
        {
            if (Services is null) return default;
            return Services.GetService(type);
        }
    }
}
