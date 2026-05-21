using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HabitTrackerWPF.Models;

namespace HabitTrackerWPF
{
    public partial class MainWindow : Window
    {
        // Из ветки profile-tab
        private UserProfile currentUser = new UserProfile();

        // Из ветки habits-tab
        private ObservableCollection<Habit> habits = new ObservableCollection<Habit>();

        public MainWindow()
        {
            InitializeComponent();

            // Инициализация привычек
            HabitsDataGrid.ItemsSource = habits;
            InitializeHabitsData();

            // Привязка ProgressBar к слайдерам
            ProductivitySlider.ValueChanged += UpdateProgressBar;
            SatisfactionSlider.ValueChanged += UpdateProgressBar;

            // Календарь
            HabitsCalendar.SelectedDate = DateTime.Now;
            HabitsCalendar.DisplayDate = DateTime.Now;

            UpdateStatus("Приложение загружено");
        }

        // ==================== ИЗ ВЕТКИ habits-tab ====================

        public class Habit
        {
            public string Name { get; set; } = "";
            public string Time { get; set; } = "";
            public bool IsDone { get; set; } = false;
        }

        private void InitializeHabitsData()
        {
            habits.Add(new Habit { Name = "Утренняя зарядка", Time = "08:00", IsDone = false });
            habits.Add(new Habit { Name = "Чтение 30 минут", Time = "20:00", IsDone = false });
            habits.Add(new Habit { Name = "Медитация", Time = "21:00", IsDone = true });
        }

        private void UpdateProgressBar(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double average = (ProductivitySlider.Value + SatisfactionSlider.Value) / 2;
            DayProgressBar.Value = average;
            UpdateStatus($"Прогресс дня: {average:F0}%");
        }

        private void AddHabit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewHabitTextBox.Text))
            {
                MessageBox.Show("Введите название привычки!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string time = NewHabitTimeTextBox.Text;
            if (string.IsNullOrWhiteSpace(time)) time = "12:00";

            habits.Add(new Habit
            {
                Name = NewHabitTextBox.Text.Trim(),
                Time = time,
                IsDone = false
            });

            NewHabitTextBox.Clear();
            NewHabitTimeTextBox.Text = "12:00";
            UpdateStatus($"Добавлена привычка");
            HabitsDataGrid.ScrollIntoView(habits[habits.Count - 1]);
        }

        // ==================== ИЗ ВЕТКИ profile-tab ====================

        private void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                MessageBox.Show("Имя не может быть пустым!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (BirthDatePicker.SelectedDate > DateTime.Now)
            {
                MessageBox.Show("Дата рождения не может быть в будущем!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            currentUser.FirstName = FirstNameTextBox.Text;
            currentUser.LastName = LastNameTextBox.Text;
            currentUser.Password = PasswordBox.Password;
            currentUser.BirthDate = BirthDatePicker.SelectedDate ?? DateTime.Now;

            if (EducationComboBox.SelectedItem is ComboBoxItem selectedEdu)
                currentUser.Education = selectedEdu.Content.ToString();

            currentUser.Hobbies = HobbiesListBox.SelectedItems
                .Cast<ListBoxItem>()
                .Select(x => x.Content.ToString())
                .ToList();

            currentUser.ReceiveNotifications = NotificationsCheckBox.IsChecked == true;
            currentUser.PublicStats = PublicStatsCheckBox.IsChecked == true;
            currentUser.AutoSave = AutoSaveCheckBox.IsChecked == true;

            if (LowActivity.IsChecked == true)
                currentUser.ActivityLevel = "Низкий";
            else if (MediumActivity.IsChecked == true)
                currentUser.ActivityLevel = "Средний";
            else if (HighActivity.IsChecked == true)
                currentUser.ActivityLevel = "Высокий";

            UpdateStatus($"Профиль сохранён: {currentUser.FirstName}");
            MessageBox.Show("Профиль сохранён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ResetProfile_Click(object sender, RoutedEventArgs e)
        {
            FirstNameTextBox.Text = "";
            LastNameTextBox.Text = "";
            PasswordBox.Password = "";
            BirthDatePicker.SelectedDate = DateTime.Now;
            EducationComboBox.SelectedIndex = 0;
            HobbiesListBox.UnselectAll();
            NotificationsCheckBox.IsChecked = false;
            PublicStatsCheckBox.IsChecked = false;
            AutoSaveCheckBox.IsChecked = true;
            MediumActivity.IsChecked = true;
            UpdateStatus("Форма сброшена");
        }

        // ==================== ОБЩИЕ МЕТОДЫ ====================

        private void UpdateStatus(string message)
        {
            StatusTextBlock.Text = $"{DateTime.Now.ToShortTimeString()} - {message}";
        }
    }
}
