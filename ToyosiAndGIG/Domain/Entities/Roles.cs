namespace ToyosiAndGIG.Domain.Entities
{
    public static class Roles
    {
        public const string Customer = "Customer";
        public const string Member = "Member";
        public const string Manager = "Manager";
        public const string Owner = "Owner";

        public static readonly string[] All = { Customer, Member, Manager, Owner };

        // Roles allowed to be self-selected during public registration.
        // Manager/Owner should be granted separately by an authenticated admin, not chosen at signup.
        public static readonly string[] PubliclyAssignable = { Customer };
        public static readonly string[] AdminAssignable = { Customer, Member, Manager };

        public static bool IsValid(string role) => All.Contains(role);
        public static bool IsPubliclyAssignable(string role) => PubliclyAssignable.Contains(role);
        public static bool IsAdminAssignable(string role) => AdminAssignable.Contains(role);
    }
}
