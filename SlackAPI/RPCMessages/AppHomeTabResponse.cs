namespace SlackAPI
{
    [RequestPath("views.publish")]
    public class AppHomeTabResponse : Response
    {
        public AppHomeTabView view;

        public class AppHomeTabView
        {
            public string id;
            public string team_id;
            public string type;
            public object close;
            public object submit;
            public Block[] blocks;
            public string private_metadata;
            public string callback_id;
            public State state;
            public string hash;
            public bool clear_on_close;
            public bool notify_on_close;
            public string root_view_id;
            public object previous_view_id;
            public string app_id;
            public string external_id;
            public string bot_id;
        }

        public class State
        {
            public Values values;
        }

        public class Values
        {
        }
    }
}
