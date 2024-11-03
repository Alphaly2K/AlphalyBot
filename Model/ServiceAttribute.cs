using AlphalyBot.Service;

namespace AlphalyBot.Model;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ServiceAttribute : Attribute
{
    public ServiceAttribute(string service, bool auth = false, string prompt = "")
    {
        Service = service;
        Auth = auth;
        Prompt = prompt;
    }

    public string Service { get; }
    public bool Auth { get; }
    public string Prompt { get; }
}