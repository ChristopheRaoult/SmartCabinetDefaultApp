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
using System.Windows.Threading;

namespace SmartDrawerWpfApp.View.UC
{
    /// <summary>
    /// Logique d'interaction pour TextDefil.xaml
    /// </summary>
    public partial class TextDefil : UserControl
    {
        // Timer and direction variables
        private DispatcherTimer _timer;
        private int _delta = 20;

        public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("myText", typeof(String), typeof(TextDefil));

        public String myText
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value);  }
        }

        public TextDefil()
        {
            InitializeComponent();

            // Create timer to tick every 10ms
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };

            // Attach a Tick handler to do the scrolling
            _timer.Tick += delegate
            {

                // Flip direction if necessary
                if ((0 < Marquee.ViewportWidth) &&  // (NOOP if not visible yet)
                    ((Marquee.ScrollableWidth <= Marquee.HorizontalOffset) ||
                     (Marquee.HorizontalOffset <= 0)))
                {
                    _delta = -_delta; 
                    
                }
                                
                // Adjust the horizontal offset
                Marquee.ScrollToHorizontalOffset(Marquee.HorizontalOffset + _delta);

            };

            // Start the timer
            _timer.Start();
        }
       
    }
}
