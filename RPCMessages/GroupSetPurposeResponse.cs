namespace SlackAPI.Models
{
    [RequestPath("groups.setPurpose")]
    public class GroupSetPurposeResponse : Response
    {
        public string purpose;
    }
}
