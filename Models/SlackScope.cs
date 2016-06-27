namespace SlackAPI.Models
{
    public enum SlackScope
    {
        Identify = 1,
        Read = 2,
        Post = 4,
        Client = 8,
        Admin = 16,
    }
}
