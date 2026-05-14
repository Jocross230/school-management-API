namespace SecSchoolApi.Model
{
    public class GradingScheme
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "Default";
        public int AMin { get; set; } = 70;
        public int BMin { get; set; } = 60;
        public int CMin { get; set; } = 50;
        public int DMin { get; set; } = 45;
        public int EMin { get; set; } = 40;
        public int FMax { get; set; } = 39;
    }
}
