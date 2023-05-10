namespace Books.Contracts.Response
{
    public class ValidationResponse
    {
        public required string PropertyName { get; init; }
        public required string Message { get; init; }
    }

}
