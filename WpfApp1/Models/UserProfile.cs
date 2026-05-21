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

        /// <summary>
        /// Копирование данных из другого профиля (для загрузки)
        /// </summary>
        public void CopyFrom(UserProfile other)
        {
            if (other == null) return;
            FirstName = other.FirstName;
            LastName = other.LastName;
            Password = other.Password;
            BirthDate = other.BirthDate;
            Education = other.Education;
            Hobbies = new List<string>(other.Hobbies);
            ReceiveNotifications = other.ReceiveNotifications;
            PublicStats = other.PublicStats;
            AutoSave = other.AutoSave;
            ActivityLevel = other.ActivityLevel;
        }
    }
}