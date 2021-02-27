namespace OcraServer.Models.ViewModels
{
    public class SearchResultViewModel
    {
        public int ID { get; set; }

        public string Name_Eng { get; set; }
        public string Name_Rus { get; set; }
        public string Name_Arm { get; set; }

        public string MainImg_LargeLink { get; set; }
        public string MainImg_MediumLink { get; set; }
        public string MainImg_SmallLink { get; set; }

        public string Type { get; set; }
    }
}
