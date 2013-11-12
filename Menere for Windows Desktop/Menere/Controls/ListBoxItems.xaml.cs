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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Menere.Controls
{
    /// <summary>
    /// Interaction logic for ListBoxItems.xaml
    /// </summary>
    public partial class ListBoxItems : UserControl
    {
        public ListBoxItems()
        {
            InitializeComponent();
            if (AppController.active_external_services != null)
            {
                AppController.active_external_services.CollectionChanged += active_external_services_CollectionChanged;
            }
            update_context_menu();
        }

        private GridViewColumnHeader _CurSortCol = null;
        private SortAdorner _CurAdorner = null;

        private void update_context_menu()
        {
            ItemContextMenu item_context_menu = new ItemContextMenu();
            listview_items.ContextMenu = item_context_menu.get_context_menu(null);
        }

        private void active_external_services_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            update_context_menu();
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;
            String field = column.Tag as String;

            if (_CurSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(_CurSortCol).Remove(_CurAdorner);
                listview_items.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (_CurSortCol == column && _CurAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            _CurSortCol = column;
            _CurAdorner = new SortAdorner(_CurSortCol, newDir);
            AdornerLayer.GetAdornerLayer(_CurSortCol).Add(_CurAdorner);
            listview_items.Items.SortDescriptions.Add(new SortDescription(field, newDir));
        }

        public class SortAdorner : Adorner
        {
            private readonly static Geometry _AscGeometry =
                Geometry.Parse("M 0,0 L 10,0 L 5,5 Z");

            private readonly static Geometry _DescGeometry =
                Geometry.Parse("M 0,5 L 10,5 L 5,0 Z");

            public ListSortDirection Direction { get; private set; }

            public SortAdorner(UIElement element, ListSortDirection dir)
                : base(element)
            { Direction = dir; }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);

                if (AdornedElement.RenderSize.Width < 20)
                    return;

                drawingContext.PushTransform(
                    new TranslateTransform(
                      AdornedElement.RenderSize.Width - 15,
                      (AdornedElement.RenderSize.Height - 5) / 2));

                drawingContext.DrawGeometry(Brushes.Black, null,
                    Direction == ListSortDirection.Ascending ?
                      _AscGeometry : _DescGeometry);

                drawingContext.Pop();
            }
        }

        private void GridViewColumnHeader_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            /*Properties.Settings.Default.ListViewWidthTitle = ListViewColumnTitle.Width;
            Properties.Settings.Default.ListViewWidthTags = ListViewColumnTags.Width;
            Properties.Settings.Default.ListViewWidthAdded = ListViewColumnAdded.Width;
            Properties.Settings.Default.ListViewWidthUpdated = ListViewColumnUpdated.Width; */
        }

    }
}
