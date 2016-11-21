namespace SlackAPI.Models
{
    [RequestPath("users.list")]
    public class UserListResponse : Response
    {
        public User[] members;
    }
}
