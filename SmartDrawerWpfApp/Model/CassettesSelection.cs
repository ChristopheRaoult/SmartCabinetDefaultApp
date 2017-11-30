using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.Model
{
    public class CassettesSelection : ViewModelBase
    {
        int cn1, cn2, cn3, cn4, cn5, cn6, cn7;

        public long RequestId { get; set; }
        public DateTime RequestTime { get; set; }
        public string UserName { get; set; }
        public string WallList { get; set; }
        public int CassetteSelectedNumber { get; set; }
        public int CassetteSelectionTotalNumber { get; set; }

        public string SeekReason { get; set; }

        public int CassetteDrawer1Number { get { return cn1; } set { cn1 = value; RaisePropertyChanged(() => CassetteDrawer1Number); } }
        public int CassetteDrawer2Number { get { return cn2; } set { cn2 = value; RaisePropertyChanged(() => CassetteDrawer2Number); } }
        public int CassetteDrawer3Number { get { return cn3; } set { cn3 = value; RaisePropertyChanged(() => CassetteDrawer3Number); } }
        public int CassetteDrawer4Number { get { return cn4; } set { cn4 = value; RaisePropertyChanged(() => CassetteDrawer4Number); } }
        public int CassetteDrawer5Number { get { return cn5; } set { cn5 = value; RaisePropertyChanged(() => CassetteDrawer5Number); } }
        public int CassetteDrawer6Number { get { return cn6; } set { cn6 = value; RaisePropertyChanged(() => CassetteDrawer6Number); } }
        public int CassetteDrawer7Number { get { return cn7; } set { cn7 = value; RaisePropertyChanged(() => CassetteDrawer7Number); } }
        public List<string>[] TagToLight = new List<string>[8];

        public List<string> ListControlNumber { get; set; }

    }
}
