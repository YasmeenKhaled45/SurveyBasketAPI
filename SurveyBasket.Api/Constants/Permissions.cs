namespace SurveyBasket.Api.Constants
{
    public static class Permissions
    {
        public static string Type { get; } = "permissions";
        public const string GetPolls = "polls:get";
        public const string AddPolls = "polls:add";
        public const string UpdatePolls = "polls:update";

        public const string GetQuestions = "questions:get";
        public const string AddQuestions = "questions:add";
        public const string UpdateQuestions = "questions:update";

        public const string GetUsers = "users:get";
        public const string AddUsers = "users:add";
        public const string UpdateUsers = "users:update";

        public const string GetRoles = "roles:get";
        public const string AddRoles = "roles:add";
        public const string UpdateRoles = "roles:update";

        public const string Results = "results:read";

        public static IList<string?> GetAllPermissions()=>
            typeof(Permissions).GetFields().Select(x=>x.GetValue(x) as string).ToList();
    }
}
