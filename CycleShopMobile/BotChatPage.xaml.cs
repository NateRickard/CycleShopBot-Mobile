using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BotFramework;
using Plugin.Media;
using Xamarin.Forms;

namespace CycleShopMobile
{
    public partial class BotChatPage : ContentPage
    {
        public BotChatPage()
        {
            InitializeComponent();
            MessageList.HasUnevenRows = true;
            MessageList.ItemSelected += (sender, e) =>
            {
                ((ListView)sender).SelectedItem = null;
            };
        }

        ConversationManager currentConversation;

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            await Task.Delay(100);

            if (currentConversation == null)
            {
                await StartConversation();
            }
        }

        async void StartStopConversation(object sender, EventArgs e)
        {
            try
            {
                if (currentConversation == null)
                    await StartConversation();
                else
                    await EndConversation();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                foreach (DictionaryEntry m in ex.Data)
                    Debug.WriteLine($"{m.Key} - {m.Value}");
            }
        }

        async Task StartConversation()
        {
            try
            {
                currentConversation = await ConversationManager.StartConversation("Mobile_User", "<< YOUR DIRECTLINE CLIENT >>");
                currentConversation.CardActionTapped = HandleCardActionTapped;
                currentConversation.Conversation.Messages.CollectionChanged += Messages_CollectionChanged;
                MessageList.ItemsSource = currentConversation.Conversation.Messages;

                // once we have a conversation, enable our send msg UI
                SendButton.IsEnabled = true;
                Text.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error starting conversation: {ex.Message}");
            }
        }

        void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var lastItem = currentConversation.Conversation.Messages.LastOrDefault();

            if (lastItem != null)
                Device.BeginInvokeOnMainThread(() => MessageList.ScrollTo(lastItem, ScrollToPosition.Start, false));
        }

        void HandleCardActionTapped(CardAction action)
        {
            switch (action.Type)
            {
                case CardActionType.OpenUrl:
                    Device.OpenUri(new Uri(action.Value));
                    break;
                default:
                    Debug.WriteLine($"Unhandled tap: {action.Value}");
                    break;
            }
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = (BotActivity)e.Item;
        }

        async Task EndConversation()
        {
            await currentConversation.EndConversation();

            currentConversation.CardActionTapped = null;
            currentConversation = null;
            MessageList.ItemsSource = null;
        }

        async void SendMessage(object sender, EventArgs e)
        {
            try
            {
                if (currentConversation == null)
                {
                    await StartConversation();
                }

                if (string.IsNullOrWhiteSpace(Text.Text))
                    return;
                
                await currentConversation.SendMessage(Text.Text);

                Text.Text = string.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        async void AddPhoto(object sender, EventArgs e)
        {
            Task startTask = null;

            if (currentConversation == null)
            {
                startTask = StartConversation();
            }

            try
            {
                var photo = await CrossMedia.Current.PickPhotoAsync();

                if (!(startTask?.IsCompleted ?? true))
                    await startTask;

                var fileName = Path.GetFileName(photo.Path);
                var extension = Path.GetExtension(photo.Path).TrimStart('.');
                var type = $"image/{extension}";

                await currentConversation.UploadAttachment(fileName, type, photo.GetStream());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}