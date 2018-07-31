using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using Microsoft.SharePoint.WorkflowActions;
using Nintex.Workflow;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;

namespace Cassini.Workflows
{
    public partial class TaxonomyUpdateActivity : Nintex.Workflow.Activities.ProgressTrackingActivity
	{
        public static DependencyProperty __ListItemProperty = DependencyProperty.Register("__ListItem", typeof(SPItemKey), typeof(TaxonomyUpdateActivity));
        public static DependencyProperty __ContextProperty = DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(TaxonomyUpdateActivity));
        public static DependencyProperty __ListIdProperty = DependencyProperty.Register("__ListId", typeof(string), typeof(TaxonomyUpdateActivity));
        public static DependencyProperty lookupListProperty = DependencyProperty.Register("lookupList", typeof(string), typeof(TaxonomyUpdateActivity));
        public static DependencyProperty itemIDProperty = DependencyProperty.Register("itemID", typeof(string), typeof(TaxonomyUpdateActivity));
        public static DependencyProperty FieldValueProperty = DependencyProperty.Register("FieldValue", typeof(string), typeof(TaxonomyUpdateActivity));
        public static DependencyProperty TaxonomyFieldNameProperty = DependencyProperty.Register("TaxonomyFieldName", typeof(string), typeof(TaxonomyUpdateActivity));
        
        [ValidationOption(ValidationOption.Required), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public SPItemKey __ListItem
        {
            get { return (SPItemKey)base.GetValue(__ListItemProperty); }
            set { SetValue(__ListItemProperty, value); }
        }   

        [ValidationOption(ValidationOption.Required), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public WorkflowContext __Context
        {
            get
            {
                return (WorkflowContext)base.GetValue(__ContextProperty);
            }
            set
            {
                base.SetValue(__ContextProperty, value);
            }
        }

        [ValidationOption(ValidationOption.Required), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string __ListId
        {
            get
            {
                return (string)base.GetValue(__ListIdProperty);
            }
            set
            {
                base.SetValue(__ListIdProperty, value);
            }
        }

        [ValidationOption(ValidationOption.Required), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TaxFieldValue
        {
            get
            {
                return (string)base.GetValue(FieldValueProperty);
            }
            set
            {
                base.SetValue(FieldValueProperty, value);
            }
        }

        [ValidationOption(ValidationOption.Required), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string lookupList
        {
            get
            {
                return (string)base.GetValue(lookupListProperty);
            }
            set
            {
                base.SetValue(lookupListProperty, value);
            }
        }

        [ValidationOption(ValidationOption.Required), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string itemID
        {
            get {
                return (string)base.GetValue(itemIDProperty);
            }
            set {
                base.SetValue(itemIDProperty, value);
            }
        }

        [ValidationOption(ValidationOption.Required), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TaxonomyFieldName
        {
            get
            {
                return (string)base.GetValue(TaxonomyFieldNameProperty);
            }
            set
            {
                base.SetValue(TaxonomyFieldNameProperty, value);
            }
        }

        public TaxonomyUpdateActivity()
        {
        }

        private const String _ErrorSource = "Cassini.Workflows";


        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            // Get the workflow context for the workflow activity.
            NWWorkflowContext ctx = NWWorkflowContext.GetContext(
                   this.__Context,
                   new Guid(this.__ListId),
                   this.__ListItem.Id,
                   this.WorkflowInstanceId,
                   this);

            string resolvedFieldValue = ctx.AddContextDataToString(this.TaxFieldValue);
            string resolvedListID = ctx.AddContextDataToString(this.lookupList);
            int resolvedItemID = int.Parse(ctx.AddContextDataToString(this.itemID));
    
            base.LogProgressStart(ctx);

            Nintex.Workflow.Diagnostics.EventLogger.Log(
             "Updating '" + TaxonomyFieldName + "' Value '" + TaxFieldValue + "' ",
              Microsoft.SharePoint.Administration.TraceSeverity.Verbose,
              _ErrorSource);

            try
            {
                SPList list = ctx.Web.Lists.GetList(new Guid(resolvedListID), false);
                SPListItem item = list.GetItemById(resolvedItemID);
                TaxonomyField taxonomyField = list.Fields[TaxonomyFieldName] as TaxonomyField;
                TaxonomyFieldValue taxonomyFieldValue = new TaxonomyFieldValue(taxonomyField);

                String[] TermValues = resolvedFieldValue.Split('|');
                if (TermValues[0].Length > 0)
                {
                    // value|GUID
                    if (TermValues.Length == 2)
                    {
                        taxonomyFieldValue.Label = TermValues[0];
                        taxonomyFieldValue.TermGuid = TermValues[1];
                    }
                    taxonomyField.SetFieldValue(item, taxonomyFieldValue);
                    item.SystemUpdate(false);
                }
            }
            catch (Exception ex)
            {
                Nintex.Workflow.Diagnostics.EventLogger.Log(
                "Error occured while updating '" + TaxonomyFieldName + "' Value '" + resolvedFieldValue + "' " + ex.Message,
                Microsoft.SharePoint.Administration.TraceSeverity.Unexpected,
                _ErrorSource);
            }

            base.LogProgressEnd(ctx, executionContext);
            return ActivityExecutionStatus.Closed;
        }


        protected override ActivityExecutionStatus HandleFault(ActivityExecutionContext executionContext, Exception exception)
        {
            Nintex.Workflow.Diagnostics.ActivityErrorHandler.HandleFault(
                executionContext,
                exception,
                this.WorkflowInstanceId,
                "Error in TaxonomyUpdateActivity");

            return base.HandleFault(executionContext, exception);
        }
	}
}
