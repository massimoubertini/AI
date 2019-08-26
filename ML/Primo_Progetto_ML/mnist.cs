using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.AI.MachineLearning.Preview;

namespace MNIST_Demo
{
    // permette l'inizializzazione degli input in un formato (il VideoFrame) che si aspetta il modello
    public sealed class MNISTModelInput
    {
        public VideoFrame Input3 { get; set; }
    }

    /* rappresenta l'output del modello dopo l'analisi ed è definito come un dizionario
     * rappresentante la predizione e la percentuale di correttezza della previsione */

    public sealed class MNISTModelOutput
    {
        public IList<float> Plus214_Output_0 { get; set; }

        public MNISTModelOutput()
        {
            this.Plus214_Output_0 = new List<float>();
        }
    }

    // è la rappresentazione del modello che permette di fare la valutazione dati gli input e gli output

    public sealed class MNISTModel
    {
        private LearningModelPreview learningModel;
        /* metodo CreateMNISTModel: è stato generato sempre automaticamente e tramite le API
         * contenute in Windows.AI.MachineLearning, permette di precaricare il modello con la
         * chiamata a LearningModelPreview.LoadModelFromStorageFileAsync */

        public static async Task<MNISTModel> CreateMNISTModel(StorageFile file)
        {
            LearningModelPreview learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            MNISTModel model = new MNISTModel();
            model.learningModel = learningModel;
            return model;
        }

        public async Task<MNISTModelOutput> EvaluateAsync(MNISTModelInput input)
        {
            MNISTModelOutput output = new MNISTModelOutput();
            LearningModelBindingPreview binding = new LearningModelBindingPreview(learningModel);
            binding.Bind("Input3", input.Input3);
            binding.Bind("Plus214_Output_0", output.Plus214_Output_0);
            LearningModelEvaluationResultPreview evalResult = await learningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}