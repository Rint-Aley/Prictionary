namespace Prictionary;

public static class Constants
{
    public static class TokenConstants
    {
        public const string REFRESH_TOKEN_COOKIE_NAME = "RefreshToken";
    }

    public static class MeaningPriorities
    {
        /// <summary>
        /// Interval after the last meaning that newer priorities have by default
        /// </summary>
        public const int defaultInterval = 500;
    }
}
