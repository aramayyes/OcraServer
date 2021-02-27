namespace OcraServer.Extra
{
    public class Constants
    {
        public static class ApplicationRoles
        {
            public const string CLIENT_ROLE = "Client";
            public const string AGENT_ROLE = "Agent";
        }

        public static class UserPoints
        {
            public const int REGISTRATION = 300;
            public const int PROFILE_DATA = 200;
            public const int RESERVATION = 750;
            public const int FEEDBACK = 250;
            public const int CANCEL_RESERVATION = -300;
        }

        public static class StaticFiles
        {
            public const string USER_IMAGES_PATH = "/Files/Photos/Users";
            public const string DEFAULT_IMAGE = "/Default.png";
        }
  
    }
}
