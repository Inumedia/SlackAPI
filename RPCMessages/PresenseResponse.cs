namespace SlackAPI.Models
{
    [RequestPath("users.setPresence")]
    public class PresenceResponse : Response
    {
    }
    public enum Presence
    {
        active,
        away
    }
}
