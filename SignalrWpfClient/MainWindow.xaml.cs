using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Windows;

namespace SignalrWpfClient
{
    public partial class MainWindow : Window
    {
        HubConnection connection;  // подключение для взаимодействия с хабом
        public MainWindow()
        {
            InitializeComponent();

            // создаем подключение к хабу
            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/chat")
                .WithAutomaticReconnect()   // автопереподключение
                .Build();


            // регистрируем функцию Receive для получения данных
            connection.On<string, string>("Receive", (user, message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{user}: {message}";
                    chatbox.Items.Insert(0, newMessage);
                });
            });
        }

        // обработчик загрузки окна
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // подключемся к хабу
                await connection.StartAsync();
                chatbox.Items.Add("Вы вошли в чат");
                sendBtn.IsEnabled = true;
            }
            catch (Exception ex)
            {
                chatbox.Items.Add(ex.Message);
            }
        }
        // обработчик нажатия на кнопку
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // отправка сообщения
                await connection.InvokeAsync("Send", userTextBox.Text, messageTextBox.Text);
            }
            catch (Exception ex)
            {
                chatbox.Items.Add(ex.Message);
            }
        }
    }
}
