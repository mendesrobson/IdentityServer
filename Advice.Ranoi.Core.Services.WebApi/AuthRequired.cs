using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Advice.Ranoi.Core.Services.WebApi
{
    public class ControllerRegistry
    {
        public String Name { get; set; }
        public List<String> Methods { get; set; }

        public ControllerRegistry()
        {
            Methods = new List<string>();
        }
    }

    public class AuthorizationRequiredAttribute : TypeFilterAttribute
    {
        public AuthorizationRequiredAttribute() : base(typeof(AuthorizationRequirementFilter))
        {
        }
    }

    public class AuthorizationRequirementFilter : IAuthorizationFilter
    {
        private IMemoryCache _cache;

        public AuthorizationRequirementFilter(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

            if (controllerActionDescriptor != null)
            {
                var service = controllerActionDescriptor.ControllerTypeInfo.Namespace.Replace("Advice.", "").Replace(".Services.WebApi", "");
                var controller = controllerActionDescriptor.ControllerName.Replace("Controller", "");
                var methodName = controllerActionDescriptor.MethodInfo.Name;

                //procura o controller no cache pra ver se tem.
                ControllerRegistry registry = null;

                if (!_cache.TryGetValue(controller, out registry))
                {
                    //Roadmap:

                    //(1) - Fazer fetch via REST quando precisar
                    //(2) - Mover tudo para Core/Nugets
                    //(3) - Testar se Attribute herda com classe
                    //(4) - Endpoint para resetar cache

                    String token = "";

                    IRestClient client;
                    IRestRequest request;
                    IRestResponse response;

                    if (!_cache.TryGetValue("Token", out token))
                    {
                        client = new RestClient("http://localhost:1241");
                        request = new RestRequest("api/Token", Method.POST);
                        var body = new { Login = "will.johnson", Password = "1234" };
                        request.AddHeader("Content-Type", "application/json");
                        request.AddHeader("Accept", "application/json");
                        //request.AddParameter("application/json", body, ParameterType.RequestBody);
                        request.AddJsonBody(body);
                        response = client.Execute(request);
                        token = response.Content.Replace("\"", "");
                        _cache.Set("Token", token);
                    }

                    client = new RestClient("http://localhost:1240"); //8080 8000
                    request = new RestRequest("api/services/{id}");
                    request.AddUrlSegment("id", service + "." + controller);
                    request.AddHeader("Authorization", "Bearer " + token);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Accept", "application/json");
                    response = client.Execute(request);
                    var content = response.Content.Replace("[", "").Replace("]", "").Split(',').Select(x => x.Replace("\"", ""));

                    registry = new ControllerRegistry();
                    registry.Name = controller;
                    foreach (var item in content)
                    {
                        registry.Methods.Add(item);
                    }

                    _cache.Set(controller, registry);
                }

                var methodIndex = registry.Methods.IndexOf(methodName);

                var claim = context.HttpContext.User.Claims.SingleOrDefault(x => x.Type.Equals("Service." + service + "." + controller));

                if (claim == null)
                    context.Result = new UnauthorizedResult();

                var claimValue = claim.Value.ElementAt(methodIndex);

                if (claimValue != '1')
                    context.Result = new UnauthorizedResult();
            }
        }
    }
}
