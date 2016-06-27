namespace SlackAPI.Models
{
    [RequestPath("presense.set")]
    public class PresenceResponse : Response
    {
    }
    public enum Presence
    {
        active,
        away
    }
}
