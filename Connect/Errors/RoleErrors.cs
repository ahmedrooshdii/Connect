using Connect.Common;

namespace Connect.Errors
{
    public static class RoleErrors
    {
        public static Error NotFoundByName(string roleName) 
            => new("Role.NotFound", $"No role found with name '{roleName}'.", ErrorType.NotFound);
    }
}
