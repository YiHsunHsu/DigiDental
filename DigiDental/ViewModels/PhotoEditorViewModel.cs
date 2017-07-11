using System.Collections.ObjectModel;

namespace DigiDental.ViewModels
{
    public class PhotoEditorViewModel : ViewModelBase.ViewModelBase
    {
        private Images images;

        public Images Images
        {
            get { return images; }
            set
            {
                images = value;
                OnPropertyChanged("Images");
            }
        }

        private ObservableCollection<Images> imagesCollection;

        public ObservableCollection<Images> ImagesCollection
        {
            get { return imagesCollection; }
            set { imagesCollection = value; }
        }

        private int rotateAngle;

        public int RotateAngle
        {
            get { return rotateAngle; }
            set
            {
                rotateAngle = value;
                OnPropertyChanged("RotateAngle");
            }
        }

    }
}
