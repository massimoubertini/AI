using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Primo_Progetto_Bot
{
    public class AdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        public AdapterWithErrorHandler(IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger)
            : base(configuration, logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                // registrare eventuali eccezioni trapelate dall'app
                logger.LogError($"Exception caught : {exception.Message}");
                // invia un catch-all all'utente
                await turnContext.SendActivityAsync("Spiacenti, sembra che qualcosa non funzioni!");
            };
        }
    }
}