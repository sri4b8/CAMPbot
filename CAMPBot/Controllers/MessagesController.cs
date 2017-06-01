using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace CAMPBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {

            string token=string.Empty;
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                var fromname = activity.From.Name??"";
                var fromId = activity.From.Id??"";


                var name = activity.Name??"";
                var id = activity.Id??"";

                string result = "fromname:" + fromname + ",fromId:" + fromId ;

                int length = (activity.Text ?? string.Empty).Length;
                try
                {
                     var queryString = Request.RequestUri.Query;
                    if (!String.IsNullOrWhiteSpace(queryString))
                    {
                        token = HttpUtility.ParseQueryString(
                            queryString.Substring(1))["username"];
                    }
                }
                catch (Exception ex)
                {

                    token = ex.Message;
                }
                Activity reply = activity.CreateReply($"hello {result} you sent {activity.Text} which was {length} chars");

                await connector.Conversations.ReplyToActivityAsync(reply);
                // await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}