using Autofac;
using Advice.Ranoi.Core.Data;
using Advice.Ranoi.Core.Data.Interfaces;
using Advice.Ranoi.Core.Domain.Interfaces;
using Advice.Ranoi.Core.Domain.Interfaces.Factories;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Advice.Ranoi.Core.Services.WebApi
{
    public class MainModule : Autofac.Module
    {
        private static void LoadDlls(string packageAddress)
        {
            var dlls = System.IO.Directory.GetFiles(packageAddress).Where(x => x.Contains("Advice.") && x.EndsWith(".dll")).ToList();

            foreach (var umaDLl in dlls)
            {
                AssemblyName name = AssemblyName.GetAssemblyName(umaDLl);
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().OrderBy(x => x.FullName).ToList();

            foreach (var dll in dlls)
            {
                AssemblyName name = AssemblyName.GetAssemblyName(dll);

                if (!assemblies.Any(x => x.FullName.Equals(name.FullName)))
                {
                    try
                    {
                        Console.WriteLine("Carregando: " + name.FullName);
                        assemblies.Add(AppDomain.CurrentDomain.Load(name));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Não consegui carregar " + name);
                    }
                }
            }
        }

        protected override void Load(ContainerBuilder builder)
        {
            //(0) - encontrar o starting point da app
            var currentAssembly = Assembly.GetEntryAssembly();
            var locationSplit = currentAssembly.Location.Split('\\');
            var fullPath = "";
            for (var i = 0; i < locationSplit.Length - 1; i++)
            {
                fullPath += locationSplit[i] + "\\";
            }

            //(1) - run nuget.exe para encontrar os paths de packages
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet.exe",
                    Arguments = "nuget locals all --list",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            List<String> nugetLocations = new List<string>();

            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                if (line.Contains("global"))
                    nugetLocations.Add(line.Replace("info : http-cache: ", "").Replace("info : global-packages: ", ""));
            }

            //(2) - json parse *.deps.json para encontrar os pacotes Advice e versões necessárias
            var dependencyFiles = System.IO.Directory.EnumerateFiles(fullPath, "*.deps.json", System.IO.SearchOption.AllDirectories).First();

            var dependencyFileText = System.IO.File.ReadAllText(dependencyFiles);

            dynamic d = JObject.Parse(dependencyFileText);

            var packagesTemp = (IEnumerable<JProperty>)d.libraries.Properties();
            var packages = packagesTemp.Select(p => p.Name).Where(x => x.StartsWith("Advice.")).ToList();

            //(3) - carregar os assemblies "Advice.*.dll" para o AppDomain
            foreach (var package in packages)
            {
                var packageSplit = package.Split('/');
                var packageAddress = nugetLocations.First() + packageSplit[0].ToLower() + "\\" + packageSplit[1] + "\\lib\\netcoreapp2.0";

                if (!System.IO.Directory.Exists(packageAddress))
                    continue;

                LoadDlls(packageAddress);
            }

            LoadDlls(fullPath);

            //(4) - rodar auto-register - nem preciso descrever isso srsrs
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            #region Factory Registro

            loadedAssemblies.Where(x => x.FullName.Contains("Advice.")).ToList().ForEach(assembly =>
            {
                builder
                    .RegisterAssemblyTypes(assembly)
                    .Where(type => type.FullName.EndsWith("Factory"))
                    .AsImplementedInterfaces();
            });

            #endregion Factory Registro

            #region Builder Registro --ctor

            loadedAssemblies.Where(x => x.FullName.Contains("Advice.")).ToList().ForEach(assembly =>
            {
                builder
                    .RegisterAssemblyTypes(assembly)
                    .Where(type => type.FullName.EndsWith("Builder"))
                    .AsImplementedInterfaces();
            });

            #endregion Factory Registro

            #region Registro de serviço

            loadedAssemblies.Where(x => x.FullName.Contains("Advice.")).ToList().ForEach(assembly =>
            {
                builder
                    .RegisterAssemblyTypes(assembly)
                    .Where(type => type.FullName.EndsWith("Services"))
                    .AsImplementedInterfaces();

                builder
                    .RegisterAssemblyTypes(assembly)
                    .Where(type => type.FullName.EndsWith("Service"))
                    .AsImplementedInterfaces();
            });

            #endregion Registro de serviço

            #region Registro de Provedor

            loadedAssemblies.Where(x => x.FullName.Contains("Advice.")).ToList().ForEach(assembly =>
            {
                builder
                    .RegisterAssemblyTypes(assembly)
                    .Where(type => type.FullName.EndsWith("Provider"))
                    .AsImplementedInterfaces();
            });

            #endregion Provider Registration

            #region IDeclaredFactory Cadastro

            var targetType = typeof(IDeclaredFactory<IRepository<IEntity>>);
            List<Type> list = new List<Type>();

            foreach (var assembly in loadedAssemblies.Where(x => x.FullName.Contains("Advice.")))
            {
                var types = assembly.GetTypes().Where(x => targetType.IsAssignableFrom(x) && !x.IsInterface).ToList();
                list.AddRange(types);
            }

            builder.RegisterTypes(list.ToArray()).As<IDeclaredFactory<IRepository<IEntity>>>();
            Boolean first = true;

            /*list.ForEach(type =>
            {
                builder.RegisterType(type).As(targetType).PreserveExistingDefaults();

            });*/

            #endregion IDeclaredFactory Registration

            #region IUnitOfWorkFactory Ciclo de Vida Override

            //builder.RegisterType<XDomainEventRepositoryFactory>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<Context>().As<IContext>().InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWorkFactory>().As<IUnitOfWorkFactory>().InstancePerLifetimeScope();
            builder.RegisterType<MongoEventParser>().As<IObserver>().InstancePerLifetimeScope();

            #endregion IUnitOfWorkFactory Lifecycle Override

            ExtraLoad(builder);
        }

        protected virtual void ExtraLoad(ContainerBuilder builder)
        {
        }
    }
}
