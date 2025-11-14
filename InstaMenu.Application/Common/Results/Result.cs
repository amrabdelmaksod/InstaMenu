namespace InstaMenu.Application.Common.Results
{
    /// <summary>
    /// Represents the result of an operation without a return value
    /// </summary>
    public class Result
    {
  public bool IsSuccess { get; }
      public bool IsFailure => !IsSuccess;
        public string Error { get; }

        protected Result(bool isSuccess, string error)
        {
            IsSuccess = isSuccess;
      Error = error ?? string.Empty;
      }

    public static Result Success() => new(true, string.Empty);
        public static Result Failure(string error) => new(false, error);

public static Result<T> Success<T>(T value) => new(true, value, string.Empty);
        public static Result<T> Failure<T>(string error) => new(false, default, error);
    }

    /// <summary>
    /// Represents the result of an operation with a return value
    /// </summary>
    public class Result<T> : Result
    {
        public T? Value { get; }

        internal Result(bool isSuccess, T? value, string error) : base(isSuccess, error)
        {
   Value = value;
        }

        public static Result<T> Success(T value) => new(true, value, string.Empty);
   public static new Result<T> Failure(string error) => new(false, default, error);

  // Implicit conversion from T to Result<T>
        public static implicit operator Result<T>(T value) => Success(value);
    }
}