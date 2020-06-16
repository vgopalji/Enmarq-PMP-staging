using CareStream.LoggerService;
using CareStream.Models;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareStream.Utility
{
    public interface IUserAttributeService
    {
        Task<UserAttributeModel> GetUserAttribute();

        Task UpsertUserAttributes(ExtensionModel extension);
    }

    public class UserAttributeService : IUserAttributeService
    {
        private readonly ILoggerManager _logger;

        public UserAttributeService(ILoggerManager logger)
        {
            _logger = logger;
        }

        public async Task<UserAttributeModel> GetUserAttribute()
        {
            try
            {
                var client = GraphClientUtility.GetGraphServiceClient();

                if (client == null)
                {
                    _logger.LogError("UserAttributeService-GetUserAttribute: Unable to create proxy for the Azure AD B2C graph client");
                    return null;
                }
                _logger.LogInfo("UserAttributeService-GetUserAttribute: [Started] to fetch user attribute in Azure AD B2C");

                var extensionName = GraphClientUtility.ExtensionName;

                var schemaExtensions = await client.SchemaExtensions.Request().GetAsync();


                var userAttributeModel = new UserAttributeModel();
                var extensionSchemas = new List<ExtensionSchema>();
                while (schemaExtensions.NextPageRequest != null)
                {
                    foreach (SchemaExtension extension in schemaExtensions.CurrentPage)
                    {
                        try
                        {
                            if (extension.Id.Contains(extensionName))
                            {
                                userAttributeModel.Id = extension.Id;

                                foreach (var item in extension.Properties)
                                {
                                    try
                                    {
                                        ExtensionSchema extensionSchema = new ExtensionSchema();

                                        extensionSchema.Name = item.Name;
                                        extensionSchema.DataType = item.Type;
                                        extensionSchemas.Add(extensionSchema);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError($"ExtensionController-GetExtensions: fail to add extension [name:{item.Name}] to collection ");
                                        _logger.LogError(ex);
                                    }
                                }
                                userAttributeModel.ExtensionSchemas = extensionSchemas;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"ExtensionController-GetExtensions: fail to add extension [name:{extension.Id}] to collection ");
                            _logger.LogError(ex);
                        }
                    }


                    schemaExtensions = await schemaExtensions.NextPageRequest.GetAsync();
                }

                return userAttributeModel;

            }
            catch (Exception ex)
            {
                _logger.LogError("ExtensionController-GetExtensions: Exception occured....");
                _logger.LogError(ex);
            }
            return null;
        }

        public async Task UpsertUserAttributes(ExtensionModel extension)
        {
            try
            {
                _logger.LogInfo("UserAttributeService-UpsertUserAttributes: [Started] creation of user attribute in Azure AD B2C");

                if (extension == null)
                {
                    _logger.LogError("UserAttributeService-UpsertUserAttributes: Input cannot be null");
                    return;
                }


                #region Validation 

                if (extension.TargetObjects == null)
                {
                    _logger.LogError("UserAttributeService-UpsertUserAttributes: Target Object for creation of custom attribute cannot be empty");
                    return;
                }

                if (string.IsNullOrEmpty(extension.Name)
                    && string.IsNullOrEmpty(extension.DataType)
                    && !extension.TargetObjects.Any())
                {
                    _logger.LogError("UserAttributeService-UpsertUserAttributes: Input [Name | Data Type | Target Obejct] for creation of custom attribute cannot be empty");
                    return;
                }
                #endregion

                var client = GraphClientUtility.GetGraphServiceClient();

                if (client == null)
                {
                    _logger.LogError("UserAttributeService-UpsertUserAttributes: Unable create graph proxy to access Azure AD B2C");
                    return;
                }


                var taskSchemaName = await CheckIfExtensionExist(GraphClientUtility.ExtensionName);
                var schemaName = string.Empty;

                if (taskSchemaName != null)
                {
                    schemaName = "";
                }

                if (string.IsNullOrEmpty(schemaName))
                {
                    //var schemaExtension = new SchemaExtension
                    //{
                    //    Id = GraphClientUtility.ExtensionName,
                    //    Description = extension.Description,
                    //    TargetTypes = extension.TargetObjects,
                    //    Properties = new List<ExtensionSchemaProperty>()
                    //    {
                    //        new ExtensionSchemaProperty
                    //        {
                    //            Name= extension.Name,
                    //            Type = extension.DataType
                    //        }
                    //    }
                    //};

                    var schemaExtension = new SchemaExtension
                    {
                        Id = "prasadtest3vikas",
                        Description = "Prasad Learn training courses extensions",
                        Status = "InDevelopment",
                        TargetTypes = new List<String>()
                {
                    "User"
                },
                        Properties = new List<ExtensionSchemaProperty>()
                {
                    new ExtensionSchemaProperty
                    {
                        Name = "courseId",
                        Type = "Integer"
                    },
                    new ExtensionSchemaProperty
                    {
                        Name = "courseName",
                        Type = "String"
                    },
                    new ExtensionSchemaProperty
                    {
                        Name = "courseType",
                        Type = "String"
                    }
                }
                    };

                    try
                    {
                        var response = await client.SchemaExtensions
                            .Request()
                            .AddAsync(schemaExtension);

                        var result = response;

                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex);
                    }

                }
                else
                {
                    var schemaExtension = new SchemaExtension
                    {
                        TargetTypes = extension.TargetObjects,
                        Properties = new List<ExtensionSchemaProperty>()
                        {
                            new ExtensionSchemaProperty
                            {
                                Name=extension.Name,
                                Type = extension.DataType
                            }
                        }
                    };
                    await client.SchemaExtensions[schemaName].Request().UpdateAsync(schemaExtension);
                }


                _logger.LogInfo("UserAttributeService-UpsertUserAttributes: [Completed] creation of user attribute in Azure AD B2C");

            }
            catch (Exception ex)
            {
                _logger.LogError("UserAttributeService-UpsertUserAttributes: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
        }

        private async Task<string> CheckIfExtensionExist(string schemaName)
        {
            var retVal = string.Empty;
            try
            {
                var client = GraphClientUtility.GetGraphServiceClient();

                if (client == null)
                {
                    _logger.LogError("UserAttributeService-CheckIfExtensionExist: Unable to create proxy for the Azure AD B2C graph client");
                    return retVal;
                }

                var schemaExtensions = await client.SchemaExtensions.Request().GetAsync();

                while (schemaExtensions.NextPageRequest != null)
                {
                    foreach (SchemaExtension extension in schemaExtensions.CurrentPage)
                    {
                        if (extension.Id.Contains(schemaName))
                        {
                            retVal = extension.Id;
                            break;
                        }
                    }
                    if (!string.IsNullOrEmpty(retVal))
                    {
                        break;
                    }
                    schemaExtensions = await schemaExtensions.NextPageRequest.GetAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("UserAttributeService-CheckIfExtensionExist: Exception occured....");
                _logger.LogError(ex);
            }
            return retVal;
        }
    }
}
