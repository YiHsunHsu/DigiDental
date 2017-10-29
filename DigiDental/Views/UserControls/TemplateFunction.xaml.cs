using DigiDental.Class;
using DigiDental.ViewModels.Class;
using DigiDental.ViewModels.UserControlViewModels;
using DigiDental.ViewModels.ViewModelBase;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace DigiDental.Views.UserControls
{
    /// <summary>
    /// TemplateFunction.xaml 的互動邏輯
    /// </summary>
    public partial class TemplateFunction : UserControl
    {
        public Agencys Agencys
        {
            get { return tfvm.Agencys; }
            set
            {
                if (tfvm == null)
                    tfvm = new TemplateFunctionViewModel(tmpA, tmpP, tmpI);
                tfvm.Agencys = value;
            }
        }

        public Patients Patients
        {
            get { return tfvm.Patients; }
            set
            {
                if (tfvm == null)
                    tfvm = new TemplateFunctionViewModel(tmpA, tmpP, tmpI);
                tfvm.Patients = value;
            }
        }

        public MTObservableCollection<ImageInfo> ShowImages
        {
            get { return tfvm.ShowImages; }
            set
            {
                if (tfvm == null)
                    tfvm = new TemplateFunctionViewModel(tmpA, tmpP, tmpI);
                tfvm.ShowImages = value;
            }
        }
        
        public UserControl TemplateContent
        {
            get { return tfvm.TemplateContent; }
        }

        public Templates TemplateItem
        {
            get { return tfvm.TemplateItem; }
        }
        
        // View Model
        private TemplateFunctionViewModel tfvm;
        // DB
        private DBRegistrations dbr;
        private DBTemplateImages dbti;
        private DBImages dbi;
        //EntityFramwork
        private DigiDentalEntities dde;
        // Class
        private PatientsFolder pf;
        private LoadBitmapImage lbi;
        // View Dialogs
        private ProcessingDialog pd;
        // Paramater
        private ObservableCollection<TemplateImages> tICollection;

        private Agencys tmpA;
        private Patients tmpP;
        private MTObservableCollection<ImageInfo> tmpI;

        public TemplateFunction(Agencys agencys, Patients patients, MTObservableCollection<ImageInfo> showImages)
        {
            InitializeComponent();

            tmpA = agencys;
            tmpP = patients;
            tmpI = showImages;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (tfvm == null)
            {
                tfvm = new TemplateFunctionViewModel(tmpA, tmpP, tmpI);
            }

            DataContext = tfvm;
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
        // 委派回傳 MainWindows
        // Wifi Auto 匯入後更新
        public delegate void ReturnValueDelegate(int Registration_ID, DateTime Registration_Date);
        public event ReturnValueDelegate ReturnValueCallback;

        // Button_AutoImport_Click 變數宣告
        int Registration_ID;
        private DateTime Registration_Date = DateTime.Now;

        bool isStop = false; //接ProcessingDialog 回傳值 停止
        bool isSkip = true; //接ProcessingDialog 回傳值 略過

        private void Button_AutoImport_Click(object sender, RoutedEventArgs e)
        {
            Button btnAutoImport = (Button)sender;

            //先載入原本樣板的圖片
            //如果略過就塞回原圖 
            btnAutoImport.Dispatcher.Invoke(() =>
            {
                bool isEverChanged = false;

                btnAutoImport.IsEnabled = false;

                pd = new ProcessingDialog();
                Task t = Task.Factory.StartNew(() =>
                {
                    pd.Dispatcher.Invoke(() =>
                    {
                        pd.PText = "圖片偵測中";
                        pd.PIsIndeterminate = true;
                        pd.ButtonContent = "跳過";
                        pd.ReturnValueCallback += new ProcessingDialog.ReturnValueDelegate(this.SetReturnValueCallbackFun);

                        pd.Show();
                    });

                    //病患資料夾路徑
                    if (pf == null)
                    {
                        pf = new PatientsFolder(Agencys, Patients, Registration_Date);
                    }

                    //資料表Registrations 處理
                    if (dbr == null)
                    {
                        dbr = new DBRegistrations();
                    }

                    //資料表Images 處理
                    if (dbi == null)
                    {
                        dbi = new DBImages();
                    }

                    //處理載入BitmapImage
                    if (lbi == null)
                    {
                        lbi = new LoadBitmapImage();
                    }

                    //載入TemplateImages
                    if (dbti == null)
                    {
                        dbti = new DBTemplateImages();
                    }

                    tICollection = new ObservableCollection<TemplateImages>();
                    tICollection = dbti.GetTemplateImagesCollection(Agencys, Patients, TemplateItem, tfvm.TemplateImportDate);

                    //default Image[i] in UserControl Templates
                    int Imagei = 0;
                    int ImageCount = (int)TemplateItem.Template_ImageCount;
                    int DecodePixelWidth = (int)TemplateItem.Template_DecodePixelWidth;
                    while (Imagei < ImageCount)
                    {
                        pd.PText = "圖片 " + (Imagei + 1) + " 偵測中";

                        //目前處理的Image[i]
                        Image iTarget;

                        TemplateContent.Dispatcher.Invoke(() =>
                        {
                            iTarget = new Image();
                            iTarget = (Image)TemplateContent.FindName("Image" + Imagei);

                            iTarget.Source = new BitmapImage(new Uri(@"/DigiDental;component/Resource/yes.png", UriKind.RelativeOrAbsolute));
                            //iTarget.Source = lbi.SettingBitmapImage(@"/DigiDental;component/Resource/yes.png", DecodePixelWidth);
                        });

                        //set the paramater default
                        bool isChanged = false;
                        bool detecting = true;
                        while (true)
                        {
                            //開始偵測wifi card路徑
                            foreach (string f in Directory.GetFiles(Agencys.Agency_WifiCardPath))
                            {
                                Thread.Sleep(500);

                                string extension = Path.GetExtension(f).ToUpper();
                                string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                                if (!Directory.Exists(pf.PatientFullFolderPathOriginal))
                                {
                                    Directory.CreateDirectory(pf.PatientFullFolderPathOriginal);
                                }
                                File.Copy(f, pf.PatientFullFolderPathOriginal + @"\" + newFileName + @"ori" + extension);

                                Registration_ID = dbr.CreateRegistrationsAndGetID(Patients, Registration_Date);

                                string imagePath = @"\" + pf.PatientFolderPathOriginal + @"\" + newFileName + @"ori" + extension;
                                string imageFileName = newFileName + @"ori" + extension;
                                string imageSize = "Original";
                                string imageExtension = extension;
                                //寫資料庫
                                //INSERT Images
                                int imageID = dbi.InsertImage(imagePath, imageFileName, imageSize, imageExtension, Registration_ID);
                                isEverChanged = true;
                                TemplateContent.Dispatcher.Invoke(() =>
                                {
                                    iTarget = new Image();
                                    iTarget = (Image)TemplateContent.FindName("Image" + Imagei);

                                    //INSERT TemplateImages
                                    dbti.InsertOrUpdateImage(Patients, TemplateItem, tfvm.TemplateImportDate, imageID, imagePath, iTarget.Uid);
                                    iTarget.Source = lbi.SettingBitmapImage(pf.PatientFullFolderPathOriginal + @"\" + newFileName + @"ori" + extension, DecodePixelWidth);
                                    isChanged = true;
                                });

                                File.Delete(f);
                                detecting = false;
                            }
                            //ProcessingDialog STOP
                            if (isStop)
                            {
                                isStop = false;
                                TemplateContent.Dispatcher.Invoke(() =>
                                {
                                    iTarget = new Image();
                                    iTarget = (Image)TemplateContent.FindName("Image" + Imagei);
                                    var findOriImage = from tc in tICollection
                                                       where tc.TemplateImage_Number == Imagei.ToString()
                                                       select tc;
                                    if (findOriImage.Count() > 0)
                                    {
                                        iTarget.Source = lbi.SettingBitmapImage(findOriImage.First().Image_Path, DecodePixelWidth);
                                    }
                                    else
                                    {
                                        iTarget.Source = new BitmapImage(new Uri(@"/DigiDental;component/Resource/no.png", UriKind.RelativeOrAbsolute));
                                        //iTarget.Source = lbi.SettingBitmapImage(@"/DigiDental;component/Resource/key.ico", DecodePixelWidth);
                                    }
                                });
                                return;
                            }
                            else
                            {
                                // import pic OR skip import (NEXT)
                                if (!detecting || !isSkip)
                                {
                                    if (!isChanged)
                                    {
                                        TemplateContent.Dispatcher.Invoke(() =>
                                        {
                                            iTarget = new Image();
                                            iTarget = (Image)TemplateContent.FindName("Image" + Imagei);
                                            var findOriImage = from tc in tICollection
                                                               where tc.TemplateImage_Number == Imagei.ToString()
                                                               select tc;
                                            if (findOriImage.Count() > 0)
                                            {
                                                iTarget.Source = lbi.SettingBitmapImage(findOriImage.First().Image_Path, DecodePixelWidth);
                                            }
                                            else
                                            {
                                                iTarget.Source = new BitmapImage(new Uri(@"/DigiDental;component/Resource/no.png", UriKind.RelativeOrAbsolute));
                                                //iTarget.Source = lbi.SettingBitmapImage(@"/DigiDental;component/Resource/key.ico", DecodePixelWidth);
                                            }
                                        });
                                    }
                                    Imagei++;
                                    isSkip = true;
                                    break;
                                }
                            }
                        }
                    }
                }).ContinueWith(cw =>
                {
                    //結束
                    pd.PText = "處理完畢";
                    pd.Close();
                    //委派回傳MainWindow
                    //刷新Registrations 資料
                    //刷新Images 資料
                    if (isEverChanged)
                    {
                        ReturnValueCallback(Registration_ID, Registration_Date);
                    }
                    GC.Collect();

                    btnAutoImport.IsEnabled = true;

                }, TaskScheduler.FromCurrentSynchronizationContext());
            });
        }

        /// <summary>
        /// 紀錄是否展開
        /// </summary>
        private bool IsStretch = true;
        private void Button_Stretch_Click(object sender, RoutedEventArgs e)
        {
            if (IsStretch)
            {
                switch(Agencys.Agency_ViewType)
                {
                    case "0":
                        tfvm.StretchWidth = 0;
                        ButtonStretch.Content = "<";
                        break;
                    case "1":
                        tfvm.StretchHeight = 0;
                        ButtonStretch.Content = "︿";
                        break;
                }
                IsStretch = false;
            }
            else
            {
                switch (Agencys.Agency_ViewType)
                {
                    case "0":
                        tfvm.StretchWidth = 270;
                        ButtonStretch.Content = ">";
                        break;
                    case "1":
                        tfvm.StretchHeight = 205;
                        ButtonStretch.Content = "﹀";
                        break;
                }
                IsStretch = true;
            }
        }

        private void Button_TemplateExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                DefaultExt = ".png",
                Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif"
            };
            if (sfd.ShowDialog() == true)
            {
                UserControl control = TemplateContent;
                
                RenderTargetBitmap bmp = new RenderTargetBitmap((int)control.ActualWidth, (int)control.ActualHeight, 96, 96, PixelFormats.Pbgra32);

                bmp.Render(control);

                var encoder = new PngBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(bmp));

                using (Stream stm = File.Create(sfd.FileName))
                {
                    encoder.Save(stm);
                }

                MessageBox.Show("檔案建立成功，存放位置於" + sfd.FileName, "提示", MessageBoxButton.OK);
            }
        }
        private void Button_PPTExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                FileName = Patients.Patient_ID + "-" + Patients.Patient_Name + "-" + TemplateItem.Template_Title,
                DefaultExt = ".pptx",
                Filter = "PowerPoint 簡報 (*.pptx)|*.pptx"
            };
            if (sfd.ShowDialog() == true)
            {
                ShowPresentation(sfd.FileName);
                MessageBox.Show("檔案建立成功，存放位置於" + sfd.FileName, "提示", MessageBoxButton.OK);
            }
            GC.Collect();
        }

        #region METHOD
        private void SetReturnValueCallbackFun(bool isDetecting)
        {
            if (isDetecting)
            {
                isStop = isDetecting;
            }
            else
            {
                isSkip = isDetecting;
            }
        }

        private void ShowPresentation(string fileName)
        {
            if (dde == null)
            {
                dde = new DigiDentalEntities();
            }

            List<TemplateImages> liTI = new List<TemplateImages>();
            var templateImages = from ti in dde.TemplateImages
                                 where ti.Patient_ID == Patients.Patient_ID
                                 && ti.Template_ID == TemplateItem.Template_ID
                                 select ti;
            liTI = templateImages.ToList();

            if (liTI.Count > 0)
            {
                PowerPoint.Application pptApplication = new PowerPoint.Application();

                Slides slides;
                _Slide slide;
                TextRange objText;

                // Create the Presentation File
                Presentation pptPresentation = pptApplication.Presentations.Add(MsoTriState.msoTrue);

                CustomLayout customLayout = pptPresentation.SlideMaster.CustomLayouts[PpSlideLayout.ppLayoutText];

                for (int i = 0; i < liTI.Count; i++)
                {
                    // Create new Slide
                    slides = pptPresentation.Slides;
                    slide = slides.AddSlide(i + 1, customLayout);

                    // Add title
                    objText = slide.Shapes[1].TextFrame.TextRange;
                    objText.Text = TemplateItem.Template_Title;
                    objText.Font.Name = "Arial";
                    objText.Font.Size = 32;
                    
                    PowerPoint.Shape shape = slide.Shapes[2];
                    slide.Shapes.AddPicture(Agencys.Agency_ImagePath + liTI[i].Image_Path, MsoTriState.msoFalse, MsoTriState.msoTrue, shape.Left, shape.Top, shape.Width, shape.Height);

                    slide.NotesPage.Shapes[2].TextFrame.TextRange.Text = "This document is created by DigiDental.";
                }
                pptPresentation.SaveAs(fileName, PpSaveAsFileType.ppSaveAsDefault, MsoTriState.msoTrue);
                //pptApplication.Quit();
            }
        }
        #endregion
    }
}
