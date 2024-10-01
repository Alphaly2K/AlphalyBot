namespace AlphalyBot.Model
{
    internal class ProgramConfigModel
    {
        public string SqlConnectionString { get; set; } = "data source={host};database={dbname};user id={user};password={passwd};charset={charset};";
        public List<long> Admins { get; set; } = new List<long> { 10001, 10002 };
        public string SqlCollation { get; set; } = "utf8mb4_general_ci";
    }
}
