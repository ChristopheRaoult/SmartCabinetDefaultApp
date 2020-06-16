using GalaSoft.MvvmLight;
using SmartDrawerDatabase.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SmartDrawerWpfApp.Model
{
    public delegate void NotifycheckedDelegate();
    /*public class BaseObject : ViewModelBase
    {
        public Product _Productinfo;
        public int _drawerId;
        public string _AbstactValue;
        public string AbstactValue { get { return _AbstactValue; } set { _AbstactValue = value; RaisePropertyChanged(() => AbstactValue); } }

        private bool? _isChecked;
        private bool _isCheckboxVisible;
        private bool reentrancyCheck = false;


        public event NotifycheckedDelegate IsCheckedChanged;

        public BaseObject(Product Productinfo, int drawerId)
        {           
            this.Productinfo = Productinfo;
            this.drawerId = drawerId;
            IsChecked = false;
            IsCheckboxVisible = true;
        }
        public Product Productinfo { get { return _Productinfo; } set { _Productinfo = value; RaisePropertyChanged(() => Productinfo); } }
        public int drawerId { get { return _drawerId; } set { _drawerId = value; RaisePropertyChanged(() => drawerId); } }
        public bool IsCheckboxVisible
        {
            get { return _isCheckboxVisible; }
            set
            {
                this._isCheckboxVisible = value;
                RaisePropertyChanged(() => IsCheckboxVisible);
            }
        }
        public bool? IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                if (_isChecked != value)
                {
                    if (reentrancyCheck)
                        return;
                    reentrancyCheck = true;
                    _isChecked = value;
                    RaisePropertyChanged(() => IsChecked);
                    reentrancyCheck = false;
                    if (IsCheckedChanged != null)
                        IsCheckedChanged();
                }
            }
        }
    }*/

    public class ItemDetail : ViewModelBase
    {


        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                RaisePropertyChanged(() => IsSelected);

            }
        }
        private Visibility _IsEnabled;
        public Visibility IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                if (_IsEnabled == value) return;
                _IsEnabled = value;
                RaisePropertyChanged(() => IsEnabled);

            }
        }




        // -1 removed ; 0 present ; 1 added
        private int _ItemEventType;
        public int ItemEventType
        {
            get { return _ItemEventType; }
            set
            {
                if (_ItemEventType == value) return;
                _ItemEventType = value;
                RaisePropertyChanged(() => ItemEventType);
                RaisePropertyChanged(() => ItemEventColor);
            }
        }

        public SolidColorBrush ItemEventColor
        {
            get
            {
                switch (ItemEventType)
                {
                    case -1: return (SolidColorBrush)(new BrushConverter().ConvertFrom("#a62d3b"));
                    case 0: return (SolidColorBrush)(new BrushConverter().ConvertFrom("#4e8dcc"));
                    case 1: return (SolidColorBrush)(new BrushConverter().ConvertFrom("#58994d"));
                }
                return Brushes.White;
            }
        }



        private string _TagId;
        public string TagId
        {
            get { return _TagId; }
            set
            {
                if (_TagId == value) return;
                _TagId = value;
                RaisePropertyChanged(() => TagId);
            }
        }

        public int _DrawerId;
        public int DrawerId { get { return _DrawerId; } set { _DrawerId = value; RaisePropertyChanged(() => DrawerId); } }


        private string _valColumn1;
        public string valColumn1
        {
            get { return _valColumn1; }
            set
            {
                if (_valColumn1 == value) return;
                _valColumn1 = value;
                RaisePropertyChanged(() => valColumn1);
            }
        }

        private string _valColumn2;
        public string valColumn2
        {
            get { return _valColumn2; }
            set
            {
                if (_valColumn2 == value) return;
                _valColumn2 = value;
                RaisePropertyChanged(() => valColumn2);
            }
        }
        private string _valColumn3;
        public string valColumn3
        {
            get { return _valColumn3; }
            set
            {
                if (_valColumn3 == value) return;
                _valColumn3 = value;
                RaisePropertyChanged(() => valColumn3);
            }
        }

        private string _valColumn4;
        public string valColumn4
        {
            get { return _valColumn4; }
            set
            {
                if (_valColumn4 == value) return;
                _valColumn4 = value;
                RaisePropertyChanged(() => valColumn4);
            }
        }

        private string _valColumn5;
        public string valColumn5
        {
            get { return _valColumn5; }
            set
            {
                if (_valColumn5 == value) return;
                _valColumn5 = value;
                RaisePropertyChanged(() => valColumn5);
            }
        }

        private string _valColumn6;
        public string valColumn6
        {
            get { return _valColumn6; }
            set
            {
                if (_valColumn6 == value) return;
                _valColumn6 = value;
                RaisePropertyChanged(() => valColumn6);
            }
        }


        private string _valColumn7;
        public string valColumn7
        {
            get { return _valColumn7; }
            set
            {
                if (_valColumn7 == value) return;
                _valColumn7 = value;
                RaisePropertyChanged(() => valColumn7);
            }
        }

        private string _valColumn8;
        public string valColumn8
        {
            get { return _valColumn8; }
            set
            {
                if (_valColumn8 == value) return;
                _valColumn8 = value;
                RaisePropertyChanged(() => valColumn8);
            }
        }

        private string _valColumn9;
        public string valColumn9
        {
            get { return _valColumn9; }
            set
            {
                if (_valColumn9 == value) return;
                _valColumn9 = value;
                RaisePropertyChanged(() => valColumn9);
            }
        }
    }

}
