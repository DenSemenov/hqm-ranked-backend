namespace hqm_ranked_backend.Helpers
{
    public static class LogHelper
    {
        public static string GetInfoLog(string text)
        {
            return String.Format("<span style='color: blue'>{0}</span><br />", text);
        }
        public static string GetErrorLog(string exception, string stackTrace)
        {
            return String.Format("<span style='color: red'>{0}</span><br /><span style='color: red'>{1}</span><br />", exception, stackTrace);
        }
    }
}
