using GalaSoft.MvvmLight;
using SmartDrawerDatabase.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.Model
{
    public delegate void NotifycheckedDelegate();
    public class BaseObject : ViewModelBase
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
    }
}
