using KFA.Vote_Search.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KFA.Vote_Search
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly VoTeBotContext _dbContext;
        public ObservableCollection<UserMessage> Messages { get; set; }

        public MainWindow(VoTeBotContext dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;
            Messages = new ObservableCollection<UserMessage>();
            LoadDataAsync();
            MessList.ItemsSource = Messages;
        }
        private async void LoadDataAsync()
        {
            try
            {
                // Асинхронно загружаем данные из БД
                var messages = await _dbContext.UserMessages.ToListAsync();
                Messages.Clear();
                foreach (var message in messages)
                {
                    Messages.Add(message);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }
    }
}