using System;

namespace SlackAPI
{
    public class Preferences
    {
        public bool email_misc;
        public bool push_everything;
        public bool seen_notification_prefs_banner;
        public bool seen_welcome_2;
        public bool seen_user_menu_tip_card;
        public bool seen_message_input_tip_card;
        public bool seen_channels_tip_card;
        public bool seen_team_menu_tip_card;
        public bool seen_flexpane_tip_card;
        public bool seen_search_input_tip_card;
        public bool has_uploaded;
        public bool search_only_my_channels;
        public bool seen_channel_menu_tip_card;
        public bool has_invited;
        public bool has_created_channel;
        public bool color_names_in_list;
        public bool growls_enabled;
        public bool push_dm_alert;
        public bool push_mention_alert;
        public bool welcome_message_hidden;
        public bool all_channels_loud;
        public bool show_member_presence;
        public bool expand_inline_imgs;
        public bool expand_internal_inline_imgs;
        public bool seen_ssb_prompt;
        public bool webapp_spellcheck;
        public bool no_joined_overlays;
        public bool no_created_overlays;
        public bool dropbox_enabled;
        public bool mute_sounds;
        public bool arrow_history;
        public bool tab_ui_return_selects;
        public bool obey_inline_img_limit;
        public bool collapsible;
        public bool collapsible_by_click;
        public bool require_at;
        public bool mac_ssb_bullet;
        public bool expand_non_media_attachments;
        public bool show_typing;
        public bool pagekeys_handled;
        public bool time24;
        public bool enter_is_special_in_tbt;
        public bool graphic_emoticons;
        public bool convert_emoticons;
        public bool autoplay_chat_sounds;
        public bool ss_emojis;
        public bool mark_msgs_read_immediately;
        public string tz;
        public string emoji_mode;
        public string highlight_words;
        //public string newxp_slackbot_step; //I don't even...
        public SearchSort search_sort;
        public string push_loud_channels;
        public string push_mention_channels;
        public string push_loud_channels_set;
        public string user_colors;
        public int push_idle_wait;
        public string push_sound;
        public string email_alerts;
        public int email_alerts_sleep_until;
        public string loud_channels;
        public string never_channels;
        public string loud_channels_set;
        public string search_excluse_channels;
        public string messages_theme;
        public string new_msg_snd;
        public string mac_ssb_bounce;
        public string last_snippet_type;
        public int display_real_names_override;
    }
}