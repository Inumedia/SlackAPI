namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("dnd_updated_user")]
    public class DndUpdatedUser
    {
        public string user;
        /*
        dnd_status": {
            "dnd_enabled": true,
            "next_dnd_start_ts": 1450387800,
            "next_dnd_end_ts": 1450423800
        }
        */
    }
}