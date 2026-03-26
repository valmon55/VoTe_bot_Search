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
using System.Windows.Data;
using System.Windows.Interop;

namespace KFA.Vote_Search.ViewModel
{
    public class MessageViewModel : INotifyPropertyChanged
    {
        private readonly VoTeBotContext _dbContext;
        private ObservableCollection<UserMessage> messages;
        public ObservableCollection<UserMessage> Messages
        {
            get => messages;
            set 
            {
                messages = value;
                OnPropertyChanged();

                if (messages != null)
                {
                    filteredMessagesView = CollectionViewSource.GetDefaultView(messages);
                    filteredMessagesView.Filter = FilterMessages;
                    OnPropertyChanged(nameof(FilteredMessagesView));
                    if (!isLoading)
                        filteredMessagesView.Refresh();
                }
            }
        }
        private ICollectionView filteredMessagesView;
        public ICollectionView FilteredMessagesView
        {
            get => filteredMessagesView;
            set
            {
                filteredMessagesView = value;
                OnPropertyChanged();
            }
        } 

        private bool isLoading = true;

        private string wordFilter = string.Empty;
        public string WordFilter 
        { 
            get { return wordFilter; } 
            set 
            { 
                wordFilter = value;
                OnPropertyChanged();
                if (!isLoading && filteredMessagesView != null)
                    filteredMessagesView.Refresh();
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
                if( !isLoading && filteredMessagesView != null)
                    filteredMessagesView.Refresh(); 
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
                        filteredMessagesView.Refresh();
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
                        filteredMessagesView.Refresh(); 
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

        private bool FilterMessages(object obj)
        {
            if (obj is UserMessage message)
            {
                // TRUE - выводить, FALSE - нет
                bool filter = true;

                if (DateTimeFromFilter.HasValue && message.MessageDate < dateTimeFromFilter.Value)
                {
                    filter = false;
                }
                if (DateTimeToFilter.HasValue && message.MessageDate >= dateTimeToFilter.Value)
                {
                    return false;
                }
                if (!string.IsNullOrEmpty(LangFilter))
                {
                    if (string.IsNullOrEmpty(message.LanguageCode) ||
                        !message.LanguageCode.Contains(LangFilter, StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
                if (!string.IsNullOrEmpty(WordFilter))
                {
                    if (string.IsNullOrEmpty(message.Message) ||
                        !message.Message.Contains(WordFilter, StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }

                return filter;
            }
            return false;
        }
        public MessageViewModel(VoTeBotContext dbContext)
        {
            _dbContext = dbContext;
            Messages = new ObservableCollection<UserMessage>();
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

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Messages.Clear();
                    foreach(var msg in messages1)
                    {
                        Messages.Add(msg);
                    }

                    SetDateRange();

                    if (filteredMessagesView != null)
                    {
                        filteredMessagesView.Refresh();
                    }
                });

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
