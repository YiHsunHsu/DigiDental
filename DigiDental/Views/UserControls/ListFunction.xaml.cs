using DigiDental.Class;
using DigiDental.ViewModels.Class;
using DigiDental.ViewModels.UserControlViewModels;
using DigiDental.ViewModels.ViewModelBase;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DigiDental.Views.UserControls
{
    /// <summary>
    /// ListFunction.xaml 的互動邏輯
    /// </summary>
    public partial class ListFunction : UserControl
    {
        public Agencys Agencys
        {
            get { return lfvm.Agencys; }
            set
            {
                if (lfvm == null)
                    lfvm = new ListFunctionViewModel(tmpA, tmpI);
                lfvm.Agencys = value;
            }
        }

        public MTObservableCollection<ImageInfo> ShowImages
        {
            get { return lfvm.ShowImages; }
            set
            {
                if (lfvm == null)
                    lfvm = new ListFunctionViewModel(tmpA, tmpI);
                lfvm.ShowImages = value;
            }
        }

        private ListFunctionViewModel lfvm;

        private Agencys tmpA;
        private MTObservableCollection<ImageInfo> tmpI;

        public ListFunction(Agencys agencys, MTObservableCollection<ImageInfo> showImages)
        {
            InitializeComponent();

            //暫存
            //Loaded後載入
            tmpA = agencys;
            tmpI = showImages;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (lfvm == null)
            {
                lfvm = new ListFunctionViewModel(tmpA, tmpI);
            }

            DataContext = lfvm;
            
        }

        private void Button_ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (lfvm.ColumnCount > 1)
                lfvm.ColumnCount--;
        }

        private void Button_ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (lfvm.ColumnCount < 5)
                lfvm.ColumnCount++;
        }

        //private void Button_ListAll_Click(object sender, RoutedEventArgs e)
        //{
        //    //載入Images
        //    //取圖片清單 Images
        //    var queryImages = from r in lfvm.RegistrationsCollection
        //                      join i in dde.Images
        //                      on r.Registration_ID equals i.Registration_ID into ri
        //                      from qri in ri.DefaultIfEmpty()
        //                      where qri.Image_Size.Equals("Original")
        //                            && qri.Image_IsEnable == true
        //                      select new ImageInfo()
        //                      {
        //                          Registration_Date = qri.Registrations.Registration_Date,
        //                          Image_ID = qri.Image_ID,
        //                          Image_Path = Agencys.Agency_ImagePath + qri.Image_Path,
        //                          Image_FileName = qri.Image_FileName,
        //                          Image_Extension = qri.Image_Extension,
        //                          Registration_ID = qri.Registration_ID,
        //                          CreateDate = qri.CreateDate
        //                      };
        //    lfvm.ImagesInfo = new ObservableCollection<ImageInfo>(queryImages);
        //    lfvm.SelectedValue = new ListFunctionViewModel.ComboBoxItem();
        //}

        //private void Button_Import_Click(object sender, RoutedEventArgs e)
        //{
        //    Button btnImport = (Button)sender;
        //    btnImport.IsEnabled = false;
        //    btnImport.Refresh();

        //    OpenFileDialog ofd = new OpenFileDialog();
        //    ofd.Multiselect = true;
        //    ofd.DefaultExt = ".png";
        //    ofd.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
        //    bool? ofdResult = ofd.ShowDialog();
        //    if (ofdResult.HasValue && ofdResult.Value)//OpenFileDialog 選確定
        //    {
        //        if (Directory.Exists(Agencys.Agency_ImagePath))
        //        {
        //            //讀寫Registrations
        //            //確認掛號資料
        //            int Registration_ID;
        //            DateTime RegistrationDate = lfvm.SelectedDate;
        //            var queryRegistration = from qr in dde.Registrations
        //                                    where qr.Patient_ID == Patients.Patient_ID && qr.Registration_Date == RegistrationDate
        //                                    select qr;
        //            if (queryRegistration.Count() == 0)
        //            {
        //                var newRegistration = new Registrations
        //                {
        //                    Patient_ID = Patients.Patient_ID,
        //                    Registration_Date = RegistrationDate
        //                };
        //                dde.Registrations.Add(newRegistration);
        //                dde.SaveChanges();
        //                Registration_ID = newRegistration.Registration_ID;
        //            }
        //            else
        //            {
        //                Registration_ID = queryRegistration.First().Registration_ID;
        //            }

        //            //..\病患資料夾\掛號日期
        //            string PatientFolderPath = Patients.Patient_ID + @"\" + RegistrationDate.ToString("yyyyMMdd");
        //            //..\病患資料夾\掛號日期\Original
        //            string PatientFolderPathOriginal = PatientFolderPath + @"\Original";
        //            //..\病患資料夾\掛號日期\Small
        //            string PatientFolderPathSmall = PatientFolderPath + @"\Small";
        //            //Agencys_ImagePath\病患資料夾\掛號日期
        //            string PatientFullFolderPath = Agencys.Agency_ImagePath + @"\" + PatientFolderPath;
        //            //Agencys_ImagePath\病患資料夾\掛號日期\Original
        //            string PatientFullFolderPathOriginal = PatientFullFolderPath + @"\Original";
        //            //Agencys_ImagePath\病患資料夾\掛號日期\Small
        //            string PatientFullFolderPathSmall = PatientFullFolderPath + @"\Small";

        //            if (!Directory.Exists(PatientFullFolderPath))
        //            {
        //                Directory.CreateDirectory(PatientFullFolderPathOriginal);
        //                Directory.CreateDirectory(PatientFullFolderPathSmall);
        //            }

        //            foreach (string fileName in ofd.FileNames)
        //            {
        //                string extension = Path.GetExtension(fileName).ToUpper();
        //                string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff");
        //                //複製原圖到目的Original
        //                File.Copy(fileName, PatientFullFolderPathOriginal + @"\" + newFileName + @"ori" + extension);

        //                //寫資料庫                    
        //                dde.Images.Add(new Images
        //                {
        //                    Image_Path = @"\" + PatientFolderPathOriginal + @"\" + newFileName + @"ori" + extension,
        //                    Image_FileName = newFileName + @"ori" + extension,
        //                    Image_Size = "Original",
        //                    Image_Extension = extension,
        //                    Registration_ID = Registration_ID
        //                });
        //                dde.SaveChanges();

        //                #region 產生小圖(未使用)
        //                //產生縮圖到Small
        //                //ImageProcess.SaveThumbPic(fileName, 300, PatientFullFolderPathSmall + @"\" + newFileName + @"sml" + extension);

        //                //寫資料庫
        //                //dde.Images.Add(new Images
        //                //{
        //                //    Image_Path = @"\" + PatientFolderPathSmall + @"\" + newFileName + @"sml" + extension,
        //                //    Image_FileName = newFileName + @"sml" + extension,
        //                //    Image_Size = "Small",
        //                //    Image_Extension = extension,
        //                //    Registration_ID = Registration_ID
        //                //});
        //                //dde.SaveChanges();
        //                #endregion

        //                Thread.Sleep(200);
        //            }

        //            //匯入之後重新載入  取掛號資訊清單 Registration
        //            var queryRegistrations = from qr in dde.Registrations
        //                                     where qr.Patient_ID == Patients.Patient_ID
        //                                     orderby qr.Registration_Date descending
        //                                     select qr;
        //            lfvm.RegistrationsCollection = new ObservableCollection<Registrations>(queryRegistrations.ToList());
        //            lfvm.SelectedDate = lfvm.SelectedDate;
        //        }
        //        else
        //        {
        //            MessageBox.Show("影像資料夾有問題，請檢查設定是否有誤", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
        //        }
        //    }
            
        //    btnImport.IsEnabled = true;
        //    btnImport.Refresh();
        //}

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (ImageInfo item in e.RemovedItems)
            {
                item.IsSelected = false;
            }
            foreach (ImageInfo item in e.AddedItems)
            {
                item.IsSelected = true;
            }
            lfvm.ImageSelectedCount = lfvm.ShowImages.Where(i => i.IsSelected).Count();
        }

        private void Button_PhotoEditor_Click(object sender, RoutedEventArgs e)
        {
            if (lfvm.ShowImages.Count() > 0)
            {
                PhotoEditor pe;
                if (lfvm.ShowImages.Where(i => i.IsSelected).Count() > 0)
                {
                    pe = new PhotoEditor(new ObservableCollection<ImageInfo>(lfvm.ShowImages.Where(i => i.IsSelected).OrderBy(o => o.Registration_Date).OrderBy(o2 => o2.Image_ID)));
                }
                else
                {
                    pe = new PhotoEditor(new ObservableCollection<ImageInfo>(lfvm.ShowImages.OrderBy(o => o.Registration_Date).OrderBy(o2 => o2.Image_ID)));
                }
                pe.Show();
                lbImages.UnselectAll();
            }
            else
            {
                MessageBox.Show("尚未載入圖片", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Button_Outport_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog.Description = "請選擇圖片匯出的位置";
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var listIsSelectedItem = lfvm.ShowImages.Where(w => w.IsSelected == true);
                foreach (ImageInfo ii in listIsSelectedItem)
                {
                    File.Copy(ii.Image_FullPath, folderBrowserDialog.SelectedPath + @"\" + ii.Image_FileName + ii.Image_Extension);
                }
                MessageBox.Show("匯出完成", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("確定刪除已選定的" + lfvm.ImageSelectedCount + "個項目?", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var dde = new DigiDentalEntities())
                {
                    try
                    {
                        var selectedItemID = from si in lfvm.ShowImages
                                             where si.IsSelected == true
                                             select si.Image_ID;
                        var deleteItem = (from i in dde.Images
                                          where selectedItemID.Contains(i.Image_ID)
                                          select i).ToList();
                        deleteItem.ForEach(i => i.Image_IsEnable = false);
                        dde.SaveChanges();

                        var selectedItem = from si in lfvm.ShowImages
                                           where si.IsSelected == true
                                           select si;
                        foreach (ImageInfo ii in selectedItem.ToArray())
                        {
                            lfvm.ShowImages.Remove(ii);
                        }
                        lfvm.CountImages = ShowImages.Count();
                    }
                    catch (Exception ex)
                    {
                        Error_Log.ErrorMessageOutput(ex.ToString());
                    }
                }
            }
        }

        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                ImageInfo dragImage = (ImageInfo)((Image)e.Source).DataContext;
                DataObject data = new DataObject(DataFormats.Text, dragImage);

                DragDrop.DoDragDrop((DependencyObject)e.Source, data, DragDropEffects.Copy);
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
                MessageBox.Show("移動圖片發生錯誤，聯絡資訊人員", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
