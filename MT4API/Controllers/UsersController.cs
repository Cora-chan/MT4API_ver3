using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using P23.MetaTrader4.Manager;
using P23.MetaTrader4.Manager.Contracts;
using P23.MetaTrader4.Manager.Contracts.Configuration;
using System.Web;
using MT4API;
using MT4API.Model;
using MT4API.Utils;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using System.Web.Script.Serialization;

namespace MT4API
{ 
    //******check the path which is absolute path to mtmanapi.dll in bin******// 
    public class UsersController : ApiController
    {
        public IList<UserRecord> users  = new List<UserRecord>();
  

        // GET api/<controller>
        public IList<UserRecord> Get()
        {
            //add absolute path to path to mtmanapi.dll in bin
            string path = @"D:\Yue's Project\MT4API_July_27th\MT4API_ver3\MT4API\bin\mtmanapi.dll";
            var are = new AutoResetEvent(false);
            using (var mt = new ClrWrapper(new ConnectionParameters
            {
                Login = 538,
                Password = "mvW9twXd",
                Server = "192.149.48.62:443"
            }, path))
            {
                mt.PumpingSwitch(i =>
                {
                    if (i == 0) // 0 - means pumping started
                        are.Set();
                });

                are.WaitOne();

                IList<UserRecord> users = mt.UsersGet();
                return users;
            }
        }

        // GET api/<controller>/5
        //dont change int id here! otherwise the router cannot recognize
        //path should be abosolute path to mtmanapi.dll in bin! otherwise will show exception!!!
        public UserRecord Get(int id)
        {
            //add absolute path to path to mtmanapi.dll in bin
            string path = @"D:\Yue's Project\MT4API_July_27th\MT4API_ver3\MT4API\bin\mtmanapi.dll";
            var are = new AutoResetEvent(false);
            using (var mt = new ClrWrapper(new ConnectionParameters
            {
                Login = 538,
                Password = "mvW9twXd",
                Server = "192.149.48.62:443"
            },path))
            {
                mt.PumpingSwitch(i =>
                {
                    if (i == 0) // 0 - means pumping started
                        are.Set();
                });
                are.WaitOne();
               UserRecord user = mt.UserRecordGet(id);
               return user;
        }
    }

        // POST api/<controller>
        public HttpResponseMessage Post(UserRecord user)
        {
            //add absolute path to path to mtmanapi.dll in bin
            string path = @"D:\Yue's Project\MT4API_July_27th\MT4API_ver3\MT4API\bin\mtmanapi.dll";
            using (var mt = new ClrWrapper
                (new ConnectionParameters 
                { Login = 538, Password = "mvW9twXd", Server = "192.149.48.62:443" },path))
            {            
                var result = mt.UserRecordNew(user);
                return Request.CreateResponse(HttpStatusCode.Created, user);
            }
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put(int id, UserRecord userRecord)
        {
            string path = @"D:\Yue's Project\MT4API_July_27th\MT4API_ver3\MT4API\bin\mtmanapi.dll";
            using (var mt = new ClrWrapper(new ConnectionParameters
            {
                Login = 538,
                Password = "mvW9twXd",
                Server = "192.149.48.62:443"
            }, path))
            {
                var result = mt.UserRecordUpdate(userRecord);
                return Request.CreateResponse(HttpStatusCode.OK,userRecord);
            }

        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}