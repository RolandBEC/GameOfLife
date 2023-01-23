using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPFClient1.Infrastructure
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string memberName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(memberName));
        }
    }
}
