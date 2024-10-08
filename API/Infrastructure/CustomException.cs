namespace API.Infrastructure
{
    public class CustomException : ApplicationException
    {
        private string msgDetails = String.Empty;
        public DateTime ErrorTimeStamp { get; set; }
        public string CauseOfError { get; set; }

		#region Constructors

		public CustomException() { }

		public CustomException(string msg)
		{
			this.msgDetails = msg;
			CauseOfError = "Unknown";
		}
		
		public CustomException(string msg, string cause, DateTime dateTime)
		{
			this.msgDetails = msg;
			CauseOfError = cause;
			ErrorTimeStamp = dateTime;
		}

		#endregion

		public override string Message => this.msgDetails;
    }
}
