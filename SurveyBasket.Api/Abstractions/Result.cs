namespace SurveyBasket.Api.Abstractions
{
    public class Result
    {
        public Result(bool issuccess, Error error) 
        {
            if ((issuccess && error != Error.none) || (!issuccess && error == Error.none))
                throw new InvalidOperationException();
            IsSuccess = issuccess;
            Error = error;
        }

        public bool IsSuccess { get;}
        public bool IsFailure => !IsSuccess;
        public Error Error { get; } = default!;

        public static Result Success()=> new(true, Error.none);
        public static Result Failure(Error error) => new(false, error);
        public static Result<TValue> Success<TValue>(TValue value)=> new(value,true,Error.none);
        public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
    }

    public class Result<TValue> : Result
    {
        private readonly TValue _value;

        public Result(TValue? value,bool IsSuccess , Error error) : base(IsSuccess, error) 
        {
            _value = value;
        }
        public TValue Value=> IsSuccess ? _value! : throw new InvalidOperationException();
    } 
}
