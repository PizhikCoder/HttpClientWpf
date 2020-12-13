using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HTTP_WPF_Client_Project
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }


        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (Message.Text != null && Message.Text.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries).Length != 0)
            {
                ChatConnectionLogic.SendMessage(Convert.ToUInt32(lbChatState.Content), Message.Text);
            }
            Message.Text = "";
        }

        private void Message_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Message.Text != null && Message.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length != 0)
                {
                    ChatConnectionLogic.SendMessage(Convert.ToUInt32(lbChatState.Content), Message.Text);
                }
                Message.Text = "";
            }
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ChatConnectionLogic.Chats.Find(x=>x.ChatId == Convert.ToUInt32(lbChatState.Content)).isChatWorking)//Проверка, не был ли закрыт чат со стороны администратора, а не клиента
            {
                ChatConnectionLogic.onChatClosing(Convert.ToUInt32(lbChatState.Content));
            }
        }
    }
}
