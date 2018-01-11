namespace SlackAPI
{
    [RequestPath("users.profile.set")]
    public class ProfileSetRespose : Response
    {
        public UserProfile profile;
    }
}
