namespace SlackAPI
{
    [RequestPath("users.prefs.get")]
    public class UserPreferencesResponse: Response
    {
        public Preferences prefs;
    }
}
