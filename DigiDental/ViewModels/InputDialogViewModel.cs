using DigiDental.Class;

namespace DigiDental.ViewModels
{
    public class InputDialogViewModel : ViewModelBase.ViewModelBase
    {
        public InputDialogViewModel(string mode)
        {
            Mode = mode;
        }

        public string Mode { get; set; }

        private string answer;
        public string Answer
        {
            get
            {
                return answer;
            }
            set
            {
                answer = value;
                OnPropertyChanged("Answer");
                switch (Mode)
                {
                    case "IP":
                        if (ValidatorHelper.IsIP(answer))
                        {
                            IsValid = true;
                        }
                        else
                        {
                            IsValid = false;
                        }
                        break;
                    case "Verify":
                        IsValid = true;
                        break;
                }
            }
        }

        private bool isValid;

        public bool IsValid
        {
            get { return isValid; }
            set
            {
                isValid = value;
                OnPropertyChanged("IsValid");
            }
        }
    }
}
