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

        private bool isLoading = true;

        private string wordFilter = string.Empty;
        public string WordFilter 
        { 
            get { return wordFilter; } 
            set 
            { 
                wordFilter = value;
                OnPropertyChanged();
                if (!isLoading)
                    FilterMessages();
            } 
        }
        private string langFilter = string.Empty;
        public string LangFilter
        { 
            get { return langFilter; }
            set
            {
                langFilter = value; 
                OnPropertyChanged();
                if( !isLoading)
                    FilterMessages();
            }
        }
        private DateTime? dateTimeFromFilter;
        public DateTime? DateTimeFromFilter
        {
            get { return dateTimeFromFilter; }
            set
            {
                if (dateTimeToFilter != value)
                {
                    dateTimeFromFilter = value;
                    OnPropertyChanged();
                    if (!isLoading)
                        FilterMessages();
                }
            }
        }
        private DateTime? dateTimeToFilter;
        public DateTime? DateTimeToFilter
        {
            get { return dateTimeToFilter; }
            set
            {
                if (dateTimeToFilter != value)
                {
                    dateTimeToFilter = value;
                    OnPropertyChanged();
                    if (!isLoading)
                        FilterMessages();
                }
            }
        }
        private DateTime? minDate;
        public DateTime? MinDate
        {
            get { return minDate; }
            set
            {
                if (minDate != value)
                { 
                    minDate = value;
                    OnPropertyChanged();
                }
            }
        }
        private DateTime? maxDate;
        public DateTime? MaxDate
        {
            get { return maxDate; }
            set
            {
                if (maxDate != value)
                {
                    maxDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private void FilterMessages()
        {
            if (messages == null || !messages.Any())
            {
                FilteredMessages = new ObservableCollection<UserMessage>(messages);
                return;
            }
            else
            {
                var filteredMess = messages.AsEnumerable();

                if (!string.IsNullOrEmpty(WordFilter))
                {
                    filteredMess = filteredMess.Where(m => m.Message != null &&
                                m.Message.Contains(WordFilter, StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrEmpty(LangFilter))
                {
                    filteredMess = filteredMess.Where(m => m.LanguageCode != null &&
                                m.LanguageCode.Contains(LangFilter, StringComparison.OrdinalIgnoreCase));
                }
                if (DateTimeFromFilter.HasValue)
                {
                    filteredMess = filteredMess.Where(m => m.MessageDate >= DateTimeFromFilter.Value.Date);
                }
                if (DateTimeToFilter.HasValue)
                {
                    filteredMess = filteredMess.Where(m => m.MessageDate < DateTimeToFilter.Value.Date);
                }
                    
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
            FilteredMessages = new ObservableCollection<UserMessage>(messages);
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            if(_dbContext == null ) 
                return;
            try
            {
                isLoading = true;
                // Асинхронно загружаем данные из БД
                var messages1 = await _dbContext.UserMessages.ToListAsync();
                Messages = new ObservableCollection<UserMessage>(messages1);

                SetDateRange();

                FilterMessages();

            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void SetDateRange()
        {
            if (Messages != null || Messages.Any())
            {
                MinDate = Messages.Min(x => x.MessageDate);
                MaxDate = Messages.Max(x => x.MessageDate);
                DateTimeFromFilter = MinDate;
                //добавляем 1 день, т.к. проверям на < Max
                DateTimeToFilter = MaxDate.Value.AddDays(1);

                OnPropertyChanged(nameof(DateTimeFromFilter));
                OnPropertyChanged(nameof(DateTimeToFilter));
                OnPropertyChanged(nameof(MinDate));
                OnPropertyChanged(nameof(MaxDate));
            }
            else
            {
                MinDate = DateTime.Today.AddMonths(-1);
                MaxDate= DateTime.Today;
                DateTimeFromFilter = MinDate;
                DateTimeToFilter = MaxDate;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
