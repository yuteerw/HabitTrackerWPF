using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HabitTrackerWPF
{
    public partial class MainWindow : Window
    {
        // Коллекция для ListView (статистика)
        private ObservableCollection<StatItem> statistics = new ObservableCollection<StatItem>();

        public MainWindow()
        {
            InitializeComponent();

            // Заполняем TreeView категориями привычек
            SetupTreeView();

            // Заполняем ListView тестовыми данными
            InitializeStatisticsData();

            // Привязываем коллекцию к ListView
            StatisticsListView.ItemsSource = statistics;

            // Подписываемся на события выбора в TreeView
            CategoriesTreeView.SelectedItemChanged += TreeView_SelectedItemChanged;

            // Подписываемся на событие выбора в ListView
            StatisticsListView.SelectionChanged += ListView_SelectionChanged;

            // Обновляем статус
            UpdateStatus("Вкладка Статистика загружена");
        }

        /// <summary>
        /// Класс для элементов статистики в ListView
        /// </summary>
        public class StatItem
        {
            public string Date { get; set; } = "";
            public string HabitName { get; set; } = "";
            public string Status { get; set; } = "";
        }

        /// <summary>
        /// Настройка TreeView с категориями привычек
        /// </summary>
        private void SetupTreeView()
        {
            // Корневой элемент
            TreeViewItem root = new TreeViewItem();
            root.Header = "📁 Все привычки";
            root.IsExpanded = true;

            // Категория: Здоровье
            TreeViewItem health = new TreeViewItem();
            health.Header = "❤️ Здоровье";
            health.Tag = "Здоровье";

            health.Items.Add(new TreeViewItem { Header = "🏃 Утренняя зарядка", Tag = "Утренняя зарядка" });
            health.Items.Add(new TreeViewItem { Header = "🧘 Медитация", Tag = "Медитация" });
            health.Items.Add(new TreeViewItem { Header = "🚶 Прогулка", Tag = "Прогулка" });

            // Категория: Развитие
            TreeViewItem development = new TreeViewItem();
            development.Header = "📚 Развитие";
            development.Tag = "Развитие";

            development.Items.Add(new TreeViewItem { Header = "📖 Чтение", Tag = "Чтение" });
            development.Items.Add(new TreeViewItem { Header = "💻 Программирование", Tag = "Программирование" });
            development.Items.Add(new TreeViewItem { Header = "🇬🇧 Английский язык", Tag = "Английский" });

            // Категория: Продуктивность
            TreeViewItem productivity = new TreeViewItem();
            productivity.Header = "⚡ Продуктивность";
            productivity.Tag = "Продуктивность";

            productivity.Items.Add(new TreeViewItem { Header = "✅ Планирование дня", Tag = "Планирование" });
            productivity.Items.Add(new TreeViewItem { Header = "⏰ Выполнение дедлайнов", Tag = "Дедлайны" });

            // Добавляем всё в корень
            root.Items.Add(health);
            root.Items.Add(development);
            root.Items.Add(productivity);

            CategoriesTreeView.Items.Add(root);
        }

        /// <summary>
        /// Заполнение ListView тестовыми данными
        /// </summary>
        private void InitializeStatisticsData()
        {
            statistics.Add(new StatItem { Date = "20.05", HabitName = "Утренняя зарядка", Status = "✅ Выполнено" });
            statistics.Add(new StatItem { Date = "20.05", HabitName = "Чтение", Status = "✅ Выполнено" });
            statistics.Add(new StatItem { Date = "20.05", HabitName = "Медитация", Status = "❌ Пропущено" });
            statistics.Add(new StatItem { Date = "19.05", HabitName = "Утренняя зарядка", Status = "✅ Выполнено" });
            statistics.Add(new StatItem { Date = "19.05", HabitName = "Чтение", Status = "❌ Пропущено" });
            statistics.Add(new StatItem { Date = "18.05", HabitName = "Прогулка", Status = "✅ Выполнено" });
            statistics.Add(new StatItem { Date = "18.05", HabitName = "Медитация", Status = "✅ Выполнено" });
            statistics.Add(new StatItem { Date = "17.05", HabitName = "Английский язык", Status = "✅ Выполнено" });
        }

        /// <summary>
        /// Обработчик выбора элемента в TreeView (обновляет статусную строку)
        /// </summary>
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeViewItem selectedItem)
            {
                string header = selectedItem.Header.ToString();
                string tag = selectedItem.Tag?.ToString() ?? "";

                if (selectedItem.Items.Count == 0)
                {
                    // Лист (конкретная привычка)
                    UpdateStatus($"Выбрана привычка: {header}");
                }
                else
                {
                    // Категория
                    UpdateStatus($"Выбрана категория: {header}");
                }
            }
        }

        /// <summary>
        /// Обработчик выбора элемента в ListView (обновляет статусную строку)
        /// </summary>
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatisticsListView.SelectedItem is StatItem selectedItem)
            {
                UpdateStatus($"Выбрана запись: {selectedItem.Date} - {selectedItem.HabitName} - {selectedItem.Status}");
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