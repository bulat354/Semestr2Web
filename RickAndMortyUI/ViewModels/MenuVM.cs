using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ReactiveUI;

namespace RickAndMortyUI.ViewModels
{
    public class MenuVM : GridVM
    {
        private string _errorText;

        public string ErrorText
        {
            get => _errorText;
            set => this.RaiseAndSetIfChanged(ref _errorText, value);
        }

        public string IpText { get; set; }
        public string PortText { get; set; }

        public bool ValidateInputs()
        {
            if (IpText is null || !Regex.IsMatch(IpText, @"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$"))
            {
                ErrorText = "Неправильно указан IP-адрес";
                return false;
            }
            else if (PortText is null || !int.TryParse(PortText, out var port))
            {
                ErrorText = "Неправильно указан порт";
                return false;
            }

            ErrorText = string.Empty;
            return true;
        }
    }
}
