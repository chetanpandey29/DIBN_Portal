namespace DIBN.Areas.Admin.Models
{
    public class LogsModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string MessageTemplate { get; set; }
        public string Level { get; set; }
        public string TimeStamp { get; set; }
        public string Exception { get; set; }
        public string LogEvent { get; set; }
        public string Username { get; set; }
        public string Ip { get; set; }
        public string Module { get; set; }
    }
}
