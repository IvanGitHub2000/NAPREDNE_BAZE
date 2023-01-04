using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using NBP_I.Session;

namespace NBP_I.Hubs
{
    public class BaseHub : Hub
    {
        public int? UserId => Context.GetHttpContext().Session.GetInt32(SessionKeys.UserId);
        public string UserName => Context.GetHttpContext().Session.GetString(SessionKeys.Username);
    }
}
