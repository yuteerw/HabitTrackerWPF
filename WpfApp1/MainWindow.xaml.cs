using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Newtonsoft.Json;
using HabitTrackerWPF.Models;

namespace HabitTrackerWPF
{
    public partial class MainWindow : Window
    {
        private UserProfile currentUser = new UserProfile();
        private ObservableCollection<Habit> habits = new ObservableCollection<Habit>();

        public class Habit
        {
            public string Name { get; set; } = "";
            public string Time { get; set; } = "";
            public bool IsDone { get; set; } = false;
        }

        public MainWindow()
        {
            InitializeComponent();
            InitializeHabitsData();
            Loaded += MainWindow_Loaded;
            CurrentDateTextBlock.Text = DateTime.Now.ToShortDateString();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            HabitsDataGrid.ItemsSource = habits;
            SetupTreeView();
            UpdateStatusBar("Приложение загружено");
        }

        private void InitializeHabitsData()
        {
            habits.Add(new Habit { Name = "Утренняя зарядка", Time = "08:00", IsDone = false });
            habits.Add(new Habit { Name = "Чтение книги", Time = "20:00", IsDone = false });
            habits.Add(new Habit { Name = "Медитация", Time = "21:00", IsDone = true });
        }

        private void SetupTreeView()
        {
            var root = new TreeViewItem { Header = "Привычки", IsExpanded = true };
            var health = new TreeViewItem { Header = "Здоровье" };
            health.Items.Add(new TreeViewItem { Header = "Зарядка" });
            health.Items.Add(new TreeViewItem { Header = "Бег" });
            var productivity = new TreeViewItem { Header = "Продуктивность" };
            productivity.Items.Add(new TreeViewItem { Header = "Планирование" });
            root.Items.Add(health);
            root.Items.Add(productivity);
            CategoriesTreeView.Items.Add(root);
        }

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

            currentUser.Hobbies = HobbiesListBox.SelectedItems.Cast<ListBoxItem>().Select(x => x.Content.ToString()).ToList();
            currentUser.ReceiveNotifications = NotificationsCheckBox.IsChecked == true;
            currentUser.PublicStats = PublicStatsCheckBox.IsChecked == true;
            currentUser.AutoSave = AutoSaveCheckBox.IsChecked == true;

            if (LowActivity.IsChecked == true) currentUser.ActivityLevel = "Низкий";
            else if (MediumActivity.IsChecked == true) currentUser.ActivityLevel = "Средний";
            else if (HighActivity.IsChecked == true) currentUser.ActivityLevel = "Высокий";

            UpdateStatusBar($"Профиль сохранён: {currentUser.FirstName}");
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
            UpdateStatusBar("Форма сброшена");
        }

        private void AddHabit_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NewHabitTextBox.Text))
            {
                habits.Add(new Habit { Name = NewHabitTextBox.Text, Time = "12:00", IsDone = false });
                NewHabitTextBox.Clear();
                UpdateStatusBar("Привычка добавлена");
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog { Filter = "JSON файлы (*.json)|*.json", DefaultExt = "json" };
            if (saveDialog.ShowDialog() == true)
            {
                var data = new { User = currentUser, Habits = habits };
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(saveDialog.FileName, json);
                UpdateStatusBar($"Сохранено в {saveDialog.FileName}");
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog { Filter = "JSON файлы (*.json)|*.json" };
            if (openDialog.ShowDialog() == true)
            {
                string json = File.ReadAllText(openDialog.FileName);
                dynamic data = JsonConvert.DeserializeObject(json);
                UpdateStatusBar($"Загружено из {openDialog.FileName}");
                MessageBox.Show("Данные загружены", "Загрузка");
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e) => Close();

        private void LightTheme_Click(object sender, RoutedEventArgs e)
        {
            var whiteBrush = new SolidColorBrush(Colors.White);
            Background = whiteBrush;
            UpdateStatusBar("Светлая тема");
        }

        private void DarkTheme_Click(object sender, RoutedEventArgs e)
        {
            var darkBrush = new SolidColorBrush(Color.FromRgb(50, 50, 50));
            Background = darkBrush;
            UpdateStatusBar("Тёмная тема");
        }

        private void About_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("Habit Tracker v1.0\nWPF Lab Work #1", "О программе");

        private void ToggleExtraPanel_Click(object sender, RoutedEventArgs e)
        {
            if (ExtraPanel.Visibility == Visibility.Visible)
            {
                ExtraPanel.Visibility = Visibility.Collapsed;
                RightColumn.Width = new GridLength(0);
                UpdateStatusBar("Доп. панель скрыта");
            }
            else
            {
                ExtraPanel.Visibility = Visibility.Visible;
                RightColumn.Width = new GridLength(2, GridUnitType.Star);
                UpdateStatusBar("Доп. панель показана");
            }
        }

        private void LoadAvatar_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "Изображения|*.jpg;*.png;*.bmp" };
            if (dialog.ShowDialog() == true)
            {
                var bitmap = new BitmapImage(new Uri(dialog.FileName));
                AvatarImage.Source = bitmap;
                UpdateStatusBar("Аватар загружен");
            }
        }

        private void IncreaseProductivity_Click(object sender, RoutedEventArgs e) =>
            ProductivitySlider.Value = Math.Min(100, ProductivitySlider.Value + 5);

        private void DecreaseProductivity_Click(object sender, RoutedEventArgs e) =>
            ProductivitySlider.Value = Math.Max(0, ProductivitySlider.Value - 5);

        private void UpdateStatusBar(string message)
        {
            StatusTextBlock.Text = $"{DateTime.Now.ToShortTimeString()} - {message}";
        }
    }
}
 