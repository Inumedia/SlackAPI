using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    public class SlackTaskClient : SlackClientBase
    {
        private readonly string APIToken;

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

        public SlackTaskClient(string token)
        {
            APIToken = token;
        }

        public SlackTaskClient(string token, IWebProxy proxySettings)
            : base(proxySettings)
        {
            APIToken = token;
        }

        public virtual async Task<LoginResponse> ConnectAsync()
        {
            var loginDetails = await EmitLoginAsync();
            if(loginDetails.ok)
                Connected(loginDetails);

            return loginDetails;
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

        public Task<K> APIRequestWithTokenAsync<K>()
            where K : Response
        {
            return APIRequestWithTokenAsync<K>(new Tuple<string, string>[] { });
        }
 
        public Task<K> APIRequestWithTokenAsync<K>(params Tuple<string,string>[] postParameters)
            where K : Response
        {
            Tuple<string, string>[] tokenArray = new Tuple<string, string>[]{
                new Tuple<string,string>("token", APIToken)
            };

            return APIRequestAsync<K>(tokenArray, postParameters);
        }

        public Task<AuthTestResponse> TestAuthAsync()
        {
            return APIRequestWithTokenAsync<AuthTestResponse>();
        }

        public Task<UserListResponse> GetUserListAsync()
        {
            return APIRequestWithTokenAsync<UserListResponse>();
        }

        public Task<ChannelListResponse> GetChannelListAsync(bool ExcludeArchived = true)
        {
            return APIRequestWithTokenAsync<ChannelListResponse>(new Tuple<string, string>("exclude_archived", ExcludeArchived ? "1" : "0"));
        }

        public Task<GroupListResponse> GetGroupsListAsync(bool ExcludeArchived = true)
        {
            return APIRequestWithTokenAsync<GroupListResponse>(new Tuple<string, string>("exclude_archived", ExcludeArchived ? "1" : "0"));
        }

        public Task<DirectMessageConversationListResponse> GetDirectMessageListAsync()
        {
            return APIRequestWithTokenAsync<DirectMessageConversationListResponse>();
        }

        public Task<FileListResponse> GetFilesAsync(string userId = null, DateTime? from = null, DateTime? to = null, int? count = null, int? page = null, FileTypes types = FileTypes.all)
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

            return APIRequestWithTokenAsync<FileListResponse>(parameters.ToArray());
        }

        private Task<K> GetHistoryAsync<K>(string channel, DateTime? latest = null, DateTime? oldest = null, int? count = null)
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

            return APIRequestWithTokenAsync<K>(parameters.ToArray());
        }

        public Task<ChannelMessageHistory> GetChannelHistoryAsync(Channel channelInfo, DateTime? latest = null, DateTime? oldest = null, int? count = null)
        {
            return GetHistoryAsync<ChannelMessageHistory>(channelInfo.id, latest, oldest, count);
        }

        public Task<MessageHistory> GetDirectMessageHistoryAsync(DirectMessageConversation conversationInfo, DateTime? latest = null, DateTime? oldest = null, int? count = null)
        {
            return GetHistoryAsync<MessageHistory>(conversationInfo.id, latest, oldest, count);
        }

        public Task<GroupMessageHistory> GetGroupHistoryAsync(Channel groupInfo, DateTime? latest = null, DateTime? oldest = null, int? count = null)
        {
            return GetHistoryAsync<GroupMessageHistory>(groupInfo.id, latest, oldest, count);
        }

        public Task<MarkResponse> MarkChannelAsync(string channelId, DateTime ts)
        {
            return APIRequestWithTokenAsync<MarkResponse>(new Tuple<string, string>("channel", channelId),
                new Tuple<string, string>("ts", ts.ToProperTimeStamp())
            );
        }

        public Task<FileInfoResponse> GetFileInfoAsync(string fileId, int? page = null, int? count = null)
        {
            List<Tuple<string,string>> parameters = new List<Tuple<string,string>>();

            parameters.Add(new Tuple<string,string>("file", fileId));
            
            if(count.HasValue)
                parameters.Add(new Tuple<string,string>("count", count.Value.ToString()));

            if (page.HasValue)
                parameters.Add(new Tuple<string, string>("page", page.Value.ToString()));

            return APIRequestWithTokenAsync<FileInfoResponse>(parameters.ToArray());
        }
        #region Groups
        public Task<GroupArchiveResponse> GroupsArchiveAsync(string channelId)
        {
            return APIRequestWithTokenAsync<GroupArchiveResponse>(new Tuple<string, string>("channel", channelId));
        }

        public Task<GroupCloseResponse> GroupsCloseAsync(string channelId)
        {
            return APIRequestWithTokenAsync<GroupCloseResponse>(new Tuple<string, string>("channel", channelId));
        }

        public Task<GroupCreateResponse> GroupsCreateAsync(string name)
        {
            return APIRequestWithTokenAsync<GroupCreateResponse>(new Tuple<string, string>("name", name));
        }

        public Task<GroupCreateChildResponse> GroupsCreateChildAsync(string channelId)
        {
            return APIRequestWithTokenAsync<GroupCreateChildResponse>(new Tuple<string, string>("channel", channelId));
        }

        public Task<GroupInviteResponse> GroupsInviteAsync(string userId, string channelId)
        {
            List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();

            parameters.Add(new Tuple<string, string>("channel", channelId));
            parameters.Add(new Tuple<string, string>("user", userId));

            return APIRequestWithTokenAsync<GroupInviteResponse>(parameters.ToArray());
        }

        public Task<GroupKickResponse> GroupsKickAsync(string userId, string channelId)
        {
            List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();

            parameters.Add(new Tuple<string, string>("channel", channelId));
            parameters.Add(new Tuple<string, string>("user", userId));

            return APIRequestWithTokenAsync<GroupKickResponse>(parameters.ToArray());
        }

        public Task<GroupLeaveResponse> GroupsLeaveAsync(string channelId)
        {
            return APIRequestWithTokenAsync<GroupLeaveResponse>(new Tuple<string, string>("channel", channelId));
        }

        public Task<GroupMarkResponse> GroupsMarkAsync(string channelId, DateTime ts)
        {
            return APIRequestWithTokenAsync<GroupMarkResponse>(new Tuple<string, string>("channel", channelId), new Tuple<string, string>("ts", ts.ToProperTimeStamp()));
        }

        public Task<GroupOpenResponse> GroupsOpenAsync(string channelId)
        {
            return APIRequestWithTokenAsync<GroupOpenResponse>(new Tuple<string, string>("channel", channelId));
        }

        public Task<GroupRenameResponse> GroupsRenameAsync(string channelId, string name)
        {
            List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();

            parameters.Add(new Tuple<string, string>("channel", channelId));
            parameters.Add(new Tuple<string, string>("name", name));

            return APIRequestWithTokenAsync<GroupRenameResponse>(parameters.ToArray());
        }

        public Task<GroupSetPurposeResponse> GroupsSetPurposeAsync(string channelId, string purpose)
        {
            List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();

            parameters.Add(new Tuple<string, string>("channel", channelId));
            parameters.Add(new Tuple<string, string>("purpose", purpose));

            return APIRequestWithTokenAsync<GroupSetPurposeResponse>(parameters.ToArray());
        }

        public Task<GroupSetTopicResponse> GroupsSetTopicAsync(string channelId, string topic)
        {
            List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();

            parameters.Add(new Tuple<string, string>("channel", channelId));
            parameters.Add(new Tuple<string, string>("topic", topic));

            return APIRequestWithTokenAsync<GroupSetTopicResponse>(parameters.ToArray());
        }

        public Task<GroupUnarchiveResponse> GroupsUnarchiveAsync(string channelId)
        {
            return APIRequestWithTokenAsync<GroupUnarchiveResponse>(new Tuple<string, string>("channel", channelId));
        }

        #endregion

        public Task<SearchResponseAll> SearchAllAsync(string query, SearchSort? sorting = null, SearchSortDirection? direction = null, bool enableHighlights = false, int? count = null, int? page = null)
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

            return APIRequestWithTokenAsync<SearchResponseAll>(parameters.ToArray());
        }

        public Task<SearchResponseMessages> SearchMessagesAsync(string query, SearchSort? sorting = null, SearchSortDirection? direction = null, bool enableHighlights = false, int? count = null, int? page = null)
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

            return APIRequestWithTokenAsync<SearchResponseMessages>(parameters.ToArray());
        }

        public Task<SearchResponseFiles> SearchFilesAsync(string query, SearchSort? sorting = null, SearchSortDirection? direction = null, bool enableHighlights = false, int? count = null, int? page = null)
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

            return APIRequestWithTokenAsync<SearchResponseFiles>(parameters.ToArray());
        }

        public Task<StarListResponse> GetStarsAsync(string userId = null, int? count = null, int? page = null){
            List<Tuple<string,string>> parameters = new List<Tuple<string,string>>();
            
            if(!string.IsNullOrEmpty(userId))
                parameters.Add(new Tuple<string,string>("user", userId));

            if(count.HasValue)
                parameters.Add(new Tuple<string,string>("count", count.Value.ToString()));

            if(page.HasValue)
                parameters.Add(new Tuple<string,string>("page", page.Value.ToString()));

            return APIRequestWithTokenAsync<StarListResponse>(parameters.ToArray());
        }

        public Task<DeletedResponse> DeleteMessageAsync(string channelId, DateTime ts)
        {
            List<Tuple<string, string>> parameters = new List<Tuple<string, string>>()
            {
                new Tuple<string,string>("ts", ts.ToProperTimeStamp()),
                new Tuple<string,string>("channel", channelId)
            };

            return APIRequestWithTokenAsync<DeletedResponse>(parameters.ToArray());
        }

        public Task<PresenceResponse> EmitPresence(Presence status)
        {
            return APIRequestWithTokenAsync<PresenceResponse>(new Tuple<string, string>("presence", status.ToString()));
        }

        public Task<UserPreferencesResponse> GetPreferencesAsync()
        {
            return APIRequestWithTokenAsync<UserPreferencesResponse>();
        }

        public Task<UserCountsResponse> GetCountsAsync()
        {
            return APIRequestWithTokenAsync<UserCountsResponse>();
        }

        public Task<LoginResponse> EmitLoginAsync(string agent = "Inumedia.SlackAPI")
        {
            return APIRequestWithTokenAsync<LoginResponse>(new Tuple<string, string>("agent", agent));
        }

        public Task<JoinDirectMessageChannelResponse> JoinDirectMessageChannelAsync(string user)
        {
            var param = new Tuple<string, string>("user", user);
            return APIRequestWithTokenAsync<JoinDirectMessageChannelResponse>(param);
        }

        public Task<PostMessageResponse> PostMessageAsync(
            string channelId,
            string text,
            string botName = null,
            string parse = null,
            bool linkNames = false,
            Attachment[] attachments = null,
            bool unfurl_links = false,
            string icon_url = null,
            string icon_emoji = null,
            bool as_user = false)
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

            parameters.Add(new Tuple<string, string>("as_user", as_user.ToString()));

            return APIRequestWithTokenAsync<PostMessageResponse>(parameters.ToArray());
        }

        public async Task<FileUploadResponse> UploadFileAsync(byte[] fileData, string fileName, string[] channelIds, string title = null, string initialComment = null, bool useAsync = false, string fileType = null)
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

            using (MultipartFormDataContent form = new MultipartFormDataContent())
            {
                form.Add(new ByteArrayContent(fileData), "file", fileName);
                HttpResponseMessage response = PostRequest(string.Format("{0}?{1}", target, string.Join("&", parameters.ToArray())), form);
                string result = await response.Content.ReadAsStringAsync();
                return result.Deserialize<FileUploadResponse>();
            }
        }

        public Task<ChannelSetTopicResponse> ChannelSetTopicAsync(string channelId, string newTopic)
        {
            return APIRequestWithTokenAsync<ChannelSetTopicResponse>(
                    new Tuple<string, string>("channel", channelId),
                    new Tuple<string, string>("topic", newTopic));
        }
    }
}