using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Xml;
using Nintex.Workflow;
using Nintex.Workflow.Common;
using Nintex.Workflow.Administration;
using Microsoft.SharePoint.Utilities;
using System.Collections.ObjectModel;
using System.IO;

namespace Cassini.Workflows.Features.Feature1
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("b0d5287f-e88a-4752-a9a5-0fbbb82fc802")]
    public class Feature1EventReceiver : SPFeatureReceiver
    {
        const string pathToNWA = "TaxonomyUpdateModule.nwa";
        const string adapterType = "Cassini.Workflows.TaxonomyUpdateAdapter";
        const string adapterAssembly = "Cassini.Workflows, Version=1.0.0.0, Culture=neutral, PublicKeyToken=79e8a3b5a648200b";

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWebApplication parent = (SPWebApplication)properties.Feature.Parent;
            
            // First step is register the action to the Nintex Workflow database
            XmlDocument nwaXml = GetNWADefinition(properties);

            ActivityReference newActivityReference = ActivityReference.ReadFromNWA(nwaXml);

            ActivityReference action = ActivityReferenceCollection.FindByAdapter(newActivityReference.AdapterType, newActivityReference.AdapterAssembly);

            if (action != null)
            {
                // update the details if the adapter already exists
                ActivityReferenceCollection.UpdateActivity(action.ActivityId, newActivityReference.Name, newActivityReference.Description, newActivityReference.Category, newActivityReference.ActivityAssembly, newActivityReference.ActivityType, newActivityReference.AdapterAssembly, newActivityReference.AdapterType, newActivityReference.HandlerUrl, newActivityReference.ConfigPage, newActivityReference.RenderBehaviour, newActivityReference.Icon, newActivityReference.ToolboxIcon, newActivityReference.WarningIcon, newActivityReference.QuickAccess, newActivityReference.ListTypeFilter);
            }
            else
            {
                ActivityReferenceCollection.AddActivity(newActivityReference.Name, newActivityReference.Description, newActivityReference.Category, newActivityReference.ActivityAssembly, newActivityReference.ActivityType, newActivityReference.AdapterAssembly, newActivityReference.AdapterType, newActivityReference.HandlerUrl, newActivityReference.ConfigPage, newActivityReference.RenderBehaviour, newActivityReference.Icon, newActivityReference.ToolboxIcon, newActivityReference.WarningIcon, newActivityReference.QuickAccess, newActivityReference.ListTypeFilter);
                action = ActivityReferenceCollection.FindByAdapter(newActivityReference.AdapterType, newActivityReference.AdapterAssembly);
            }

            // Second step is to modify the web.config file to allow use of the activity in declarative workflows
            string activityTypeName = string.Empty;
            string activityNamespace = string.Empty;
            Utility.ExtractNamespaceAndClassName(action.ActivityType, out activityTypeName, out activityNamespace);
            AuthorisedTypes.InstallAuthorizedWorkflowTypes(parent, action.ActivityAssembly, activityNamespace, activityTypeName);

            // Third step is to activate the action for the farm
            ActivityActivationReference reference = new ActivityActivationReference(action.ActivityId, Guid.Empty, Guid.Empty);
            reference.AddOrUpdateActivationReference();
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPWebApplication parent = (SPWebApplication)properties.Feature.Parent;

            ActivityReference action = ActivityReferenceCollection.FindByAdapter(adapterType, adapterAssembly);
            if (action != null)
            {
                // Remove the action definition from the workflow configuration database if the Feature is not activated elsewhere
                if (!IsFeatureActivatedInAnyWebApp(parent, properties.Definition.Id))
                    ActivityReferenceCollection.RemoveAction(action.ActivityId);

                string activityTypeName = string.Empty;
                string activityNamespace = string.Empty;
                Utility.ExtractNamespaceAndClassName(action.ActivityType, out activityTypeName, out activityNamespace);

                // Remove the web.config entry
                Collection<SPWebConfigModification> modifications = parent.WebConfigModifications;
                foreach (SPWebConfigModification modification in modifications)
                {
                    if (modification.Owner == AuthorisedTypes.OWNER_TOKEN) // OWNER_TOKEN is the owner for any web config modification added by Nintex Workflow
                    {
                        if (IsAuthorizedTypeMatch(modification.Value, action.ActivityAssembly, activityTypeName, activityNamespace))
                        {
                            modifications.Remove(modification);
                            parent.Farm.Services.GetValue<SPWebService>().ApplyWebConfigModifications();
                            break;
                        }
                    }
                }

            }
        }

        private bool IsAuthorizedTypeMatch(string modification, string activityAssembly, string activityType, string activityNamespace)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(modification);
            if (doc.FirstChild.Name == "authorizedType")
            {
                return (doc.SelectSingleNode("//@TypeName").Value == activityType
                        && doc.SelectSingleNode("//@Namespace").Value == activityNamespace
                        && doc.SelectSingleNode("//@Assembly").Value == activityAssembly);

            }

            return false;
        }

        private bool IsFeatureActivatedInAnyWebApp(SPWebApplication thisWebApplication, Guid thisFeatureId)
        {
            SPWebService webService = SPWebService.ContentService;
            if (webService == null)
                throw new ApplicationException("Cannot access ContentService");

            SPWebApplicationCollection webApps = webService.WebApplications;
            foreach (SPWebApplication webApp in webApps)
            {
                if (webApp != thisWebApplication)
                    if (webApp.Features[thisFeatureId] != null)
                        return true;
            }

            return false;
        }

        private XmlDocument GetNWADefinition(SPFeatureReceiverProperties properties)
        {
            using (Stream stream = properties.Definition.GetFile(pathToNWA))
            {
                XmlDocument nwaXml = new XmlDocument();
                nwaXml.Load(stream);

                return nwaXml;
            }
        }
    }
}
