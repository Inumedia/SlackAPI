namespace SlackAPI.Models
{
    [RequestPath("users.prefs.get")]
    public class UserPreferencesResponse: Response
    {
        public Preferences prefs;
    }
}
