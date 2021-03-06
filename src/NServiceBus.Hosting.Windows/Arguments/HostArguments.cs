namespace NServiceBus.Hosting.Windows.Arguments
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using NDesk.Options;

    class HostArguments
    {
        OptionSet installOptions;
        OptionSet uninstallOptions;

        public HostArguments(IEnumerable<string> args)
        {
            DependsOn = new List<string>();
            ScannedAssemblies = new List<string>();

            uninstallOptions = new OptionSet
                {
                    {
                        "?|h|help", "Help about the command line options.", key => { Help = true; }
                    },
                    {
                        "uninstall",
                        "Uninstall the endpoint as a Windows service."
                        , s => { }
                    },
                    {
                        "serviceName=",
                        "Specify the service name for the installed service."
                        , s => { ServiceName = s; }
                    },
                    {
                        "sideBySide",
                        "Install the service with the version included in the service name. This allows running multiple endpoints side by side when doing hot deployments."
                        , s => { SideBySide = true; }
                    },
                };

            installOptions = new OptionSet
                {
                    {
                        "?|h|help",
                        "Help about the command line options.",
                        key => { Help = true; }
                    },
                    {
                        "install",
                        "Install the endpoint as a Windows service."
                        , s => { Install = true; }
                    },
                    {
                        "serviceName=",
                        "Specify the service name for the installed service."
                        , s => { ServiceName = s; }
                    },
                    {
                        "displayName=",
                        "Friendly name for the installed service."
                        , s => { DisplayName = s; }
                    },
                    {
                        "description=",
                        "Description for the service."
                        , s => { Description = s; }
                    },
                    {
                        "endpointConfigurationType=",
                        "Specify the type implementing IConfigureThisEndpoint that should be used."
                        , s => { EndpointConfigurationType = s; }
                    },
                    {
                        "dependsOn=",
                        "Specifies the names of services or groups which must start before this service."
                        , s =>
                        {
                            foreach (var d in s.Split(','))
                            {
                                DependsOn.Add(d);
                            }
                        }
                    },
                    {
                        "sideBySide",
                        "Install the service with the version included in the service name. This allows running multiple endpoints side by side when doing hot deployments."
                        , s => { SideBySide = true; }
                    },
                    {
                        "endpointName=",
                        "The name of this endpoint."
                        , s => { EndpointName = s; }
                    },
                    {
                        "username=",
                        "Username for the account the service should run under."
                        , s => { Username = s; }
                    },
                    {
                        "password=",
                        "Password for the service account."
                        , s => { Password = s; }
                    },
                    {
                        "startManually",
                        "Specifies that the service should start manually."
                        , s => { StartManually = true; }
                    },
                    {
                        "scannedAssemblies=",
                        "Configures NServiceBus to use the types found in the given assemblies."
                        , s => ScannedAssemblies.Add(s)
                    },
                };

            try
            {
                OtherArgs = installOptions.Parse(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Help = true;
            }
        }

        public bool Install { get; private set; }
        public List<string> OtherArgs { get; set; }
        public bool SideBySide { get; set; }
        public bool Help { get; set; }
        public string ServiceName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string EndpointConfigurationType { get; set; }
        public List<string> DependsOn { get; }
        public bool StartManually { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string EndpointName { get; set; }
        public List<string> ScannedAssemblies { get; }

        public void PrintUsage()
        {
            var stringBuilder = new StringBuilder();

            var helpText = string.Empty;
            var callingAssembly = Assembly.GetCallingAssembly();
            using (var stream = callingAssembly.GetManifestResourceStream("NServiceBus.Hosting.Windows.Content.Help.txt"))
            {
                if (stream != null)
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        helpText = streamReader.ReadToEnd();
                    }
                }
            }

            installOptions.WriteOptionDescriptions(new StringWriter(stringBuilder));
            var installOptionsHelp = stringBuilder.ToString();

            stringBuilder.Clear();
            uninstallOptions.WriteOptionDescriptions(new StringWriter(stringBuilder));
            var uninstallOptionsHelp = stringBuilder.ToString();

            Console.Out.WriteLine(helpText, installOptionsHelp, uninstallOptionsHelp);
        }
    }
}