using System;
using System.Collections.Generic;

namespace HabitTrackerWPF.Models
{
    /// <summary>
    /// Класс профиля пользователя для хранения личных данных
    /// </summary>
    public class UserProfile
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Password { get; set; } = "";
        public DateTime BirthDate { get; set; } = DateTime.Now;
        public string Education { get; set; } = "Среднее";
        public List<string> Hobbies { get; set; } = new List<string>();
        public bool ReceiveNotifications { get; set; } = false;
        public bool PublicStats { get; set; } = false;
        public bool AutoSave { get; set; } = true;
        public string ActivityLevel { get; set; } = "Средний";
    }
}