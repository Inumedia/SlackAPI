using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace SlackAPI
{
    /// <summary>
    /// SlackClient is intended to solely handle RPC (HTTP-based) functionality. Does not handle WebSocket connectivity.
    /// 
    /// For WebSocket connectivity, refer to <see cref="SlackAPI.SlackSocketClient"/>
    /// </summary>
    public class SlackClient
    {
        readonly string APIToken;
        bool authWorks = false;

        const string APIBaseLocation = "https://slack.com/api/";
        const int Timeout = 5000;

        const char StartHighlight = '\uE001';
        const char EndHightlight = '\uE001';

        static List<Tuple<string, string>> replacers = new List<Tuple<string, string>>(){
            new Tuple<string,string>("&", "&amp;"),
            new Tuple<string,string>("<", "&lt;"),
            new Tuple<string,string>(">", "&gt;")
        };

        //Dictionary<int, Action<ReceivingMessage>> socketCallbacks;

        public Self MySelf;
        public User MyData;
        public Team MyTeam;

        public List<string> starredChannels;

        public List<User> Users;
        public List<Channel> Channels;
        public List<Channel> Groups;
        public List<DirectMessageConversation> DirectMessages;

        public Dictionary<string, User> UserLookup;
        public Dictionary<string, Channel> ChannelLookup;
        public Dictionary<string, Channel> GroupLookup;
        public Dictionary<string, DirectMessageConversation> DirectMessageLookup;

        //public event Action<ReceivingMessage> OnUserTyping;
        //public event Action<ReceivingMessage> OnMessageReceived;
        //public event Action<ReceivingMessage> OnPresenceChanged;
        //public event Action<ReceivingMessage> OnHello;

        public SlackClient(string token)
        {
            APIToken = token;
        }

		public virtual void Connect(Action<LoginResponse> onConnected = null, Action onSocketConnected = null)
        {
            EmitLogin((loginDetails) =>
            {
				if(loginDetails.ok)
					Connected(loginDetails);
                if (onConnected != null)
                    onConnected(loginDetails);
            });
        }

        protected virtual void Connected(LoginResponse loginDetails)
        {
            MySelf = loginDetails.self;
            MyData = loginDetails.users.First((c) => c.id == MySelf.id);
            MyTeam = loginDetails.team;

            Users = new List<User>(loginDetails.users.Where((c) => !c.deleted));
            Channels = new List<Channel>(loginDetails.channels);
            Groups = new List<Channel>(loginDetails.groups);
            DirectMessages = new List<DirectMessageConversation>(loginDetails.ims.Where((c) => Users.Exists((a) => a.id == c.user) && c.id != MySelf.id));
            starredChannels =
                    Groups.Where((c) => c.is_starred).Select((c) => c.id)
                .Union(
                    DirectMessages.Where((c) => c.is_starred).Select((c) => c.user)
                ).Union(
                    Channels.Where((c) => c.is_starred).Select((c) => c.id)
                ).ToList();

            UserLookup = new Dictionary<string, User>();
            foreach (User u in Users) UserLookup.Add(u.id, u);

            ChannelLookup = new Dictionary<string, Channel>();
            foreach (Channel c in Channels) ChannelLookup.Add(c.id, c);

            GroupLookup = new Dictionary<string, Channel>();
            foreach (Channel g in Groups) GroupLookup.Add(g.id, g);

            DirectMessageLookup = new Dictionary<string, DirectMessageConversation>();
            foreach (DirectMessageConversation im in DirectMessages) DirectMessageLookup.Add(im.id, im);
        }

        public static void APIRequest<K>(Action<K> callback, Tuple<string, string>[] getParameters, Tuple<string, string>[] postParameters)
            where K : Response
        {
            RequestPath path = RequestPath.GetRequestPath<K>();
            //TODO: Custom paths? Appropriate subdomain paths? Not sure.
            //Maybe store custom path in the requestpath.path itself?

            string parameters = getParameters
				.Select(new Func<Tuple<string, string>, string>(a => string.Format("{0}={1}", Uri.EscapeDataString(a.Item1), Uri.EscapeDataString(a.Item2))))
                .Aggregate((a, b) =>
            {
                if (string.IsNullOrEmpty(a))
                    return b;
                else
                    return string.Format("{0}&{1}", a, b);
            });

            Uri requestUri = new Uri(string.Format("{0}?{1}", Path.Combine(APIBaseLocation, path.Path), parameters));

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUri);

            //This will handle all of the processing.
            RequestState<K> state = new RequestState<K>(request, postParameters, callback);
            state.Begin();
        }

        public static void APIGetRequest<K>(Action<K> callback, params Tuple<string, string>[] getParameters)
            where K : Response
        {
            APIRequest<K>(callback, getParameters, new Tuple<string, string>[0]);
        }

        public void APIRequestWithToken<K>(Action<K> callback, params Tuple<string,string>[] getParameters)
            where K : Response
        {
            Tuple<string, string>[] tokenArray = new Tuple<string, string>[]{
                new Tuple<string,string>("token", APIToken)
            };

            if (getParameters != null && getParameters.Length > 0)
                tokenArray = tokenArray.Concat(getParameters).ToArray();

            APIRequest(callback, tokenArray, new Tuple<string, string>[0]);
        }

        public static void StartAuth(Action<AuthStartResponse> callback, string email)
        {
            APIRequest(callback, new Tuple<string, string>[] { new Tuple<string, string>("email", email) }, new Tuple<string, string>[0]);
        }

        public static void AuthSignin(Action<AuthSigninResponse> callback, string userId, string teamId, string password)
        {
            APIRequest(callback, new Tuple<string, string>[] { 
                new Tuple<string,string>("user", userId),
                new Tuple<string,string>("team", teamId),
                new Tuple<string,string>("password", password)
            }, new Tuple<string, string>[0]);
        }

        public void TestAuth(Action<AuthTestResponse> callback)
        {
            APIRequestWithToken(callback);
        }

        public void GetUserList(Action<UserListResponse> callback)
        {
            APIRequestWithToken(callback);
        }

        public void GetChannelList(Action<ChannelListResponse> callback, bool ExcludeArchived = true)
        {
            APIRequestWithToken(callback, new Tuple<string, string>("exclude_archived", ExcludeArchived ? "1" : "0"));
        }

        public void GetGroupsList(Action<GroupListResponse> callback, bool ExcludeArchived = true)
        {
            APIRequestWithToken(callback, new Tuple<string, string>("exclude_archived", ExcludeArchived ? "1" : "0"));
        }

        public void GetDirectMessageList(Action<DirectMessageConversationListResponse> callback)
        {
            APIRequestWithToken(callback);
        }

        public void GetFiles(Action<FileListResponse> callback, string userId = null, DateTime? from = null, DateTime? to = null, int? count = null, int? page = null, FileTypes types = FileTypes.all)
        {
            List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();

            if (!string.IsNullOrEmpty(userId))
                parameters.Add(new Tuple<string,string>("user", userId));

            if (from.HasValue)
                parameters.Add(new Tuple<string, string>("ts_from", from.Value.ToProperTimeStamp()));

            if (to.HasValue)
                parameters.Add(new Tuple<string, string>("ts_to", to.Value.ToProperTimeStamp()));

            if (!types.HasFlag(FileTypes.all))
            {
                FileTypes[] values = (FileTypes[])Enum.GetValues(typeof(FileTypes));
                
                StringBuilder building = new StringBuilder();
                bool first = true;
                for (int i = 0; i < values.Length; ++i)
                {
                    if (types.HasFlag(values[i]))
                    {
                        if (!first) building.Append(",");

                        building.Append(values[i].ToString());

                        first = false;
                    }
                }

                if (building.Length > 0)
                    parameters.Add(new Tuple<string, string>("types", building.ToString()));
            }

            if (count.HasValue)
                parameters.Add(new Tuple<string, string>("count", count.Value.ToString()));

            if (page.HasValue)
                parameters.Add(new Tuple<string, string>("page", page.Value.ToString()));

            APIRequestWithToken(callback, parameters.ToArray());
        }

        void GetHistory<K>(Action<K> historyCallback, string channel, DateTime? latest = null, DateTime? oldest = null, int? count = null)
            where K : MessageHistory
        {
            List<Tuple<string,string>> parameters = new List<Tuple<string,string>>();
            parameters.Add(new Tuple<string, string>("channel", channel));
            
            if(latest.HasValue)
                parameters.Add(new Tuple<string, string>("latest", latest.Value.ToProperTimeStamp()));
            if(oldest.HasValue)
                parameters.Add(new Tuple<string, string>("oldest", oldest.Value.ToProperTimeStamp()));
            if(count.HasValue)
                parameters.Add(new Tuple<string,string>("count", count.Value.ToString()));

            APIRequestWithToken(historyCallback, parameters.ToArray());
        }

        public void GetChannelHistory(Action<ChannelMessageHistory> callback, Channel channelInfo, DateTime? latest = null, DateTime? oldest = null, int? count = null)
        {
            GetHistory(callback, channelInfo.id, latest, oldest, count);
        }

        public void GetDirectMessageHistory(Action<MessageHistory> callback, DirectMessageConversation conversationInfo, DateTime? latest = null, DateTime? oldest = null, int? count = null)
        {
            GetHistory(callback, conversationInfo.id, latest, oldest, count);
        }

        public void GetGroupHistory(Action<GroupMessageHistory> callback, Channel groupInfo, DateTime? latest = null, DateTime? oldest = null, int? count = null)
        {
            GetHistory(callback, groupInfo.id, latest, oldest, count);
        }

        public void MarkChannel(Action<MarkResponse> callback, string channelId, DateTime ts)
        {
            APIRequestWithToken(callback,
                new Tuple<string, string>("channel", channelId),
                new Tuple<string, string>("ts", ts.ToProperTimeStamp())
            );
        }

        public void GetFileInfo(Action<FileInfoResponse> callback, string fileId, int? page = null, int? count = null)
        {
            List<Tuple<string,string>> parameters = new List<Tuple<string,string>>();

            parameters.Add(new Tuple<string,string>("file", fileId));
            
            if(count.HasValue)
                parameters.Add(new Tuple<string,string>("count", count.Value.ToString()));

            if (page.HasValue)
                parameters.Add(new Tuple<string, string>("page", page.Value.ToString()));

            APIRequestWithToken(callback, parameters.ToArray());
        }
        #region Groups
        public void GroupsArchive(Action<GroupArchiveResponse> callback, string channelId)
        {
            APIRequestWithToken(callback, new Tuple<string, string>("channel", channelId));
        }

        public void GroupsClose(Action<GroupCloseResponse> callback, string channelId)
        {
            APIRequestWithToken(callback, new Tuple<string, string>("channel", channelId));
        }

        public void GroupsCreate(Action<GroupCreateResponse> callback, string name)
        {
            APIRequestWithToken(callback, new Tuple<string, string>("name", name));
        }

        public void GroupsCreateChild(Action<GroupCreateChildResponse> callback, string channelId)
        {
            APIRequestWithToken(callback, new Tuple<string, string>("channel", channelId));
        }

        public void GroupsInvite(Action<GroupInviteResponse> callback, string userId, string channelId)
        {
            List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();

            parameters.Add(new Tuple<string, string>("channel", channelId));
            parameters.Add(new Tuple<string, string>("user", userId));

            APIRequestWithToken(callback, parameters.ToArray());
        }

        public void GroupsKick(Action<GroupKickResponse> callback, string userId, string channelId)
        {
            List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();

            parameters.Add(new Tuple<string, string>("channel", channelId));
            parameters.Add(new Tuple<string, string>("user", userId));

            APIRequestWithToken(callback, parameters.ToArray());
        }

        public void GroupsLeave(Action<GroupLeaveResponse> callback, string channelId)
        {
            APIRequestWithToken(callback, new Tuple<string, string>("channel", channelId));
        }

        public void GroupsMark(Action<GroupMarkResponse> callback, string channelId, DateTime ts)
        {
            APIRequestWithToken(callback, new Tuple<string, string>("channel", channelId), new Tuple<string, string>("ts", ts.ToProperTimeStamp()));
        }

        public void GroupsOpen(Action<GroupOpenResponse> callback, string channelId)
        {
            APIRequestWithToken(callback, new Tuple<string, string>("channel", channelId));
        }

        public void GroupsRename(Action<GroupRenameResponse> callback, string channelId, string name)
        {
            List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();

            parameters.Add(new Tuple<string, string>("channel", channelId));
            parameters.Add(new Tuple<string, string>("name", name));

            APIRequestWithToken(callback, parameters.ToArray());
        }

        public void GroupsSetPurpose(Action<GroupSetPurposeResponse> callback, string channelId, string purpose)
        {
            List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();

            parameters.Add(new Tuple<string, string>("channel", channelId));
            parameters.Add(new Tuple<string, string>("purpose", purpose));

            APIRequestWithToken(callback, parameters.ToArray());
        }

        public void GroupsSetTopic(Action<GroupSetPurposeResponse> callback, string channelId, string topic)
        {
            List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();

            parameters.Add(new Tuple<string, string>("channel", channelId));
            parameters.Add(new Tuple<string, string>("topic", topic));

            APIRequestWithToken(callback, parameters.ToArray());
        }

        public void GroupsUnarchive(Action<GroupUnarchiveResponse> callback, string channelId)
        {
            APIRequestWithToken(callback, new Tuple<string, string>("channel", channelId));
        }

        #endregion

        public void SearchAll(Action<SearchResponseAll> callback, string query, SearchSort? sorting = null, SearchSortDirection? direction = null, bool enableHighlights = false, int? count = null, int? page = null)
        {
            List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();
            parameters.Add(new Tuple<string, string>("query", query));

            if (sorting.HasValue)
                parameters.Add(new Tuple<string, string>("sort", sorting.Value.ToString()));

            if (direction.HasValue)
                parameters.Add(new Tuple<string, string>("sort_dir", direction.Value.ToString()));

            if (enableHighlights)
                parameters.Add(new Tuple<string, string>("highlight", "1"));

            if (count.HasValue)
                parameters.Add(new Tuple<string, string>("count", count.Value.ToString()));

            if (page.HasValue)
                parameters.Add(new Tuple<string, string>("page", page.Value.ToString()));

            APIRequestWithToken(callback, parameters.ToArray());
        }

        public void SearchMessages(Action<SearchResponseMessages> callback, string query, SearchSort? sorting = null, SearchSortDirection? direction = null, bool enableHighlights = false, int? count = null, int? page = null)
        {
            List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();
            parameters.Add(new Tuple<string, string>("query", query));

            if (sorting.HasValue)
                parameters.Add(new Tuple<string, string>("sort", sorting.Value.ToString()));

            if (direction.HasValue)
                parameters.Add(new Tuple<string, string>("sort_dir", direction.Value.ToString()));

            if (enableHighlights)
                parameters.Add(new Tuple<string, string>("highlight", "1"));

            if (count.HasValue)
                parameters.Add(new Tuple<string, string>("count", count.Value.ToString()));

            if (page.HasValue)
                parameters.Add(new Tuple<string, string>("page", page.Value.ToString()));

            APIRequestWithToken(callback, parameters.ToArray());
        }

        public void SearchFiles(Action<SearchResponseFiles> callback, string query, SearchSort? sorting = null, SearchSortDirection? direction = null, bool enableHighlights = false, int? count = null, int? page = null)
        {
            List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();
            parameters.Add(new Tuple<string, string>("query", query));

            if (sorting.HasValue)
                parameters.Add(new Tuple<string, string>("sort", sorting.Value.ToString()));

            if (direction.HasValue)
                parameters.Add(new Tuple<string, string>("sort_dir", direction.Value.ToString()));

            if (enableHighlights)
                parameters.Add(new Tuple<string, string>("highlight", "1"));

            if (count.HasValue)
                parameters.Add(new Tuple<string, string>("count", count.Value.ToString()));

            if (page.HasValue)
                parameters.Add(new Tuple<string, string>("page", page.Value.ToString()));

            APIRequestWithToken(callback, parameters.ToArray());
        }

        public void GetStars(Action<StarListResponse> callback, string userId = null, int? count = null, int? page = null){
            List<Tuple<string,string>> parameters = new List<Tuple<string,string>>();
            
            if(!string.IsNullOrEmpty(userId))
                parameters.Add(new Tuple<string,string>("user", userId));

            if(count.HasValue)
                parameters.Add(new Tuple<string,string>("count", count.Value.ToString()));

            if(page.HasValue)
                parameters.Add(new Tuple<string,string>("page", page.Value.ToString()));

            APIRequestWithToken(callback, parameters.ToArray());
        }

        public void PostMessage(
            Action<PostMessageResponse> callback,
            string channelId,
            string text,
            string botName = null,
            string parse = null,
            bool linkNames = false,
            Attachment[] attachments = null,
            bool unfurl_links = false,
            string icon_url = null,
            string icon_emoji = null)
        {
            List<Tuple<string,string>> parameters = new List<Tuple<string,string>>();

            parameters.Add(new Tuple<string,string>("channel", channelId));
            parameters.Add(new Tuple<string,string>("text", text));

            if(!string.IsNullOrEmpty(botName))
                parameters.Add(new Tuple<string,string>("username", botName));

            if (!string.IsNullOrEmpty(parse))
                parameters.Add(new Tuple<string, string>("parse", parse));

            if (linkNames)
                parameters.Add(new Tuple<string, string>("link_names", "1"));

            if (attachments != null && attachments.Length > 0)
                parameters.Add(new Tuple<string, string>("attachments", JsonConvert.SerializeObject(attachments)));

            if (unfurl_links)
                parameters.Add(new Tuple<string, string>("unfurl_links", "1"));

            if (!string.IsNullOrEmpty(icon_url))
                parameters.Add(new Tuple<string, string>("icon_url", icon_url));

            if (!string.IsNullOrEmpty(icon_emoji))
                parameters.Add(new Tuple<string, string>("icon_emoji", icon_emoji));

            APIRequestWithToken(callback, parameters.ToArray());
        }

        public void UploadFile(Action<FileUploadResponse> callback, byte[] fileData, string fileName, string[] channelIds, string title = null, string initialComment = null, bool useAsync = false, string fileType = null)
        {
            Uri target = new Uri(Path.Combine(APIBaseLocation, useAsync ? "files.uploadAsync" : "files.upload"));

            List<string> parameters = new List<string>();
            parameters.Add(string.Format("token={0}", APIToken));

            //File/Content
            if (!string.IsNullOrEmpty(fileType))
                parameters.Add(string.Format("{0}={1}", "filetype", fileType));

            if (!string.IsNullOrEmpty(fileName))
                parameters.Add(string.Format("{0}={1}", "filename", fileName));

            if (!string.IsNullOrEmpty(title))
                parameters.Add(string.Format("{0}={1}", "title", title));

            if (!string.IsNullOrEmpty(initialComment))
                parameters.Add(string.Format("{0}={1}", "initial_comment", initialComment));

            parameters.Add(string.Format("{0}={1}", "channels", string.Join(",", channelIds)));

            using(HttpClient client = new HttpClient())
            using (MultipartFormDataContent form = new MultipartFormDataContent())
            {
                form.Add(new ByteArrayContent(fileData), "file", fileName);
                HttpResponseMessage response = client.PostAsync(string.Format("{0}?{1}", target, string.Join("&", parameters.ToArray())), form).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                callback(JsonConvert.DeserializeObject<FileUploadResponse>(result, new JavascriptDateTimeConverter()));
            }
        }

        public void EmitPresence(Action<PresenceResponse> callback, Presence status)
        {
            APIRequestWithToken(callback, new Tuple<string, string>("presence", status.ToString()));
        }

        public void GetPreferences(Action<UserPreferencesResponse> callback)
        {
            APIRequestWithToken(callback);
        }

        public void GetCounts(Action<UserCountsResponse> callback)
        {
            APIRequestWithToken(callback);
        }

        public void EmitLogin(Action<LoginResponse> callback, string agent = "Inumedia<3")
        {
            APIRequestWithToken(callback, new Tuple<string, string>("agent", agent));
        }
    }
}