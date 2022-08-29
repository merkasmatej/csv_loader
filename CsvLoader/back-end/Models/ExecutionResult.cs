namespace back_end.Models
{
    public class ExecutionResult
    {
        /// <summary>
        /// Return Code: 0 = Successful, Others: failure.
        /// </summary>
        public int ReturnCode { get; set; }
        /// <summary>
        /// The return message.
        /// </summary>
        public string ReturnMessage { get; set; }
    }
}
