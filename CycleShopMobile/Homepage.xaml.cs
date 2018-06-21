using Xamarin.Forms;

namespace CycleShopMobile
{
    public partial class Homepage : TabbedPage
    {
        public Homepage()
        {
            InitializeComponent();

            Children.Add(new DashboardPage());
            Children.Add(new BotChatPage());
        }
    }
}