namespace SlackAPI
{
    [RequestPath("views.publish")]
    public class PublishViewResponse : Response
    {
        public string warning;
        public ViewResponse view;

        public ResponseMetadata response_metadata { get; set; }

        public class ResponseMetadata
        {
            public string[] messages { get; set; }
        }

        public class ViewResponse
        {
            public string id;
            public string team_id;
            public string app_id;
            public string app_installed_team_id;
            public string bot_id;
            public string type;
            public IBlock[] blocks;
            public string hash;
            public string private_metadata;
            public string callback_id;
            public string root_view_id;
            public string external_id;
            public Text title;
            public object close;
            public object submit;
            public object previous_view_id;
            public bool clear_on_close;
            public bool notify_on_close;
        }
    }
}
