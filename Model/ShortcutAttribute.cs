namespace AlphalyBot.Model;


[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ShortcutAttribute: Attribute
{
    public ShortcutAttribute(string service, string shortcut, bool auth=false)
    {
        Service = service;
        Auth = auth;
        Shortcut = shortcut;
    }

    public string Service { get; }
    public bool Auth { get; }
    public string Shortcut { get; }
}