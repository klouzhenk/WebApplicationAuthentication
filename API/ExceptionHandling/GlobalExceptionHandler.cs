namespace API.ExceptionHandling
{
    public class GlobalExceptionHandler : ApplicationException
    {
        private string msgDetails = String.Empty;
        public DateTime ErrorTimeStamp { get; set; }
        public string CauseOfError { get; set; }

        // constructors
        public GlobalExceptionHandler() { }
        public GlobalExceptionHandler(string msg, string cause, DateTime dateTime)
        {
            this.msgDetails = msg;
            CauseOfError = cause;
            ErrorTimeStamp = dateTime;
        }
        public override string Message => $"Global exception: {this.msgDetails}";
    }
}
