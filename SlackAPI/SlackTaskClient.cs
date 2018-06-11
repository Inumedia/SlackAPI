﻿using Newtonsoft.Json;
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
    public class SlackTaskClient
    {
        readonly string APIToken;
        readonly Tuple<string, string>[] APILoginParameters;
        //bool authWorks = false;

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

        public SlackTaskClient(string token, Tuple<string, string>[] loginParameters = null)
        {
            APIToken = token;
            APILoginParameters = loginParameters;
        }

        public virtual async Task<LoginResponse> ConnectAsync()
        {
            var loginDetails = await EmitLoginAsync();
            if (loginDetails.ok)
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

        public static Task<K> APIRequestAsync<K>(Tuple<string, string>[] getParameters, Tuple<string, string>[] postParameters)
            where K : Response
        {
            var path = RequestPath.GetRequestPath<K>();
            //TODO: Custom paths? Appropriate subdomain paths? Not sure.
            //Maybe store custom path in the requestpath.path itself?

            var requestUri = SlackClient.GetSlackUri(Path.Combine(APIBaseLocation, path.Path), getParameters);
            var request = (HttpWebRequest)HttpWebRequest.Create(requestUri);

            //This will handle all of the processing.
            var state = new RequestStateForTask<K>(request, postParameters);
            return state.Execute();
        }

        public static Task<K> APIGetRequestAsync<K>(params Tuple<string, string>[] getParameters)
            where K : Response
        {
            return APIRequestAsync<K>(getParameters, new Tuple<string, string>[0]);
        }

        public Task<K> APIRequestWithTokenAsync<K>()
            where K : Response
        {
            return APIRequestWithTokenAsync<K>(new Tuple<string, string>[] { });
        }

        public Task<K> APIRequestWithTokenAsync<K>(params Tuple<string, string>[] postParameters)
            where K : Response
        {
            var tokenArray = new Tuple<string, string>[]{
                new Tuple<string,string>("token", APIToken)
            };

            return APIRequestAsync<K>(tokenArray, postParameters);
        }

        [Obsolete("Please use the OAuth method for authenticating users")]
        public static Task<AuthStartResponse> StartAuthAsync(string email)
        {
            return APIRequestAsync<AuthStartResponse>(new Tuple<string, string>[] { new Tuple<string, string>("email", email) }, new Tuple<string, string>[0]);
        }

        public static Task<FindTeamResponse> FindTeam(string team)
        {
            //This seems to accept both 'team.slack.com' and just plain 'team'.
            //Going to go with the latter.
            var domainName = new Tuple<string, string>("domain", team);
            return APIRequestAsync<FindTeamResponse>(new Tuple<string, string>[] { domainName }, new Tuple<string, string>[0]);
        }

        public static Task<AuthSigninResponse> AuthSignin(string userId, string teamId, string password)
        {
            return APIRequestAsync<AuthSigninResponse>(new Tuple<string, string>[] {
                new Tuple<string,string>("user", userId),
                new Tuple<string,string>("team", teamId),
                new Tuple<string,string>("password", password)
            }, new Tuple<string, string>[0]);
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
            var parameters = new List<Tuple<string, string>>();

            if (!string.IsNullOrEmpty(userId))
                parameters.Add(new Tuple<string, string>("user", userId));

            if (from.HasValue)
                parameters.Add(new Tuple<string, string>("ts_from", from.Value.ToProperTimeStamp()));

            if (to.HasValue)
                parameters.Add(new Tuple<string, string>("ts_to", to.Value.ToProperTimeStamp()));

            if (!types.HasFlag(FileTypes.all))
            {
                var values = (FileTypes[])Enum.GetValues(typeof(FileTypes));

                var building = new StringBuilder();
                var first = true;
                for (var i = 0; i < values.Length; ++i)
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
            var parameters = new List<Tuple<string, string>>();
            parameters.Add(new Tuple<string, string>("channel", channel));

            if (latest.HasValue)
                parameters.Add(new Tuple<string, string>("latest", latest.Value.ToProperTimeStamp()));
            if (oldest.HasValue)
                parameters.Add(new Tuple<string, string>("oldest", oldest.Value.ToProperTimeStamp()));
            if (count.HasValue)
                parameters.Add(new Tuple<string, string>("count", count.Value.ToString()));

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
            var parameters = new List<Tuple<string, string>>();

            parameters.Add(new Tuple<string, string>("file", fileId));

            if (count.HasValue)
                parameters.Add(new Tuple<string, string>("count", count.Value.ToString()));

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
            var parameters = new List<Tuple<string, string>>();

            parameters.Add(new Tuple<string, string>("channel", channelId));
            parameters.Add(new Tuple<string, string>("user", userId));

            return APIRequestWithTokenAsync<GroupInviteResponse>(parameters.ToArray());
        }

        public Task<GroupKickResponse> GroupsKickAsync(string userId, string channelId)
        {
            var parameters = new List<Tuple<string, string>>();

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
            var parameters = new List<Tuple<string, string>>();

            parameters.Add(new Tuple<string, string>("channel", channelId));
            parameters.Add(new Tuple<string, string>("name", name));

            return APIRequestWithTokenAsync<GroupRenameResponse>(parameters.ToArray());
        }

        public Task<GroupSetPurposeResponse> GroupsSetPurposeAsync(string channelId, string purpose)
        {
            var parameters = new List<Tuple<string, string>>();

            parameters.Add(new Tuple<string, string>("channel", channelId));
            parameters.Add(new Tuple<string, string>("purpose", purpose));

            return APIRequestWithTokenAsync<GroupSetPurposeResponse>(parameters.ToArray());
        }

        public Task<GroupSetTopicResponse> GroupsSetTopicAsync(string channelId, string topic)
        {
            var parameters = new List<Tuple<string, string>>();

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
            var parameters = new List<Tuple<string, string>>();
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
            var parameters = new List<Tuple<string, string>>();
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
            var parameters = new List<Tuple<string, string>>();
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

        public Task<StarListResponse> GetStarsAsync(string userId = null, int? count = null, int? page = null)
        {
            var parameters = new List<Tuple<string, string>>();

            if (!string.IsNullOrEmpty(userId))
                parameters.Add(new Tuple<string, string>("user", userId));

            if (count.HasValue)
                parameters.Add(new Tuple<string, string>("count", count.Value.ToString()));

            if (page.HasValue)
                parameters.Add(new Tuple<string, string>("page", page.Value.ToString()));

            return APIRequestWithTokenAsync<StarListResponse>(parameters.ToArray());
        }

        public Task<DeletedResponse> DeleteMessageAsync(string channelId, DateTime ts)
        {
            var parameters = new List<Tuple<string, string>>()
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
            var parameters = new List<Tuple<string, string>>();

            parameters.Add(new Tuple<string, string>("channel", channelId));
            parameters.Add(new Tuple<string, string>("text", text));

            if (!string.IsNullOrEmpty(botName))
                parameters.Add(new Tuple<string, string>("username", botName));

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

        public Task<FileUploadResponse> UploadFileAsync(byte[] fileData, string fileName, string[] channelIds, string title = null, string initialComment = null, bool useAsync = false, string fileType = null)
        {
            var target = new Uri(Path.Combine(APIBaseLocation, useAsync ? "files.uploadAsync" : "files.upload"));

            var parameters = new List<string>();
            parameters.Add(string.Format("token={0}", APIToken));

            // File/Content
            if (!string.IsNullOrEmpty(fileType))
                parameters.Add(string.Format("{0}={1}", "filetype", fileType));

            if (!string.IsNullOrEmpty(fileName))
                parameters.Add(string.Format("{0}={1}", "filename", fileName));

            if (!string.IsNullOrEmpty(title))
                parameters.Add(string.Format("{0}={1}", "title", title));

            if (!string.IsNullOrEmpty(initialComment))
                parameters.Add(string.Format("{0}={1}", "initial_comment", initialComment));

            parameters.Add(string.Format("{0}={1}", "channels", string.Join(",", channelIds)));

            using (var client = new HttpClient())
            using (var form = new MultipartFormDataContent())
            {
                form.Add(new ByteArrayContent(fileData), "file", fileName);
                var response = client.PostAsync(string.Format("{0}?{1}", target, string.Join("&", parameters.ToArray())), form).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                //callback(JsonConvert.DeserializeObject<FileUploadResponse>(result, new JavascriptDateTimeConverter()));
                throw new NotImplementedException("This operation has not been implemented.");
            }
        }

        public Task<ChannelSetTopicResponse> ChannelSetTopicAsync(string channelId, string newTopic)
        {
            return APIRequestWithTokenAsync<ChannelSetTopicResponse>(
                new Tuple<string, string>("channel", channelId),
                new Tuple<string, string>("topic", newTopic));
        }

        #region Channel

        public Task<ChannelCreateResponse> ChannelsCreateAsync(string name)
        {
            return APIRequestWithTokenAsync<ChannelCreateResponse>(new Tuple<string, string>("name", name));
        }

        public Task<ChannelInviteResponse> ChannelsInviteAsync(string userId, string channelId)
        {
            var parameters = new List<Tuple<string, string>>()
            {
                new Tuple<string,string>("channel", channelId),
                new Tuple<string,string>("user", userId),
            };
            return APIRequestWithTokenAsync<ChannelInviteResponse>(parameters.ToArray());
        }

        public Task<ChannelSetPurposeResponse> ChannelsSetPurposeAsync(string channelId, string purpose)
        {
            var parameters = new List<Tuple<string, string>>()
            {
                new Tuple<string,string>("channel", channelId),
                new Tuple<string,string>("purpose", purpose),
            };
            return APIRequestWithTokenAsync<ChannelSetPurposeResponse>(parameters.ToArray());
        }

        public Task<ChannelRenameResponse> ChannelsRenameAsync(string channelId, string name)
        {
            var parameters = new List<Tuple<string, string>>()
            {
                new Tuple<string,string>("channel", channelId),
                new Tuple<string,string>("name", name),
            };
            return APIRequestWithTokenAsync<ChannelRenameResponse>(parameters.ToArray());
        }

        public Task<ChannelArchiveResponse> ChannelsArchiveAsync(string channelId)
        {
            return APIRequestWithTokenAsync<ChannelArchiveResponse>(new Tuple<string, string>("channel", channelId));
        }

        public Task<ChannelUnarchiveResponse> ChannelsUnarchiveAsync(string channelId)
        {
            return APIRequestWithTokenAsync<ChannelUnarchiveResponse>(new Tuple<string, string>("channel", channelId));
        }

        #endregion

        #region Pin

        public Task<PinListResponse> PinsListAsync(string channelId)
        {
            var parameters = new List<Tuple<string, string>>()
            {
                new Tuple<string,string>("channel", channelId),
            };
            return APIRequestWithTokenAsync<PinListResponse>(parameters.ToArray());
        }

        public Task<PinRemoveResponse> PinsRemoveAsync(string channelId, long? timestamp)
        {
            var parameters = new List<Tuple<string, string>>()
            {
                new Tuple<string,string>("channel", channelId),
                new Tuple<string,string>("timestamp", (timestamp != null ? timestamp.Value.ToString() : null)),
            };
            return APIRequestWithTokenAsync<PinRemoveResponse>(parameters.ToArray());
        }

        #endregion

        #region User

        // https://levels.io/slack-typeform-auto-invite-sign-ups/
        public Task<AdminInviteResponse> AddUserAsync(string email, string firstName, bool restricted, string[] channelIds)
        {
            var parameters = new List<Tuple<string, string>>()
            {
                new Tuple<string,string>("email", email),
                new Tuple<string,string>("first_name", firstName),
                new Tuple<string,string>("channels", (channelIds != null ? string.Join(",", channelIds) : null))
            };
            if (restricted)
                parameters.Add(new Tuple<string, string>("restricted", "1"));
            return APIRequestWithTokenAsync<AdminInviteResponse>(parameters.ToArray());
        }

        public Task<AdminDeleteResponse> DeleteUserAsync(string userId)
        {
            var parameters = new List<Tuple<string, string>>()
            {
                new Tuple<string,string>("userId", userId)
            };
            return APIRequestWithTokenAsync<AdminDeleteResponse>(parameters.ToArray());
        }

        public Task<AdminSetInactiveResponse> SetInactiveUserAsync(string userId)
        {
            var parameters = new List<Tuple<string, string>>()
            {
                new Tuple<string,string>("user", userId),
                new Tuple<string,string>("set_active", "true")
            };
            return APIRequestWithTokenAsync<AdminSetInactiveResponse>(parameters.ToArray());
        }

        public Task<ProfileSetRespose> SetUserProfileEmailAsync(string userId, string email)
        {
            var parameters = new List<Tuple<string, string>>()
            {
                new Tuple<string,string>("user", userId),
                new Tuple<string,string>("profile", "{\"email\":\"" + email + "\"}"),
                new Tuple<string,string>("set_active", "true")
            };
            return APIRequestWithTokenAsync<ProfileSetRespose>(parameters.ToArray());
        }

        public Task<ProfileSetRespose> SetUserProfileFullNameAsync(string userId, string firstName, string lastName)
        {
            var parameters = new List<Tuple<string, string>>()
            {
                new Tuple<string,string>("user", userId),
                new Tuple<string,string>("profile", "{\"first_name\":\"" + firstName + "\",\"last_name\": \"" + lastName +"\"}"),
                new Tuple<string,string>("set_active", "true")
            };
            return APIRequestWithTokenAsync<ProfileSetRespose>(parameters.ToArray());
        }

        public Task<ProfileSetRespose> SetUserProfileUserNameAsync(string userId, string userName)
        {
            var parameters = new List<Tuple<string, string>>()
            {
                new Tuple<string,string>("user", userId),
                new Tuple<string,string>("profile", "{\"username\":\"" + userName + "\"}"),
                new Tuple<string,string>("set_active", "true")
            };
            return APIRequestWithTokenAsync<ProfileSetRespose>(parameters.ToArray());
        }

        #endregion
    }
}