
namespace TenVids.Utilities
{
    public class ChannelCreationResult
    {
        public bool Succeeded { get; }
        public string ErrorMessage { get; }

        private ChannelCreationResult(bool succeeded, string errorMessage)
        {
            Succeeded = succeeded;
            ErrorMessage = errorMessage;
        }

        public static ChannelCreationResult Success() => new(true, null);
        public static ChannelCreationResult Failure(string error) => new(false, error);
    }
}