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
        private ObservableCollection<UserMessage> _messages = new ObservableCollection<UserMessage>();

        public ObservableCollection<UserMessage> messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged();
            }
        }

        public MessageViewModel(VoTeBotContext dbContext)
        {
            _dbContext = dbContext;
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            if(_dbContext == null ) 
                return;
            try
            {
                // Асинхронно загружаем данные из БД
                var messages1 = await _dbContext.UserMessages.ToListAsync();
                messages.Clear();
                foreach (var message in messages1)
                {
                    messages.Add(message);
                }
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
