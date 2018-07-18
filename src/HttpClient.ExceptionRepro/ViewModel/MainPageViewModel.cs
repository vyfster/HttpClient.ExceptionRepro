using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using NetHttp = System.Net.Http;

namespace HttpClient.ExceptionRepro.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        protected string _serverLocation = string.Empty;
        protected string _uploadResult = "Nothing to see yet ..";
        protected bool _isBusy = false;
        protected bool _uploadButtonsEnabled = true;

        public MainPageViewModel()
        {
        }

        public ICommand UploadDataExpectSuccess
        {
            get
            {
                return new Command(async (p) =>
                {
                    try
                    {
                        IsBusy = true;
                        UploadButtonsEnabled = false;

                        UploadResult = "Starting Upload - expect success.";

                        await UploadDataToServerAsync(p.ToString());
                    }
                    finally
                    {
                        UploadButtonsEnabled = true;
                        IsBusy = false;
                    }
                });
            }
        }

        public ICommand UploadDataExpectException
        {
            get
            {
                return new Command(async (p) =>
                {
                    try
                    {
                        IsBusy = true;
                        UploadButtonsEnabled = false;

                        UploadResult = "Starting Upload - expect exception.";

                        await UploadDataToServerAsync(p.ToString());
                    }
                    finally
                    {
                        UploadButtonsEnabled = true;
                        IsBusy = false;
                    }
                });
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        public bool UploadButtonsEnabled
        {
            get { return _uploadButtonsEnabled; }
            set
            {
                _uploadButtonsEnabled = value;
                OnPropertyChanged("UploadButtonsEnabled");
            }
        }

        public string ServerLocation
        {
            get { return _serverLocation; }
            set
            {
                _serverLocation = value;
                OnPropertyChanged("ServerLocation");
            }
        }

        public string UploadResult
        {
            get { return _uploadResult; }
            set
            {
                _uploadResult = value;
                OnPropertyChanged("UploadResult");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected async Task UploadDataToServerAsync(string imageName)
        {
            var httpClient = new NetHttp.HttpClient();

            var uri = ServerLocation;
            var fileData = GetFileData(imageName);
            var item = new
            {
                FileData = fileData
            };
            var json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            try
            {
                var result = await httpClient.PostAsync(uri, content)
                    .ConfigureAwait(false);

                UploadResult = $"HttpClient handled successfully: {result.ReasonPhrase}.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);

                UploadResult = $"HttpClient Exception: {ex.Message}";
            }
        }

        protected string GetFileData(string resourceName)
        {
            var assembly = typeof(MainPageViewModel).GetTypeInfo().Assembly;

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var binaryReader = new BinaryReader(stream))
                {
                    var bytes = binaryReader.ReadBytes((int)stream.Length - 1);
                    return Convert.ToBase64String(bytes);
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
