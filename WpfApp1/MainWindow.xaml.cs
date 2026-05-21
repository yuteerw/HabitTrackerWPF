using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Win32;
using Newtonsoft.Json;
using HabitTrackerWPF.Models;

namespace HabitTrackerWPF
{
    public partial class MainWindow : Window
    {
        // Данные пользователя и привычек
        private UserProfile currentUser = new UserProfile();
        private ObservableCollection<Habit> habits = new ObservableCollection<Habit>();
        private ObservableCollection<StatItem> statistics = new ObservableCollection<StatItem>();

        // Путь к файлу для автосохранения
        private string autoSavePath = "habittracker_autosave.json";

        public class Habit
        {
            public string Name { get; set; } = "";
            public string Time { get; set; } = "";
            public bool IsDone { get; set; } = false;
        }

        public class StatItem
        {
            public string Date { get; set; } = "";
            public string HabitName { get; set; } = "";
            public string Status { get; set; } = "";
        }

        public MainWindow()
        {
            InitializeComponent();

            // Устанавливаем текущую дату в статусбаре
            CurrentDateTextBlock.Text = DateTime.Now.ToShortDateString();

            // Подписываемся на события
            ProductivitySlider.ValueChanged += ProductivitySlider_ValueChanged;
            SatisfactionSlider.ValueChanged += (s, e) => UpdateProgressBar();
            ProductivitySlider.ValueChanged += (s, e) => UpdateProgressBar();

            // Инициализация данных
            HabitsDataGrid.ItemsSource = habits;
            StatisticsListView.ItemsSource = statistics;

            InitializeTestData();
            SetupTreeView();
            InitializeStatisticsData();

            // Автозагрузка при запуске
            AutoLoad();

            UpdateStatus("Приложение загружено");
        }

        /// <summary>
        /// Инициализация тестовых данных
        /// </summary>
        private void InitializeTestData()
        {
            habits.Add(new Habit { Name = "Утренняя зарядка", Time = "08:00", IsDone = false });
            habits.Add(new Habit { Name = "Чтение книги", Time = "20:00", IsDone = false });
            habits.Add(new Habit { Name = "Медитация", Time = "21:00", IsDone = true });
        }

        /// <summary>
        /// Настройка TreeView
        /// </summary>
        private void SetupTreeView()
        {
            var root = new TreeViewItem { Header = "📁 Привычки", IsExpanded = true };
            var health = new TreeViewItem { Header = "❤️ Здоровье" };
            health.Items.Add(new TreeViewItem { Header = "Зарядка" });
            health.Items.Add(new TreeViewItem { Header = "Медитация" });
            var productivity = new TreeViewItem { Header = "⚡ Продуктивность" };
            productivity.Items.Add(new TreeViewItem { Header = "Чтение" });
            root.Items.Add(health);
            root.Items.Add(productivity);
            CategoriesTreeView.Items.Add(root);

            CategoriesTreeView.SelectedItemChanged += (s, e) =>
            {
                if (e.NewValue is TreeViewItem item)
                    UpdateStatus($"Выбрано: {item.Header}");
            };
        }

        /// <summary>
        /// Тестовые данные для статистики
        /// </summary>
        private void InitializeStatisticsData()
        {
            statistics.Add(new StatItem { Date = "20.05", HabitName = "Зарядка", Status = "✅" });
            statistics.Add(new StatItem { Date = "19.05", HabitName = "Чтение", Status = "✅" });
            statistics.Add(new StatItem { Date = "18.05", HabitName = "Медитация", Status = "❌" });
        }

        private void UpdateProgressBar()
        {
            DayProgressBar.Value = (ProductivitySlider.Value + SatisfactionSlider.Value) / 2;
        }

        private void ProductivitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateProgressBar();
            UpdateStatus($"Продуктивность: {ProductivitySlider.Value:F0}%");
        }

        // ========== ЛИЧНЫЕ ДАННЫЕ ==========

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

            if (EducationComboBox.SelectedItem is ComboBoxItem edu)
                currentUser.Education = edu.Content.ToString();

