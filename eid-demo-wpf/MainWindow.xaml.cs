using Egelke.Eid.Client;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace eid_demo_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Readers readers;

        public MainWindow()
        {
            InitializeComponent();
            readers = new Readers(ReaderScope.User);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EidCardView view = (EidCardView)Application.Current.FindResource("card");

            EidCard eid = (EidCard)readers.ListCards().Where(c => c is EidCard).FirstOrDefault();
            using (eid)
            {
                eid.Open();

                view.UpdatePicture(eid.Picture);
                view.Address = eid.Address;
                view.Identity = eid.Identity;
            }
        }
    }
}
