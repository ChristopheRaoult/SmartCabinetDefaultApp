using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.ViewModel
{
    public class SelectionViewModel :  ViewModelBase
    {
        bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; RaisePropertyChanged(() => IsSelected); }
        }
        public int PullItemId { get; set; }
        public int ServerPullItemId { get; set; }
        public string PullItemDate { get; set; }
        public string Description { get; set; }
        public string User { get; set; }
        public int TotalToPull { get; set; }
        public int TotalToPullInDevice { get; set; }
        public List<string> lstTopull { get; set; }
        public List<string> lstTagpulled { get; set; }
    }
}
