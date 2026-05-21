using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HabitTrackerWPF
{
    public partial class MainWindow : Window
    {
        // Коллекция привычек (автоматически обновляет DataGrid)
        private ObservableCollection<Habit> habits = new ObservableCollection<Habit>();

        public MainWindow()
        {
            InitializeComponent();

            // Привязываем коллекцию к DataGrid
            HabitsDataGrid.ItemsSource = habits;

            // Добавляем тестовые привычки
            InitializeHabitsData();

            // Привязываем ProgressBar к Slider'ам
            ProductivitySlider.ValueChanged += UpdateProgressBar;
            SatisfactionSlider.ValueChanged += UpdateProgressBar;

            // Подсвечиваем текущую дату в календаре
            HabitsCalendar.SelectedDate = DateTime.Now;
            HabitsCalendar.DisplayDate = DateTime.Now;

            // Обновляем статус
            UpdateStatus("Вкладка Привычки загружена");
        }

        /// <summary>
        /// Класс привычки для отображения в DataGrid
        /// </summary>
        public class Habit
        {
            public string Name { get; set; } = "";
            public string Time { get; set; } = "";
            public bool IsDone { get; set; } = false;
        }

        /// <summary>
        /// Добавление тестовых привычек
        /// </summary>
        private void InitializeHabitsData()
        {
            habits.Add(new Habit { Name = "Утренняя зарядка", Time = "08:00", IsDone = false });
            habits.Add(new Habit { Name = "Чтение 30 минут", Time = "20:00", IsDone = false });
            habits.Add(new Habit { Name = "Медитация", Time = "21:00", IsDone = true });
            habits.Add(new Habit { Name = "Прогулка на свежем воздухе", Time = "15:00", IsDone = false });
        }

        /// <summary>
        /// Обновление ProgressBar на основе среднего значения слайдеров
        /// </summary>
        private void UpdateProgressBar(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double average = (ProductivitySlider.Value + SatisfactionSlider.Value) / 2;
            DayProgressBar.Value = average;
            UpdateStatus($"Прогресс дня: {average:F0}%");
        }

        /// <summary>
        /// Добавление новой привычки
        /// </summary>
        private void AddHabit_Click(object sender, RoutedEventArgs e)
        {
            // Проверка: название не пустое
            if (string.IsNullOrWhiteSpace(NewHabitTextBox.Text))
            {
                MessageBox.Show("Введите название привычки!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка: время не пустое
            string time = NewHabitTimeTextBox.Text;
            if (string.IsNullOrWhiteSpace(time))
            {
                time = "12:00";
            }

            // Добавляем новую привычку
            habits.Add(new Habit
            {
                Name = NewHabitTextBox.Text.Trim(),
                Time = time,
                IsDone = false
            });

            // Очищаем поле ввода
            NewHabitTextBox.Clear();
            NewHabitTimeTextBox.Text = "12:00";

            // Обновляем статус
            UpdateStatus($"Добавлена привычка: {NewHabitTextBox.Text}");

            // Прокручиваем DataGrid к последнему элементу
            HabitsDataGrid.ScrollIntoView(habits[habits.Count - 1]);
        }

        /// <summary>
        /// Обновление строки состояния
        /// </summary>
        private void UpdateStatus(string message)
        {
            StatusTextBlock.Text = $"{DateTime.Now.ToShortTimeString()} - {message}";
        }
    }
}