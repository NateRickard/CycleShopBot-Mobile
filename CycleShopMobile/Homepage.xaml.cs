using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace CycleShopMobile
{
    public partial class Homepage : TabbedPage
    {
        public Homepage()
        {
            InitializeComponent();
            this.Children.Add(new DashboardPage());
            this.Children.Add(new BotChatPage());
        }
    }
}