            currentUser.Hobbies = HobbiesListBox.SelectedItems.Cast<ListBoxItem>().Select(x => x.Content.ToString()).ToList();
            currentUser.ReceiveNotifications = NotificationsCheckBox.IsChecked == true;
            currentUser.PublicStats = PublicStatsCheckBox.IsChecked == true;
            currentUser.AutoSave = AutoSaveCheckBox.IsChecked == true;

            if (LowActivity.IsChecked == true) currentUser.ActivityLevel = "Низкий";
            else if (MediumActivity.IsChecked == true) currentUser.ActivityLevel = "Средний";
            else if (HighActivity.IsChecked == true) currentUser.ActivityLevel = "Высокий";

            UpdateStatus($"Профиль сохранён: {currentUser.FirstName}");
            MessageBox.Show("Профиль сохранён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            if (currentUser.AutoSave) SaveAllToFile(autoSavePath);
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

        // ========== ПРИВЫЧКИ ==========

        private void AddHabit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewHabitTextBox.Text))
            {
                MessageBox.Show("Введите название привычки!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            habits.Add(new Habit
            {
                Name = NewHabitTextBox.Text.Trim(),
                Time = string.IsNullOrWhiteSpace(NewHabitTimeTextBox.Text) ? "12:00" : NewHabitTimeTextBox.Text,
                IsDone = false
            });

            NewHabitTextBox.Clear();
            NewHabitTimeTextBox.Text = "12:00";
            UpdateStatus("Привычка добавлена");

            if (currentUser.AutoSave) SaveAllToFile(autoSavePath);
        }

        // ========== СОХРАНЕНИЕ И ЗАГРУЗКА (ГЛАВНОЕ В ЭТОЙ ВЕТКЕ) ==========

        /// <summary>
        /// Класс-контейнер для сохранения всех данных
        /// </summary>
        [Serializable]
        public class SaveData
        {
            public UserProfile User { get; set; }
            public List<Habit> Habits { get; set; }
            public string AvatarPath { get; set; }
        }

        /// <summary>
        /// Сохранение всех данных в JSON
        /// </summary>
        private void SaveAllToFile(string filePath)
        {
            try
            {
                var saveData = new SaveData
                {
                    User = currentUser,
                    Habits = habits.ToList(),
                    AvatarPath = (AvatarImage.Source as BitmapImage)?.UriSource?.LocalPath ?? ""
                };

                string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
                File.WriteAllText(filePath, json);
                UpdateStatus($"Сохранено в {Path.GetFileName(filePath)}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Загрузка всех данных из JSON
        /// </summary>
        private void LoadAllFromFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    UpdateStatus("Файл не найден");
                    return;
                }

                string json = File.ReadAllText(filePath);
                var saveData = JsonConvert.DeserializeObject<SaveData>(json);

                if (saveData == null) return;

                // Загружаем пользователя
                if (saveData.User != null)
                {
                    currentUser.CopyFrom(saveData.User);

                    // Заполняем форму
                    FirstNameTextBox.Text = currentUser.FirstName;
                    LastNameTextBox.Text = currentUser.LastName;
                    PasswordBox.Password = currentUser.Password;
                    BirthDatePicker.SelectedDate = currentUser.BirthDate;

                    // Образование
                    for (int i = 0; i < EducationComboBox.Items.Count; i++)
                    {
                        if ((EducationComboBox.Items[i] as ComboBoxItem)?.Content.ToString() == currentUser.Education)
                        {
                            EducationComboBox.SelectedIndex = i;
                            break;
                        }
                    }

                    // Хобби
                    HobbiesListBox.UnselectAll();
                    foreach (ListBoxItem item in HobbiesListBox.Items)
                    {
                        if (currentUser.Hobbies.Contains(item.Content.ToString()))
                            item.IsSelected = true;
                    }

                    NotificationsCheckBox.IsChecked = currentUser.ReceiveNotifications;
                    PublicStatsCheckBox.IsChecked = currentUser.PublicStats;
                    AutoSaveCheckBox.IsChecked = currentUser.AutoSave;

                    if (currentUser.ActivityLevel == "Низкий") LowActivity.IsChecked = true;
                    else if (currentUser.ActivityLevel == "Высокий") HighActivity.IsChecked = true;
                    else MediumActivity.IsChecked = true;
                }

                // Загружаем привычки
                if (saveData.Habits != null)
                {
                    habits.Clear();
                    foreach (var h in saveData.Habits)
                        habits.Add(h);
                }

                // Загружаем аватар
                if (!string.IsNullOrEmpty(saveData.AvatarPath) && File.Exists(saveData.AvatarPath))
                {
                    try
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(saveData.AvatarPath);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        AvatarImage.Source = bitmap;
                    }
                    catch { }
                }

                UpdateStatus($"Загружено из {Path.GetFileName(filePath)}");
                MessageBox.Show("Данные успешно загружены!", "Загрузка", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Автозагрузка при запуске
        /// </summary>
        private void AutoLoad()
        {
            if (File.Exists(autoSavePath))
                LoadAllFromFile(autoSavePath);
        }

        /// <summary>
        /// Сохранение (меню и тулбар)
        /// </summary>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "JSON файлы (*.json)|*.json",
                DefaultExt = "json",
                FileName = "habittracker_data.json"
            };
            if (dialog.ShowDialog() == true)
                SaveAllToFile(dialog.FileName);
        }

        /// <summary>
        /// Загрузка (меню и тулбар)
        /// </summary>
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "JSON файлы (*.json)|*.json"
            };
            if (dialog.ShowDialog() == true)
                LoadAllFromFile(dialog.FileName);
        }

        // ========== МЕНЮ И ТЕМЫ ==========

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser.AutoSave) SaveAllToFile(autoSavePath);
            Close();
        }

        private void LightTheme_Click(object sender, RoutedEventArgs e)
        {
            Background = new SolidColorBrush(Colors.White);
            UpdateStatus("Светлая тема");
        }

        private void DarkTheme_Click(object sender, RoutedEventArgs e)
        {
            Background = new SolidColorBrush(Color.FromRgb(50, 50, 50));
            UpdateStatus("Тёмная тема");
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Habit Tracker v1.0\nWPF Лабораторная работа №1\n\nФункции:\n- Личные данные с валидацией\n- Управление привычками\n- Статистика\n- Сохранение/загрузка JSON", "О программе");
        }

        // ========== TOGGLEBUTTON И REPEATBUTTON ==========

        private void ToggleExtraPanel_Click(object sender, RoutedEventArgs e)
        {
            if (ExtraPanel.Visibility == Visibility.Visible)
            {
                ExtraPanel.Visibility = Visibility.Collapsed;
                RightColumn.Width = new GridLength(0);
                ToggleExtraPanelButton.Content = "📁 Показать панель";
                UpdateStatus("Панель статистики скрыта");
            }
            else
            {
                ExtraPanel.Visibility = Visibility.Visible;
                RightColumn.Width = new GridLength(1.5, GridUnitType.Star);
                ToggleExtraPanelButton.Content = "📁 Скрыть панель";
                UpdateStatus("Панель статистики показана");
            }
        }

        private void IncreaseProductivity_Click(object sender, RoutedEventArgs e)
        {
            ProductivitySlider.Value = Math.Min(100, ProductivitySlider.Value + 5);
        }

        private void DecreaseProductivity_Click(object sender, RoutedEventArgs e)
        {
            ProductivitySlider.Value = Math.Max(0, ProductivitySlider.Value - 5);
        }

        // ========== АВАТАР ==========

        private void LoadAvatar_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Выберите аватар"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(dialog.FileName);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    AvatarImage.Source = bitmap;
                    UpdateStatus("Аватар загружен");

                    if (currentUser.AutoSave) SaveAllToFile(autoSavePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void UpdateStatus(string message)
        {
            StatusTextBlock.Text = $"{DateTime.Now.ToShortTimeString()} - {message}";
        }
    }
}