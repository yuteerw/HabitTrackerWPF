using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HabitTrackerWPF.Models;

namespace HabitTrackerWPF
{
    public partial class MainWindow : Window
    {
        // Объект для хранения профиля пользователя
        private UserProfile currentUser = new UserProfile();

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Сохранение данных из формы в объект UserProfile с валидацией
        /// </summary>
        private void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            // Валидация: имя не пустое
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                MessageBox.Show("Имя не может быть пустым!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Валидация: фамилия не пустая
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
            {
                MessageBox.Show("Фамилия не может быть пустой!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Валидация: дата рождения не в будущем
            if (BirthDatePicker.SelectedDate > DateTime.Now)
            {
                MessageBox.Show("Дата рождения не может быть в будущем!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Сохранение данных из формы в объект
            currentUser.FirstName = FirstNameTextBox.Text;
            currentUser.LastName = LastNameTextBox.Text;
            currentUser.Password = PasswordBox.Password;
            currentUser.BirthDate = BirthDatePicker.SelectedDate ?? DateTime.Now;

            // Образование
            if (EducationComboBox.SelectedItem is ComboBoxItem selectedEdu)
                currentUser.Education = selectedEdu.Content.ToString();

            // Хобби (множественный выбор)
            currentUser.Hobbies = HobbiesListBox.SelectedItems
                .Cast<ListBoxItem>()
                .Select(x => x.Content.ToString())
                .ToList();

            // Чекбоксы
            currentUser.ReceiveNotifications = NotificationsCheckBox.IsChecked == true;
            currentUser.PublicStats = PublicStatsCheckBox.IsChecked == true;
            currentUser.AutoSave = AutoSaveCheckBox.IsChecked == true;

            // Уровень активности (RadioButton)
            if (LowActivity.IsChecked == true)
                currentUser.ActivityLevel = "Низкий";
            else if (MediumActivity.IsChecked == true)
                currentUser.ActivityLevel = "Средний";
            else if (HighActivity.IsChecked == true)
                currentUser.ActivityLevel = "Высокий";
            // Обновление статуса
            StatusTextBlock.Text = $"✅ Профиль сохранён: {currentUser.FirstName} {currentUser.LastName}";

            MessageBox.Show("Профиль успешно сохранён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Сброс всех полей ввода
        /// </summary>
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

            StatusTextBlock.Text = "🔄 Форма сброшена";
        }
    }
}