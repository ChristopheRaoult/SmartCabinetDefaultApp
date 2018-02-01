using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls.Dialogs;
using SmartDrawerAdmin.Fingerprint;
using SmartDrawerAdmin.StaticHelpers;
using SmartDrawerDatabase;
using SmartDrawerDatabase.DAL;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace SmartDrawerAdmin.ViewModel
{
   
    public class MainViewModel : ViewModelBase
    {
        #region Variables
        private MainWindow mainview0;       
        #endregion

        #region Properties
        private ObservableCollection<Device> _dataDevice= new ObservableCollection<Device>();
        public ObservableCollection<Device> DataDevice
        {
            get { return _dataDevice; }
            set
            {
                _dataDevice = value;
                RaisePropertyChanged(() => DataDevice);
            }
        }
        private Device _selectedDevice;
        public Device SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                EditedDevice = SelectedDevice;
                RaisePropertyChanged(() => SelectedDevice);
            }
        }
        private Device _editedDevice = new Device();
        public Device EditedDevice
        {
            get { return _editedDevice; }
            set
            {
                _editedDevice = value;
                RaisePropertyChanged(() => EditedDevice);
            }
        }   

        private ObservableCollection<UsersViewModel> _dataUser = new ObservableCollection<UsersViewModel>();
        public ObservableCollection<UsersViewModel> DataUser
        {
            get { return _dataUser; }
            set
            {
                _dataUser = value;
                RaisePropertyChanged(() => DataUser);
            }
        }
        private UsersViewModel _selectedUser;
        public UsersViewModel SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                EditedUser = SelectedUser;
                RaisePropertyChanged(() => SelectedUser);
            }
        }
        private UsersViewModel _editedUser = new UsersViewModel();
        public UsersViewModel EditedUser
        {
            get { return _editedUser; }
            set
            {
                _editedUser = value;
                RaisePropertyChanged(() => EditedUser);
            }
        }

        private UsersViewModel _selectedGrantedUser;
        public UsersViewModel SelectedGrantedUser
        {
            get { return _selectedGrantedUser; }
            set
            {
                _selectedGrantedUser = value;           
                
                if (SelectedGrantedUser != null)
                {
                    PopulateDeviceGranted();
                }
                RaisePropertyChanged(() => SelectedGrantedUser);
            }
        }

        private Device _selectedDeviceAvailable;
        public Device SelectedDeviceAvailable
        {
            get { return _selectedDeviceAvailable; }
            set
            {
                _selectedDeviceAvailable = value;               
                RaisePropertyChanged(() => SelectedDeviceAvailable);
            }
        }

        private ObservableCollection<Device> _dataDeviceAvailable = new ObservableCollection<Device>();
        public ObservableCollection<Device> DataDeviceAvailable
        {
            get { return _dataDeviceAvailable; }
            set
            {
                _dataDeviceAvailable = value;
                RaisePropertyChanged(() => DataDeviceAvailable);
            }
        }

        private Device _selectedDeviceGranted;
        public Device SelectedDeviceGranted
        {
            get { return _selectedDeviceGranted; }
            set
            {
                _selectedDeviceGranted = value;              
                RaisePropertyChanged(() => SelectedDeviceGranted);
            }
        }
        private ObservableCollection<Device> _dataDeviceGranted = new ObservableCollection<Device>();
        public ObservableCollection<Device> DataDeviceGranted
        {
            get { return _dataDeviceGranted; }
            set
            {
                _dataDeviceGranted = value;
                RaisePropertyChanged(() => DataDeviceGranted);
            }
        }
        #endregion

        #region Fonctions
        private async void PopulateDevice()
        {
            try
            {
                var ctx = await RemoteDatabase.GetDbContextAsync();
                var lstDev = ctx.Devices.ToList();
                EditedDevice = new Device();
                DataDevice.Clear();
                foreach (var dv in lstDev)
                    DataDevice.Add(dv);               
                ctx.Database.Connection.Close();
                ctx.Dispose();
            }
            catch (Exception error)
            {
                await mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in Populate Device");
                    exp.ShowDialog();
                }));
            }
        }
        private async void PopulateUser()
        {
            try
            {
                var ctx = await RemoteDatabase.GetDbContextAsync();
                var lstDev = ctx.GrantedUsers
                    .Where(u => u.UserRankId > 1).ToList();

                EditedUser = new UsersViewModel();
                DataUser.Clear();
                foreach (var dv in lstDev)
                {
                    UsersViewModel uvm = new UsersViewModel()
                    {
                         Id = dv.GrantedUserId,
                         Login =  dv.Login,                         
                         FirstName = dv.FirstName,
                         LastName = dv.LastName,
                         BadgeId = dv.BadgeNumber,
                         Fingerprints = dv.Fingerprints.Count
                    };
                    DataUser.Add(uvm);
                }
                ctx.Database.Connection.Close();
                ctx.Dispose();
            }
            catch (Exception error)
            {
                await mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in Populate Device");
                    exp.ShowDialog();
                }));
            }

        }

        private async void PopulateDeviceGranted()
        {
            try
            {                
                DataDeviceAvailable.Clear();
                DataDeviceGranted.Clear();
                if (SelectedGrantedUser != null)
                {
                    var ctx = await RemoteDatabase.GetDbContextAsync();
                    var user = ctx.GrantedUsers.Find(SelectedGrantedUser.Id);
                    if (user != null)
                    {
                        var lstDev = ctx.GrantedAccesses.GetByUser(user);
                        foreach (var dv in lstDev)
                            DataDeviceGranted.Add(dv.Device);

                        foreach (var dv in DataDevice)
                        {                          
                            if (DataDeviceGranted.Where(d =>  d.DeviceId == dv.DeviceId).FirstOrDefault() == null)
                                DataDeviceAvailable.Add(dv);
                        }

                    }

                    ctx.Database.Connection.Close();
                    ctx.Dispose();
                }      
            }
            catch (Exception error)
            {
                await mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in Populate Device");
                    exp.ShowDialog();
                }));
            }
        }

        #endregion
        #region Commands
        public RelayCommand btSaveDevice { get; set; }
        public RelayCommand btDeleteDevice { get; set; }
        private async void DeleteDevice()
        {
            if (SelectedDevice != null)  //Update existing
            {
                var ctx = await RemoteDatabase.GetDbContextAsync();
                var original = ctx.Devices.Find(SelectedDevice.DeviceId);
                if (original != null)
                    ctx.Devices.Remove(original);
                ctx.SaveChanges();
                ctx.Database.Connection.Close();
                ctx.Dispose();
                PopulateDevice();
            }
        }
        private async void SaveDevice()
        {
            if (EditedDevice!= null)
            {
                var ctx = await RemoteDatabase.GetDbContextAsync();
                if (SelectedDevice != null)  //Update existing
                {
                    if (string.IsNullOrEmpty(EditedDevice.Name) || string.IsNullOrEmpty(EditedDevice.RfidSerial))
                    {
                        await mainview0.ShowMessageAsync("INFORMATION", "Please Fill device Name and RFID Serial before saving");
                        return;
                    }
                    else
                    {
                        var original = ctx.Devices.Find(SelectedDevice.DeviceId);
                        if (original != null)
                        {
                            original.Name = EditedDevice.Name;
                            original.SerialNumber = EditedDevice.SerialNumber;
                            original.RfidSerial = EditedDevice.RfidSerial;
                            original.IpAddress = EditedDevice.IpAddress;
                            ctx.Entry(original).State = EntityState.Modified;
                            ctx.SaveChanges();
                        }
                    }
                }
                else //save new
                {
                    if (string.IsNullOrEmpty(EditedDevice.Name) || string.IsNullOrEmpty(EditedDevice.RfidSerial))
                    {
                        await mainview0.ShowMessageAsync("INFORMATION", "Please Fill device Name and RFID Serial before saving");
                        return;
                    }
                    else
                    {
                        ctx.Devices.Add(new Device
                        {
                            DeviceTypeId = 15,
                            Name = EditedDevice.Name,
                            SerialNumber = EditedDevice.SerialNumber,
                            RfidSerial = EditedDevice.RfidSerial,
                            IpAddress = EditedDevice.IpAddress
                        });
                        ctx.SaveChanges();
                    }
                }
                ctx.Database.Connection.Close();
                ctx.Dispose();
                PopulateDevice();
            }
        }
        public RelayCommand btResetDevice { get; set; }
        private void ResetDevice()
        {
            SelectedDevice = null;
            EditedDevice = new Device();
        }

        public RelayCommand btEnrollUser { get; set; }
        private async void EnrollUser()
        {

            if (SelectedUser != null)  //Update existing
            {
                var ctx = await RemoteDatabase.GetDbContextAsync();
                var original = ctx.GrantedUsers.Find(SelectedUser.Id);
                if (original != null)
                {
                    var enrollForm = new EnrollFingersForm(original);
                    if (!enrollForm.IsDisposed)
                    {
                        enrollForm.ShowDialog();
                    }
                }
            }
            else
            {
                await mainview0.ShowMessageAsync("INFORMATION", "Please create and save an user before enroll any fingerprint");
                return;
            }           
            PopulateUser();

        }

        public RelayCommand btSaveUser { get; set; }
        private async void SaveUser()
        {
            if (EditedUser != null)
            {
                var ctx = await RemoteDatabase.GetDbContextAsync();
                if (SelectedUser != null)  //Update existing
                {
                    if (string.IsNullOrEmpty(EditedUser.Login) || string.IsNullOrEmpty(EditedUser.FirstName) || string.IsNullOrEmpty(EditedUser.LastName))
                    {
                        await mainview0.ShowMessageAsync("INFORMATION", "Please Fill user Login , Firstname and Lastname before saving");
                        return;
                    }
                    var original = ctx.GrantedUsers.Find(SelectedUser.Id);
                    if (original != null)
                    {
                        original.Login = SelectedUser.Login;
                        if (!string.IsNullOrWhiteSpace(SelectedUser.Password))
                            original.Password = PasswordHashing.Sha256Of(SelectedUser.Password);
                        original.LastName = SelectedUser.LastName;
                        original.FirstName = SelectedUser.FirstName;
                        original.BadgeNumber = SelectedUser.BadgeId;
                        ctx.Entry(original).State = EntityState.Modified;
                        ctx.SaveChanges();
                    }
                }
                else //save new
                {
                    if (string.IsNullOrEmpty(EditedUser.Login) || string.IsNullOrEmpty(EditedUser.FirstName) || string.IsNullOrEmpty(EditedUser.LastName))
                    {
                        await mainview0.ShowMessageAsync("INFORMATION", "Please Fill user Login , Firstname and Lastname before saving");
                        return;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(EditedUser.Password))
                        {
                            ctx.GrantedUsers.Add(new GrantedUser()
                            {
                                Login = EditedUser.Login,
                                Password = PasswordHashing.Sha256Of(EditedUser.Password),
                                FirstName = EditedUser.FirstName,
                                LastName = EditedUser.LastName,
                                BadgeNumber = EditedUser.BadgeId,
                                UserRankId = 3,
                            });
                        }
                        else
                        {
                            ctx.GrantedUsers.Add(new GrantedUser()
                            {
                                Login = EditedUser.Login,                               
                                FirstName = EditedUser.FirstName,
                                LastName = EditedUser.LastName,
                                BadgeNumber = EditedUser.BadgeId,
                                UserRankId = 3,
                            });
                        }
                        ctx.SaveChanges();
                    }
                }
                ctx.Database.Connection.Close();
                ctx.Dispose();
                PopulateUser();            
            }
        }
        public RelayCommand btDeleteUser { get; set; }
        private async void DeleteUser()
        {
            if (SelectedUser != null)  //Update existing
            {
                var ctx = await RemoteDatabase.GetDbContextAsync();
                var original = ctx.GrantedUsers.Find(SelectedUser.Id);
                if (original != null)
                    ctx.GrantedUsers.Remove(original);
                ctx.SaveChanges();
                ctx.Database.Connection.Close();
                ctx.Dispose();
                PopulateUser();
            }
        }
        public RelayCommand btResetUser { get; set; }
        private void ResetUser()
        {
            SelectedUser = null;
            EditedUser = new UsersViewModel();
        }       

        public RelayCommand btAddGrantedDevice { get; set; }
        private async void AddGrantedDevice()
        {
            if ((SelectedDeviceAvailable != null) && (SelectedGrantedUser != null))
            {
                try
                {
                    var ctx = await RemoteDatabase.GetDbContextAsync();
                    var user = ctx.GrantedUsers.Find(SelectedGrantedUser.Id);
                    if (user != null)
                    {
                        ctx.GrantedAccesses.AddOrUpdateAccess(user, SelectedDeviceAvailable, ctx.GrantTypes.All());
                        await ctx.SaveChangesAsync();
                        ctx.Database.Connection.Close();
                        ctx.Dispose();
                    }
                }
                catch (Exception error)
                {
                    await mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                    {
                        ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in Add Access");
                        exp.ShowDialog();
                    }));
                }
            }
            PopulateDeviceGranted();
        }

        public RelayCommand btRemoveGrantedDevice { get; set; }
        private async void RemoveGrantDevice()
        {
            if ((SelectedDeviceGranted != null) && (SelectedGrantedUser != null))
            {
                try
                {
                    var ctx = await RemoteDatabase.GetDbContextAsync();
                    var user = ctx.GrantedUsers.Find(SelectedGrantedUser.Id);
                    if (user != null)
                    {
                        ctx.GrantedAccesses.RemoveAccess(user, SelectedDeviceGranted);
                        await ctx.SaveChangesAsync();
                        ctx.Database.Connection.Close();
                        ctx.Dispose();
                    }
                }
                catch (Exception error)
                {
                    await mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                    {
                        ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in remove access");
                    }));
                }
            }
            PopulateDeviceGranted();
        }
        #endregion





        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            mainview0 = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            mainview0.Loaded += Mainview0_Loaded;
            if (string.IsNullOrEmpty(Properties.Settings.Default.DbPassword))
            {
                string pwd = "rfid";
                var secureString = pwd.ToSecureString();
                Properties.Settings.Default.DbPassword = secureString.EncryptString();
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Upgrade();
            }

            btSaveDevice = new RelayCommand( ()=> SaveDevice());
            btResetDevice = new RelayCommand( ()=> ResetDevice());
            btDeleteDevice = new RelayCommand( ()=> DeleteDevice());

            btSaveUser = new RelayCommand( ()=> SaveUser());
            btResetUser = new RelayCommand( ()=> ResetUser());
            btDeleteUser = new RelayCommand( ()=> DeleteUser());
            btEnrollUser = new RelayCommand( () => EnrollUser());
            btAddGrantedDevice = new RelayCommand(() => AddGrantedDevice());
            btRemoveGrantedDevice = new RelayCommand(() => RemoveGrantDevice());
           
        }

        bool isAdmin = false;
        bool quit = false;
        private async void Mainview0_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            isAdmin = true;
            /*LoginDialogData result = null;
            do
            {
                LoginDialogSettings lds = new LoginDialogSettings();
                lds.ColorScheme = MetroDialogColorScheme.Theme;
                lds.InitialUsername = "Admin";

                result = await mainview0.ShowLoginAsync("Information", "Enter administrator password?", lds);
                if (result != null)
                {
                    try
                    {
                        var ctx = await RemoteDatabase.GetDbContextAsync();
                        GrantedUser adminUser = ctx.GrantedUsers.GetByLogin(result.Username);
                        if (adminUser == null)
                        {
                            MessageDialogResult mdr = await mainview0.ShowMessageAsync("Question", "Admin not found , Would you like to quit", MessageDialogStyle.AffirmativeAndNegative);
                            if (mdr == MessageDialogResult.Affirmative)
                                System.Windows.Application.Current.Shutdown();
                        }
                            
                        if (adminUser.Password == SmartDrawerDatabase.PasswordHashing.Sha256Of(result.Password))
                        {
                            isAdmin = true;
                        }                        
                        ctx.Database.Connection.Close();
                        ctx.Dispose();
                    }
                    catch (Exception error)
                    {
                        await mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                        {
                            ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error get Admin");
                        }));
                    }   
                }
            }
            while (!isAdmin);*/

            if (isAdmin)
            {
                PopulateDevice();
                PopulateUser();
            }  
        }
    }
}