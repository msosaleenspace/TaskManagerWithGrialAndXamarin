using System.Threading.Tasks;
using Xamarin.Forms;
using UXDivers.Grial;

namespace Eleos3
{
    public partial class RootFlyoutPage : FlyoutPage
    {
        public RootFlyoutPage()
        {
            InitializeComponent();

            Flyout = new MainMenuPage(LaunchPageInDetail);
        }

        private void LaunchPageInDetail(Page page)
        {
            Detail = page;
            IsPresented = false;
        }
    }
}