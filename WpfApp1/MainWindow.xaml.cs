using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace HabitTrackerWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Устанавливаем текущую дату в статусбаре
            CurrentDateTextBlock.Text = DateTime.Now.ToShortDateString();

            // Подписываемся на изменение слайдера для отображения значения
            ProductivitySlider.ValueChanged += ProductivitySlider_ValueChanged;

            // Обновляем статус
            UpdateStatus("Приложение загружено");
        }

        /// <summary>
        /// Отображение текущего значения слайдера
        /// </summary>
        private void ProductivitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ProductivityValueTextBlock.Text = $"{ProductivitySlider.Value:F0}%";
            UpdateStatus($"Продуктивность изменена на {ProductivitySlider.Value:F0}%");
        }

        /// <summary>
        /// Сохранение (заглушка — будет в ветке save-load)
        /// </summary>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Сохранение (будет реализовано в следующей ветке)");
            MessageBox.Show("Функция сохранения будет добавлена в ветке save-load", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Загрузка (заглушка — будет в ветке save-load)
        /// </summary>
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Загрузка (будет реализовано в следующей ветке)");
            MessageBox.Show("Функция загрузки будет добавлена в ветке save-load", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Выход из приложения
        /// </summary>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Переключение на светлую тему
        /// </summary>
        private void LightTheme_Click(object sender, RoutedEventArgs e)
        {
            Background = new SolidColorBrush(Colors.White);
            UpdateStatus("Включена светлая тема");
        }

        /// <summary>
        /// Переключение на тёмную тему
        /// </summary>
        private void DarkTheme_Click(object sender, RoutedEventArgs e)
        {
            Background = new SolidColorBrush(Color.FromRgb(50, 50, 50));
            UpdateStatus("Включена тёмная тема");
        }

        /// <summary>
        /// Информация о программе
        /// </summary>
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Habit Tracker v1.0\nWPF Лабораторная работа №1\nРазработано в рамках учебного задания", "О программе", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Скрытие/показ правой панели (ToggleButton)
        /// </summary>
        private void ToggleExtraPanel_Click(object sender, RoutedEventArgs e)
        {
            if (ExtraPanel.Visibility == Visibility.Visible)
            {
                ExtraPanel.Visibility = Visibility.Collapsed;
                RightColumn.Width = new GridLength(0);
                ToggleExtraPanelButton.Content = "📁 Показать панель";
                UpdateStatus("Дополнительная панель скрыта");
            }
            else
            {
                ExtraPanel.Visibility = Visibility.Visible;
                RightColumn.Width = new GridLength(1.5, GridUnitType.Star);
                ToggleExtraPanelButton.Content = "📁 Скрыть панель";
                UpdateStatus("Дополнительная панель показана");
            }
        }

        /// <summary>
        /// Загрузка аватара пользователя через OpenFileDialog
        /// </summary>
        private void LoadAvatar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            dialog.Title = "Выберите аватар пользователя";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(dialog.FileName);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    AvatarImage.Source = bitmap;
                    UpdateStatus($"Аватар загружен: {System.IO.Path.GetFileName(dialog.FileName)}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Увеличение продуктивности (RepeatButton)
        /// </summary>
        private void IncreaseProductivity_Click(object sender, RoutedEventArgs e)
        {
            if (ProductivitySlider.Value < 100)
            {
                ProductivitySlider.Value = Math.Min(100, ProductivitySlider.Value + 5);
            }
        }

        /// <summary>
        /// Уменьшение продуктивности (RepeatButton)
        /// </summary>
        private void DecreaseProductivity_Click(object sender, RoutedEventArgs e)
        {
            if (ProductivitySlider.Value > 0)
            {
                ProductivitySlider.Value = Math.Max(0, ProductivitySlider.Value - 5);
            }
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