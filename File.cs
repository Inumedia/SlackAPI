using System;

namespace SlackAPI
{
    //TODO: See below:
    /// <summary>
    /// Please do a full, thorough, review of this.
    /// </summary>
    public class File
    {
        public string id;
        public DateTime created;
        public DateTime timestamp;

        public string name;
        public string title;
        public string mimetype;
        public string filetype;
        public string pretty_type;
        public string user;

        public string mode;
        public bool editable;
        public bool is_external;
        public string external_type;
        public string username;

        /// <summary>
        /// File size in bytes
        /// </summary>
        public int size;

        public string url;
        public string url_download;
        public string url_private;
        public string url_private_download;

        public string thumb_64;
        public string thumb_80;
        public string thumb_160;
        public string thumb_360;
        public string thumb_360_gif;
        public int thumb_360_w;
        public int thumb_360_h;
        public string thumb_480;
        public int thumb_480_w;
        public int thumb_480_h;

        public string permalink;
        public string permalink_public;
        public string edit_link;
        public string preview;
        public string preview_highlight;
        public int lines;
        public int lines_more;

        public bool is_public;
        public bool public_url_shared;
        public bool display_as_bot;
        public string[] channels;
        public string[] groups;
        public string[] ims;
        public FileComment initial_comment;
        public int comments_count;
        public int num_stars;
        public bool is_starred;
        public string[] pinned_to;

        public Reaction[] reactions;
    }

    [Flags]
    public enum FileTypes
    {
        all = 63,
        posts = 1,
        snippets = 2,
        images = 4,
        gdocs = 8,
        zips = 16,
        pdfs = 32
    }

    public class FileComment
    {
        public string id;
        public DateTime timestamp;
        public string user;
        public string comment;
    }
}
