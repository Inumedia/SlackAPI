namespace SlackAPI
{
    public class UserProfile
    {
        public string first_name;
        public string last_name;
        public string real_name;
        public string email;
        public string skype;
        public string status_emoji;
        public string status_text;
        public string phone;
        public string image_24;
        public string image_32;
        public string image_48;
        public string image_72;
        public string image_192;
        public string image_512;

        public override string ToString()
        {
            return real_name;
        }

        public string image_max
        {
            get
            {
                if (!string.IsNullOrEmpty(image_512)) return image_512;
                if (!string.IsNullOrEmpty(image_192)) return image_192;
                if (!string.IsNullOrEmpty(image_72)) return image_72;
                if (!string.IsNullOrEmpty(image_32)) return image_32;
                if (!string.IsNullOrEmpty(image_24)) return image_24;
                return null;
            }
        }
    }
}
