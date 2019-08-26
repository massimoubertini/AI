using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ComputerVision_WPF
{
    public partial class MainWindow : Window
    {
        private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient("bc116f9fdbdd49dc9627695b657be473", "https://westcentralus.api.cognitive.microsoft.com/face/v1.0");

        // elenco delle facce rilevate
        private Face[] faces;

        // elenco delle descrizioni per le facce rilevate
        private String[] faceDescriptions;

        // fattore di ridimensionamento per l'immagine visualizzata
        private double resizeFactor;

        public MainWindow()
        { InitializeComponent(); }

        // visualizza l'immagine e chiama Detect Faces
        private async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {   // 'immagine da scansionare .
            var openDlg = new Microsoft.Win32.OpenFileDialog();
            openDlg.Filter = "JPEG Image(*.jpg)|*.jpg";
            bool? result = openDlg.ShowDialog(this);
            if (!(bool)result)
                return;
            // visualizza il file immagine
            string filePath = openDlg.FileName;
            Uri fileUri = new Uri(filePath);
            BitmapImage bitmapSource = new BitmapImage();
            bitmapSource.BeginInit();
            bitmapSource.CacheOption = BitmapCacheOption.None;
            bitmapSource.UriSource = fileUri;
            bitmapSource.EndInit();
            FacePhoto.Source = bitmapSource;
            // rileva le facce dell'immagine
            Title = "Rilevamento...";
            faces = await UploadAndDetectFaces(filePath);
            Title = String.Format("Detection Finished. {0} face(s) detected", faces.Length);
            if (faces.Length > 0)
            {
                // disegna rettangoli intorno alle facce
                DrawingVisual visual = new DrawingVisual();
                DrawingContext drawingContext = visual.RenderOpen();
                drawingContext.DrawImage(bitmapSource, new Rect(0, 0, bitmapSource.Width, bitmapSource.Height));
                double dpi = bitmapSource.DpiX;
                resizeFactor = 96 / dpi;
                faceDescriptions = new String[faces.Length];
                for (int i = 0; i < faces.Length; ++i)
                {
                    Face face = faces[i];
                    // disegna un rettangolo sulla faccia
                    drawingContext.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Red, 2), new Rect(face.FaceRectangle.Left * resizeFactor, face.FaceRectangle.Top * resizeFactor, face.FaceRectangle.Width * resizeFactor, face.FaceRectangle.Height * resizeFactor));
                    // memorizza la descrizione del viso
                    faceDescriptions[i] = FaceDescription(face);
                }
                drawingContext.Close();
                // visualizza l'immagine con il rettangolo intorno al viso
                RenderTargetBitmap faceWithRectBitmap = new RenderTargetBitmap((int)(bitmapSource.PixelWidth * resizeFactor), (int)(bitmapSource.PixelHeight * resizeFactor), 96, 96, PixelFormats.Pbgra32);
                faceWithRectBitmap.Render(visual);
                FacePhoto.Source = faceWithRectBitmap;
                // scrive il testo nella barra di stato
                faceDescriptionStatusBar.Text = "Posizionare il puntatore del mouse su una faccia per vedere la descrizione del viso.";
            }
        }

        // ritorna una stringa che descrive la faccia specificata
        private string FaceDescription(Face face)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Face: ");
            // aggiunge il sesso, l'età e il sorriso
            sb.Append(face.FaceAttributes.Gender);
            sb.Append(", ");
            sb.Append(face.FaceAttributes.Age);
            sb.Append(", ");
            sb.Append(String.Format("smile {0:F1}%, ", face.FaceAttributes.Smile * 100));
            // aggiunge le emozioni, mostra tutte le emozioni oltre il 10%
            sb.Append("Emotion: ");
            EmotionScores emotionScores = face.FaceAttributes.Emotion;
            if (emotionScores.Anger >= 0.1f) sb.Append(String.Format("anger {0:F1}%, ", emotionScores.Anger * 100));
            if (emotionScores.Contempt >= 0.1f) sb.Append(String.Format("contempt {0:F1}%, ", emotionScores.Contempt * 100));
            if (emotionScores.Disgust >= 0.1f) sb.Append(String.Format("disgust {0:F1}%, ", emotionScores.Disgust * 100));
            if (emotionScores.Fear >= 0.1f) sb.Append(String.Format("fear {0:F1}%, ", emotionScores.Fear * 100));
            if (emotionScores.Happiness >= 0.1f) sb.Append(String.Format("happiness {0:F1}%, ", emotionScores.Happiness * 100));
            if (emotionScores.Neutral >= 0.1f) sb.Append(String.Format("neutral {0:F1}%, ", emotionScores.Neutral * 100));
            if (emotionScores.Sadness >= 0.1f) sb.Append(String.Format("sadness {0:F1}%, ", emotionScores.Sadness * 100));
            if (emotionScores.Surprise >= 0.1f) sb.Append(String.Format("surprise {0:F1}%, ", emotionScores.Surprise * 100));
            // aggiunge gli occhiali
            sb.Append(face.FaceAttributes.Glasses);
            sb.Append(", ");
            // aggiunge i capelli
            sb.Append("Hair: ");
            // visualizza la calvizie se è oltre 1%
            if (face.FaceAttributes.Hair.Bald >= 0.01f)
                sb.Append(String.Format("bald {0:F1}% ", face.FaceAttributes.Hair.Bald * 100));
            // visualizza tutti gli attributi di colore dei capelli oltre 10%
            HairColor[] hairColors = face.FaceAttributes.Hair.HairColor;
            foreach (HairColor hairColor in hairColors)
            {
                if (hairColor.Confidence >= 0.1f)
                {
                    sb.Append(hairColor.Color.ToString());
                    sb.Append(String.Format(" {0:F1}% ", hairColor.Confidence * 100));
                }
            }
            // ritorna la stringa generata
            return sb.ToString();
        }

        // visualizza la descrizione del viso quando il mouse si trova nel rettangolo
        private void FacePhoto_MouseMove(object sender, MouseEventArgs e)
        {   // se la chiamata REST non è stata completata
            if (faces == null)
                return;
            // trova la posizione del mouse rispetto all'immagine
            Point mouseXY = e.GetPosition(FacePhoto);
            ImageSource imageSource = FacePhoto.Source;
            BitmapSource bitmapSource = (BitmapSource)imageSource;
            // regolazione della scala tra le dimensioni effettive  equelle visualizzate
            var scale = FacePhoto.ActualWidth / (bitmapSource.PixelWidth / resizeFactor);
            // controlla se la posizione del mouse è nel rettangolo
            bool mouseOverFace = false;
            for (int i = 0; i < faces.Length; ++i)
            {
                FaceRectangle fr = faces[i].FaceRectangle;
                double left = fr.Left * scale;
                double top = fr.Top * scale;
                double width = fr.Width * scale;
                double height = fr.Height * scale;
                // visualizza la descrizione del volto per questa faccia, se il mouse è nel suo rettangolo
                if (mouseXY.X >= left && mouseXY.X <= left + width && mouseXY.Y >= top && mouseXY.Y <= top + height)
                {
                    faceDescriptionStatusBar.Text = faceDescriptions[i];
                    mouseOverFace = true;
                    break;
                }
            }
            // se il mouse non è nel rettangolo
            if (!mouseOverFace)
                faceDescriptionStatusBar.Text = "Posizionare il puntatore del mouse su una faccia per vedere la descrizione del viso.";
        }

        // carica l'immagine e chiama Detect Faces
        private async Task<Face[]> UploadAndDetectFaces(string imageFilePath)
        {   // l'elenco degli attributi del viso da ritornare
            IEnumerable<FaceAttributeType> faceAttributes = new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Emotion, FaceAttributeType.Glasses, FaceAttributeType.Hair };
            // chiama le Face API
            try
            {
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    Face[] faces = await faceServiceClient.DetectAsync(imageFileStream, returnFaceId: true, returnFaceLandmarks: false, returnFaceAttributes: faceAttributes);
                    return faces;
                }
            }
            // intercetta e visualizza gli errori delle Face API
            catch (FaceAPIException f)
            {
                MessageBox.Show(f.ErrorMessage, f.ErrorCode);
                return new Face[0];
            }
            //  intercetta e visualizza tutti gli altri errori
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Errore!");
                return new Face[0];
            }
        }
    }
}