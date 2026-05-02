using Connect.Common;

namespace Connect.Errors
{
    public static class UserErrors
    {
        public static Error NotFound(string userName) 
            => new("User.NotFound", $"No user found with username '{userName}'.", ErrorType.NotFound);

        public static Error UserAlreadyExists
            => new("User.UserAlreadyExists", "A user with the same username already exists.", ErrorType.Conflict);

        public static Error EmailAlreadyExists
            => new("User.EmailAlreadyExists", "A user with the same email already exists.", ErrorType.Conflict);
    }
}
