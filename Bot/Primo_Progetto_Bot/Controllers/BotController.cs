using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;

namespace Primo_Progetto_Bot.Controllers
{
    /* questo controller ASP è creato per gestire una richiesta, Dependency Injection
     * fornirà l'adattatore e l'implementazione IBot è in fase di esecuzione, più
     * implementazioni diverse di IBot  in esecuzione in endpoint diversi possono essere
     * ottenute specificando un tipo più specifico per il parametro del costruttore di bot */

    [Route("api/messages")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter Adapter;
        private readonly IBot Bot;

        public BotController(IBotFrameworkHttpAdapter adapter, IBot bot)
        {
            Adapter = adapter;
            Bot = bot;
        }

        [HttpPost]
        public async Task PostAsync()
        {
            // The adapter will invoke the bot.
            /* delegare l'elaborazione del POST HTTP all'adapter
             * l'adapter richiamerà il bot  */
            await Adapter.ProcessAsync(Request, Response, Bot);
        }
    }
}