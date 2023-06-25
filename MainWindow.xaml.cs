using CV19.Infrastructure;
using CV19.Models.Decanat;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using static CV19.Infrastructure.ScreenInformation;

namespace CV19
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CollectionViewSource_Filter( object sender, System.Windows.Data.FilterEventArgs e )
        {
            if (e.Item is not Group group)
            {
                return;
            }
            if (group.Name is null)
            {
                return;
            }

            var filter_text = GroupNameFilterText.Text;
            if (filter_text.Length == 0)
            {
                return;
            }

            if (group.Name.Contains( filter_text, System.StringComparison.OrdinalIgnoreCase ))
            {
                return;
            }
            if (group.Description != null && group.Description.Contains( filter_text, System.StringComparison.OrdinalIgnoreCase ))
            {
                return;
            }

            e.Accepted = false;
        }

        private void GroupNameFilterText_TextChanged( object sender, System.Windows.Controls.TextChangedEventArgs e )
        {

            var collection = (CollectionViewSource) ((System.Windows.Controls.TextBox) sender).FindResource( "GroupsCollection" );
            collection.View.Refresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}