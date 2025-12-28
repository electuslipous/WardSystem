using Blazored.LocalStorage;
using System.Threading.Tasks;
using WardSystem.Models;

namespace WardSystem.Services
{
    public class WardService
    {
        public List<Room> Ward { get; private set; } = new();
        public string DailyDoctor { get; set; } = ""; // Osztályos Orvos
        public string OnCallDoctor { get; set; } = ""; // Ügyeletes Orvos

        // Keep the "Printing" doctor name separate if you wish, 
        // or map it to DailyDoctor. For now, let's keep them distinct.
        public string CurrentUserDoctorName { get; set; } = "";
        public WardService() {
            InitWard();
        }
        private void InitWard() {
            for (int i = 1; i <= 12; i++)
            {
                RoomType type = i switch
                {
                    <= 5 => RoomType.Quad,
                    11 => RoomType.Single,
                    _ => RoomType.Double,
                };
                Ward.Add(new Room { Number = i, Type = type });
            }
        }

        public async Task SaveWardData(ILocalStorageService localStorage)
        {
            await localStorage.SetItemAsync("wardData", Ward);
            // SAVE THE NEW STAFF INFO
            await localStorage.SetItemAsync("dailyDoctor", DailyDoctor);
            await localStorage.SetItemAsync("onCallDoctor", OnCallDoctor);
        }

        public async Task LoadWardData(ILocalStorageService localStorage)
        {
            var savedWard = await localStorage.GetItemAsync<List<Room>>("wardData");
            if (savedWard != null && savedWard.Count > 0) Ward = savedWard;

            var savedDaily = await localStorage.GetItemAsync<string>("dailyDoctor");
            if (!string.IsNullOrEmpty(savedDaily)) DailyDoctor = savedDaily;

            var savedOnCall = await localStorage.GetItemAsync<string>("onCallDoctor");
            if (!string.IsNullOrEmpty(savedOnCall)) OnCallDoctor = savedOnCall;
        }

        //public async Task AddPatientToBed(int roomNumber, int bedIndex, Patient patient)
        //{
        //    var room = Ward.FirstOrDefault(r => r.Number == roomNumber);
        //    if (room != null && bedIndex < room.Beds)
        //    {
        //        // Ensure the list is large enough to reach this index
        //        while (room.Patients.Count <= bedIndex)
        //        {
        //            room.Patients.Add(null);
        //        }
        //        room.Patients[bedIndex] = patient;
        //    }

        //}
        public async Task AddPatientToBed(int roomNumber, int bedIndex, Patient patient, ILocalStorageService localStorage)
        {
            var room = Ward.FirstOrDefault(r => r.Number == roomNumber);

            if (room != null && bedIndex < room.Beds)
            {
                // 1. MEMORY UPDATE (Your existing logic)
                while (room.Patients.Count <= bedIndex)
                {
                    room.Patients.Add(null);
                }
                room.Patients[bedIndex] = patient;

                // 2. DISK UPDATE (The missing piece!)
                // This ensures the data is there when you load the Print Report
                await SaveWardData(localStorage);
            }
        }
        public void AddPatient(int roomNumber, Patient patient)
        {
            var room = Ward.FirstOrDefault(r => r.Number == roomNumber);
            if (room != null && !room.isFull)
            {
                room.Patients.Add(patient);
            }
        }
        public void RemovePatient(int roomNumber, Patient patient)
        {
            var room = Ward.FirstOrDefault(r => r.Number == roomNumber);
            room?.Patients.Remove(patient);
        }

        public async Task DischargePatient(int roomNumber, int bedIndex, ILocalStorageService localStorage)
        {
            var room = Ward.FirstOrDefault(r => r.Number == roomNumber);
            if (room != null && bedIndex < room.Patients.Count)
            {
                // 1. Set the bed to null (Empty) instead of removing it from the list
                // This keeps Bed 2 as Bed 2, even if it's empty.
                room.Patients[bedIndex] = null;

                // 2. Save to Disk immediately
                await SaveWardData(localStorage);
            }
        }
    }
}
