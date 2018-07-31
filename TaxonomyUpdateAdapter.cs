using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using System.Workflow.ComponentModel;
using System.Collections;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WorkflowActions;

using Nintex.Workflow;
using Nintex.Workflow.Activities.Adapters;

namespace Cassini.Workflows
{
    public class TaxonomyUpdateAdapter : GenericRenderingAction
    {
        const string Parameter_FieldValue = "FieldValue";
        const string Parameter_TaxonomyFieldName = "TaxonomyFieldName";
        const string Parameter_lookupList = "lookupList";
        const string Parameter_itemID = "itemID";

        public override NWActionConfig GetDefaultConfig(GetDefaultConfigContext context)
        {
            NWActionConfig c = new NWActionConfig(this);

            c.Parameters = new ActivityParameter[4];

            c.Parameters[0] = new ActivityParameter();
            c.Parameters[0].Name = Parameter_lookupList;
            c.Parameters[0].PrimitiveValue = new PrimitiveValue();

            c.Parameters[1] = new ActivityParameter();
            c.Parameters[1].Name = Parameter_itemID;
            c.Parameters[1].PrimitiveValue = new PrimitiveValue();

            c.Parameters[2] = new ActivityParameter();
            c.Parameters[2].Name = Parameter_FieldValue;
            c.Parameters[2].PrimitiveValue = new PrimitiveValue();

            c.Parameters[3] = new ActivityParameter();
            c.Parameters[3].Name = Parameter_TaxonomyFieldName;
            c.Parameters[3].PrimitiveValue = new PrimitiveValue();

            c.TLabel = ActivityReferenceCollection.FindByAdapter(this).Name;
            c.IsValid = false;
            return c;
        }

        public override bool ValidateConfig(ActivityContext context)
        {
            bool isValid = true;

            Dictionary<string, ActivityParameterHelper> parameters = context.Configuration.GetParameterHelpers();

            if (string.IsNullOrEmpty(parameters[Parameter_lookupList].Value))
            {
                validationSummary.AddError("List", ValidationSummaryErrorType.CannotBeBlank);
                isValid &= false;
            }

            if (string.IsNullOrEmpty(parameters[Parameter_itemID].Value))
            {
                validationSummary.AddError("List Item ID ", ValidationSummaryErrorType.CannotBeBlank);
                isValid &= false;
            }

            if (string.IsNullOrEmpty(parameters[Parameter_FieldValue].Value))
            {
                validationSummary.AddError("FieldValue", ValidationSummaryErrorType.CannotBeBlank);
                isValid &= false;
            }

            if (string.IsNullOrEmpty(parameters[Parameter_TaxonomyFieldName].Value))
            {
                validationSummary.AddError("Event Source", ValidationSummaryErrorType.CannotBeBlank);
                isValid &= false;
            }

            return isValid;
        }


        public override NWActionConfig GetConfig(RetrieveConfigContext context)
        {
            NWActionConfig config = this.GetDefaultConfig(context);

            Dictionary<string, ActivityParameterHelper> parameters = config.GetParameterHelpers();

            ActivityParameterHelper.RetrieveValue(context.Activity, TaxonomyUpdateActivity.lookupListProperty, parameters[Parameter_lookupList].Parameter, context);
            ActivityParameterHelper.RetrieveValue(context.Activity, TaxonomyUpdateActivity.itemIDProperty, parameters[Parameter_itemID].Parameter, context);
            ActivityParameterHelper.RetrieveValue(context.Activity, TaxonomyUpdateActivity.FieldValueProperty, parameters[Parameter_FieldValue].Parameter, context);
            ActivityParameterHelper.RetrieveValue(context.Activity, TaxonomyUpdateActivity.TaxonomyFieldNameProperty, parameters[Parameter_TaxonomyFieldName].Parameter, context);
            return config;
        }

        public override CompositeActivity AddActivityToWorkflow(PublishContext context)
        {
            TaxonomyUpdateActivity activity = new TaxonomyUpdateActivity();

            Dictionary<string, ActivityParameterHelper> parameters = context.Config.GetParameterHelpers();

            parameters[Parameter_lookupList].AssignTo(activity, TaxonomyUpdateActivity.lookupListProperty, context);
            parameters[Parameter_itemID].AssignTo(activity, TaxonomyUpdateActivity.itemIDProperty, context);
            parameters[Parameter_FieldValue].AssignTo(activity, TaxonomyUpdateActivity.FieldValueProperty, context);
            parameters[Parameter_TaxonomyFieldName].AssignTo(activity, TaxonomyUpdateActivity.TaxonomyFieldNameProperty, context);

            activity.SetBinding(TaxonomyUpdateActivity.__ContextProperty, new ActivityBind(context.ParentWorkflow.Name, StandardWorkflowDataItems.__context));
            activity.SetBinding(TaxonomyUpdateActivity.__ListItemProperty, new ActivityBind(context.ParentWorkflow.Name, StandardWorkflowDataItems.__item));
            activity.SetBinding(TaxonomyUpdateActivity.__ListIdProperty, new ActivityBind(context.ParentWorkflow.Name, StandardWorkflowDataItems.__list));

            ActivityFlags f = new ActivityFlags();
            f.AddLabelsFromConfig(context);
            f.AssignTo(activity);

            context.ParentActivity.Activities.Add(activity);

            return null;
        }

        public override ActionSummary BuildSummary(ActivityContext context)
        {
            return new ActionSummary("Update a list item with managed metadata.");
        }
    }
}