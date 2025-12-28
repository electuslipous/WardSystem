namespace WardSystem.Models
{
    public enum RoomType
    {
        Single = 1,
        Double = 2,
        Quad = 4,
    }
    public class Room
    {
        public int Number { get; set; }
        public RoomType Type { get; set; }
        public int Beds => (int)Type;
        public List<Patient> Patients { get; set; } = new();
        public bool isFull => Patients.Count >= Beds;
    }
}
