using KFA.Vote_Search.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KFA.Vote_Search.ViewModel
{
    public class MessageViewModel : INotifyPropertyChanged
    {
        private readonly VoTeBotContext _dbContext;
        private ObservableCollection<UserMessage> messages = new ObservableCollection<UserMessage>();

        private ObservableCollection<UserMessage> filteredMessages;

        private string wordFilter = string.Empty;
        public string WordFilter 
        { 
            get { return wordFilter; } 
            set 
            { 
                wordFilter = value;
                OnPropertyChanged();
                FilterMessages();
            } 
        }

        private void FilterMessages()
        {
            if(string.IsNullOrEmpty(WordFilter))
            {
                FilteredMessages = new ObservableCollection<UserMessage>(messages);
            }
            else
            {
                var filteredMess = messages.Where(m => m.Message.Contains(WordFilter, StringComparison.OrdinalIgnoreCase)).ToList();
                FilteredMessages = new ObservableCollection<UserMessage>(filteredMess);
            }
        }

        public ObservableCollection<UserMessage> Messages
        {
            get => messages;
            set
            {
                messages = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<UserMessage> FilteredMessages
        {
            get => filteredMessages;
            set
            {
                filteredMessages = value;
                OnPropertyChanged();
            }
        }

        public MessageViewModel(VoTeBotContext dbContext)
        {
            _dbContext = dbContext;
            LoadDataAsync();
            FilteredMessages = new ObservableCollection<UserMessage>(messages);
        }

        private async void LoadDataAsync()
        {
            if(_dbContext == null ) 
                return;
            try
            {
                // Асинхронно загружаем данные из БД
                var messages1 = await _dbContext.UserMessages.ToListAsync();
                Messages = new ObservableCollection<UserMessage>(messages1);
                FilteredMessages = new ObservableCollection<UserMessage>(messages1);
                //messages.Clear();
                //foreach (var message in messages1)
                //{
                //    messages.Add(message);
                //}
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        // Реализация INotifyPropertyChanged для уведомления UI об изменениях
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
