using System;
using System.Linq;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace MNIST_Demo
{
    public sealed partial class MainPage : Page
    {
        // inizializzo le variabili che rappresentano le classi wrapper generate da Visual Studio
        private MNISTModel ModelGen = new MNISTModel();

        private MNISTModelInput ModelInput = new MNISTModelInput();
        private MNISTModelOutput ModelOutput = new MNISTModelOutput();
        private Helper helper = new Helper();
        private RenderTargetBitmap renderBitmap = new RenderTargetBitmap();

        public MainPage()
        {
            this.InitializeComponent();
            // impostare i tipi di dispositivo inking supportati
            inkCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse | Windows.UI.Core.CoreInputDeviceTypes.Pen | Windows.UI.Core.CoreInputDeviceTypes.Touch;
            inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(
                new Windows.UI.Input.Inking.InkDrawingAttributes()
                {
                    Color = Windows.UI.Colors.White,
                    Size = new Size(22, 22),
                    IgnorePressure = true,
                    IgnoreTilt = true,
                }
            );
            LoadModel();
        }

        private async void LoadModel()
        {
            /* caricare il modello di apprendimento automatico a partire dal file aggiunto nella
             * cartella Assets del progetto tramite le API di StorageFile e quindi agganciarlo
             * alla classe corrispondente al modello stesso creato da Visual Studio */
            StorageFile modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/MNIST.onnx"));
            ModelGen = await MNISTModel.CreateMNISTModel(modelFile);
        }

        /* la vera e propria valutazione avviene solamente allo scatenarsi dell'evento di Click
         * sul pulsante Recognize creato nell'UI, le operazioni da effettuare in questo caso sono
         * solamente due e rappresentano le ultime due fasi: la prima è il binding delle proprietà
         * d'input verso il modello (in questo caso solo una) e la seconda è l'analisi con il recupero
         * delle proprietà di output */

        private async void recognizeButton_Click(object sender, RoutedEventArgs e)
        {
            // associa l'input del modello al contenuto di InkCanvas
            ModelInput.Input3 = await helper.GetHandWrittenImage(inkGrid);
            // valuta il modello
            ModelOutput = await ModelGen.EvaluateAsync(ModelInput);
            // query LINQ  per verificare la cifra di probabilità più alta
            var maxIndex = ModelOutput.Plus214_Output_0.IndexOf(ModelOutput.Plus214_Output_0.Max());
            numberLabel.Text = maxIndex.ToString();
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.InkPresenter.StrokeContainer.Clear();
            numberLabel.Text = "";
        }
    }
}