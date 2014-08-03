﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snowflake.Core.API;
using Newtonsoft.Json;
namespace Snowflake.Core.API.JSAPI
{
    public static class JSBridge
    {
        public static string GetAllPlatforms(JSRequest request)
        {
            return JSBridge.ProcessJSONP(CoreAPI.GetAllPlatforms(), request);
        }

        public static string GetGamesByPlatform(JSRequest request)
        {
            return JSBridge.ProcessJSONP(CoreAPI.GetGamesByPlatform(request.MethodParameters["platformid"]), request);
        }
        
        public static string Work(JSRequest request)
        {
            System.Threading.Thread.Sleep(100000);
            return "done";
        }

        private static string ProcessJSONP(dynamic output, JSRequest request){
            if (request.MethodParameters.ContainsKey("jsoncallback"))
            {
                return request.MethodParameters["jsoncallback"] + "(" + JsonConvert.SerializeObject(output) + ");";
            }
            else
            {
                return JsonConvert.SerializeObject(output);
            }
        }
    }
}