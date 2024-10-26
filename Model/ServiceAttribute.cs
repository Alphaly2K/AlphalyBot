using AlphalyBot.Service;

namespace AlphalyBot.Model;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ServiceAttribute : Attribute
{
    public ServiceAttribute(Services service, bool auth = false)
    {
        Service = service;
        Auth = auth;
    }

    public Services Service { get; }

    public bool Auth { get; }
}