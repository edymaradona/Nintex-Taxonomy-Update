## Taxonomy Update action
New item site workflow that allows selection of a content type and populating managed metadata fields that are included in the SharePoint document content type template.

Solution supports Nintex site workflow start forms with Create Item action that have no associated list or item until the workflow is running.

Custom Action adapted from a solution by Anupam Ranku found here http://ranku.site/update-managed-metadata-fields-using-nintex-workflow-custom-action/, many thanks.


## Input Form

Nintex 2013 workflow start form (Nintex Form) has one or more Managed Metadata fields.

### Input Field 
Add Managed Metadata input control, ignore the message "The control is not connected"
Advanced settings:
1. Store Client ID in JavaScript variable: Yes
2.  Client ID JavaScript variable name: taxField

### Hidden textbox input control to pass string value
Add textbox control and connect to a string variable e.g. stringField
Advanced settings:
1.  Store Client ID in JavaScript variable: Yes
2. Client ID JavaScript variable name: destField


### Form Settings custom JavaScript:
    function copyMetadata()  {
      var p = NWF$("#" + taxField);
      NWF$('#' + destField).val(p.find("input[type='hidden']").val());  
    } 
 
### Submit button
Advanced settings:
Client Click: copyMetadata()

### Create Item action
You can pass all non-metadata fileds a normal
Store new Item ID to a List Item ID variable

Add a Commit Pending Changes action after the Create Item action and before this Taxonomy Update action.

### Taxonomy Update action
Choose the destination list, List Item ID from the Create Item action, Case sensitive internal field name and taxonomy value variable {Workflow:stringField}

