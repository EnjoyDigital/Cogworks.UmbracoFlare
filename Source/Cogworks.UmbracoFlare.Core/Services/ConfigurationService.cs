using Cogworks.UmbracoFlare.Core.Model;
using Cogworks.UmbracoFlare.Core.Extensions;
using Cogworks.UmbracoFlare.Core.Models;
using System;
using System.IO;
using System.Xml.Serialization;
using Umbraco.Cms.Core.Hosting;
using Microsoft.Extensions.Logging;

namespace Cogworks.UmbracoFlare.Core.Services
{
    public interface IConfigurationService
    {
        bool ConfigurationFileHasData(UmbracoFlareConfigModel configurationFile);

        UmbracoFlareConfigModel LoadConfigurationFile();

        UmbracoFlareConfigModel SaveConfigurationFile(UmbracoFlareConfigModel configurationObject);
    }

    public class ConfigurationService : IConfigurationService
    {
        private readonly ILogger<IConfigurationService> logger;
        private readonly IHostingEnvironment hostingEnvironmen;

        public ConfigurationService(ILogger<IConfigurationService> logger, IHostingEnvironment hostingEnvironmen)
        {
            this.logger = logger;
            this.hostingEnvironmen = hostingEnvironmen;
        }

        public bool ConfigurationFileHasData(UmbracoFlareConfigModel configurationFile)
        {
            return configurationFile.AccountEmail.HasValue() && configurationFile.ApiKey.HasValue();
        }

        public UmbracoFlareConfigModel LoadConfigurationFile()
        {
            try
            {
                var configurationFilePath = hostingEnvironmen.MapPathWebRoot(ApplicationConstants.ConfigurationFile.ConfigurationFilePath);
                var serializer = new XmlSerializer(typeof(UmbracoFlareConfigModel));

                using (var reader = new StreamReader(configurationFilePath))
                {
                    return (UmbracoFlareConfigModel)serializer.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                logger.LogError($"Could not load the file in this path {ApplicationConstants.ConfigurationFile.ConfigurationFilePath}", e);
            }

            return new UmbracoFlareConfigModel();
        }

        public UmbracoFlareConfigModel SaveConfigurationFile(UmbracoFlareConfigModel configurationFile)
        {
            try
            {
                var configurationFilePath = (ApplicationConstants.ConfigurationFile.ConfigurationFilePath);
                var serializer = new XmlSerializer(typeof(UmbracoFlareConfigModel));

                using (var writer = new StreamWriter(configurationFilePath))
                {
                    serializer.Serialize(writer, configurationFile);
                }

                return configurationFile;
            }
            catch (Exception e)
            {
                logger.LogError($"Could not save the configuration file in this path {ApplicationConstants.ConfigurationFile.ConfigurationFilePath}", e);
            }

            return null;
        }
    }
}