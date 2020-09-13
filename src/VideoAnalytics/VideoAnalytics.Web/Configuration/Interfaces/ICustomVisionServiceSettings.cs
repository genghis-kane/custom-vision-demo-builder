namespace VideoAnalytics.Web.Configuration.Interfaces
{
    public interface ICustomVisionServiceSettings
    {
        string AccountRegion { get; set; }
        string AccountName { get; set; }
        string AccountEndpoint { get; }
        string AccountKey { get; set; }
    }
}