namespace API.ExceptionHandling
{
    public class GlobalException : ApplicationException
    {
        private string msgDetails = String.Empty;
        public DateTime ErrorTimeStamp { get; set; }
        public string CauseOfError { get; set; }

        // constructors
        public GlobalException() { }
        public GlobalException(string msg)
        {
            this.msgDetails = msg;
            CauseOfError = "Unknown";
        }
        public GlobalException(string msg, string cause, DateTime dateTime)
        {
            this.msgDetails = msg;
            CauseOfError = cause;
            ErrorTimeStamp = dateTime;
        }
        public override string Message => this.msgDetails;
    }
}
