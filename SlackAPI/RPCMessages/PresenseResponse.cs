namespace SlackAPI
{
    [RequestPath("users.setPresence")]
    public class PresenceResponse : Response
    {
    }

    public enum Presence
    {
        active,
        away,
        auto
    }
}
