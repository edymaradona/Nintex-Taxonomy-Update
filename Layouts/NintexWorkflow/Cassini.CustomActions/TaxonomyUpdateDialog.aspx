<%@ Page Language="C#" DynamicMasterPageFile="~masterurl/default.master" AutoEventWireup="true" 
    CodeBehind="TaxonomyUpdateDialog.aspx.cs" 
    Inherits="Cassini.Workflows.TaxonomyUpdateDialog, Cassini.Workflows, Version=1.0.0.0, Culture=neutral, PublicKeyToken=79e8a3b5a648200b" 
    EnableEventValidation="false"%>

<%-- Register directives required by Nintex Workflow 2013 --%>
<%@ Register TagPrefix="Nintex" Namespace="Nintex.Workflow.ServerControls"  Assembly="Nintex.Workflow.ServerControls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=913f6bae0ca5ae12" %>
<%@ Register TagPrefix="Nintex" Namespace="Nintex.Workflow.ApplicationPages" Assembly="Nintex.Workflow.ApplicationPages, Version=1.0.0.0, Culture=neutral, PublicKeyToken=913f6bae0ca5ae12" %>
<%@ Register TagPrefix="Nintex" TagName="ConfigurationProperty" src="~/_layouts/15/NintexWorkflow/ConfigurationProperty.ascx" %>
<%@ Register TagPrefix="Nintex" TagName="ConfigurationPropertySection" src="~/_layouts/15/NintexWorkflow/ConfigurationPropertySection.ascx" %>
<%@ Register TagPrefix="Nintex" TagName="DialogLoad" Src="~/_layouts/15/NintexWorkflow/DialogLoad.ascx" %>
<%@ Register TagPrefix="Nintex" TagName="DialogBody" Src="~/_layouts/15/NintexWorkflow/DialogBody.ascx" %>
<%-- Place additional Register directives after this comment. --%>
<%@ Register 
    TagPrefix="Nintex" 
    TagName="SingleLineInput" 
    Src="~/_layouts/15/NintexWorkflow/SingleLineInput.ascx" %> 
<%@ Register 
    TagPrefix="Nintex" 
    TagName="PlainTextWebControl" 
    Src="~/_layouts/15/NintexWorkflow/PlainTextWebControl.ascx" %> 
<%@ Register 
    TagPrefix="Nintex" 
    TagName="DropDownWithInsertReference" 
    Src="~/_layouts/15/NintexWorkflow/DropDownWithInsertReference.ascx" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <%-- The DialogLoad control must be the first child of this Content control. --%>
    <Nintex:DialogLoad runat="server" />
    <script type="text/javascript" src="/_layouts/15/NintexWorkflow/DropDownWithInsertReference.js"></script>
    <script type="text/javascript" language="javascript">

    function TPARetrieveConfig() {
        setRTEValue('<%= taxonomyFieldName.ClientID %>', configXml.selectSingleNode("/NWActionConfig/Parameters/Parameter[@Name='TaxonomyFieldName']/PrimitiveValue/@Value").text);
        SetDropDown('<%= targetListName.ClientID %>', configXml.selectSingleNode("/NWActionConfig/Parameters/Parameter[@Name='lookupList']/PrimitiveValue/@Value").text);

        var TaxFieldValue = document.getElementById("<%= taxFieldValue.ClientID %>");
        TaxFieldValue.value = configXml.selectSingleNode("/NWActionConfig/Parameters/Parameter[@Name='FieldValue']/PrimitiveValue/@Value").text;

        var TargetListItemID = document.getElementById("<%= targetListItemID.ClientID %>");
        TargetListItemID.value = configXml.selectSingleNode("/NWActionConfig/Parameters/Parameter[@Name='itemID']/PrimitiveValue/@Value").text;

    }

    function TPAWriteConfig() {

        configXml.selectSingleNode("/NWActionConfig/Parameters/Parameter[@Name='TaxonomyFieldName']/PrimitiveValue/@Value").text = getRTEValue('<%= taxonomyFieldName.ClientID %>');
        configXml.selectSingleNode("/NWActionConfig/Parameters/Parameter[@Name='lookupList']/PrimitiveValue/@Value").text = GetDropDown("<%= targetListName.ClientID %>");

        var TaxFieldValue = document.getElementById("<%= taxFieldValue.ClientID %>");
        configXml.selectSingleNode("/NWActionConfig/Parameters/Parameter[@Name='FieldValue']/PrimitiveValue/@Value").text = TaxFieldValue.value;

        var TargetListItemID = document.getElementById("<%= targetListItemID.ClientID %>");
        configXml.selectSingleNode("/NWActionConfig/Parameters/Parameter[@Name='itemID']/PrimitiveValue/@Value").text = TargetListItemID.value;
        return true;
    }

    function SetDropDown(clientId, value) {
            var dropdownControl = document.getElementById(clientId);
            for (var i = 0; i < dropdownControl.options.length; i++) {
                if (dropdownControl.options[i].value == value) {
                    dropdownControl.selectedIndex = i;
                    return;
                }
            }
    }
    function GetDropDown(clientId) {
        var dropdownControl = document.getElementById(clientId);

        if (IsOtherOptionSelected(dropdownControl)) {
            return getRTEValue(clientId);
        }
        else {
            if (dropdownControl.selectedIndex == -1) {
                return "";
            }
            else {
                return dropdownControl.options[dropdownControl.selectedIndex].value;
            }
        }
    }
    onLoadFunctions[onLoadFunctions.length] = function () {
        dialogSectionsArray["<%= MainControls1.ClientID %>"] = true;
    };

    </script>
    
</asp:Content>

<asp:Content ID="ContentBody" ContentPlaceHolderID="PlaceHolderMain" runat="Server">
  <Nintex:ConfigurationPropertySection runat="server" Id="MainControls1">
    <TemplateRowsArea>

      <Nintex:ConfigurationProperty runat="server" FieldTitle="Target List Name" RequiredField="True">
        <TemplateControlArea>
            <Nintex:ListSelector runat="server" id="targetListName" LibrariesOnly="false"/>
        </TemplateControlArea>
      </Nintex:ConfigurationProperty>

      <Nintex:ConfigurationProperty runat="server" FieldTitle="Target List Item ID" RequiredField="True">
        <TemplateControlArea>
            <Nintex:VariableSelector RenderForPrimitiveValue="true" runat="server" id="targetListItemID" IncludeSPItemKeyVars="True"/>
        </TemplateControlArea>
      </Nintex:ConfigurationProperty>

      <Nintex:ConfigurationProperty runat="server" FieldTitle="Target List Internal Field Name (case sensitive)" RequiredField="True">
        <TemplateControlArea>
           <Nintex:SingleLineInput runat="server" id="taxonomyFieldName"></Nintex:SingleLineInput>
        </TemplateControlArea>
      </Nintex:ConfigurationProperty>
        
      <Nintex:ConfigurationProperty runat="server" FieldTitle="Taxonomy Field Value" RequiredField="True">
        <TemplateControlArea>
            <Nintex:VariableSelector runat="server" id="taxFieldValue" RenderForPrimitiveValue="true"  IncludeTextVars="true"/>
        </TemplateControlArea>
      </Nintex:ConfigurationProperty>
     
    </TemplateRowsArea>
  </Nintex:ConfigurationPropertySection>
    <div>
        <div><span>Help:</span></div>
        <div><span>Library Name  e.g. 'Draft'</span></div>
        <div><span>ID of item in library to update</span></div>
        <div><span>Taxonomy Field Name: name of the taxonomy field. e.g. 'Program'</span></div>
        <div>
        <span>FieldValue: value of the taxonomy field. Format: TermName|GUID' e.g. 'Software|81216f24-e2a1-4866-a813-21a46cea2c75'</span>
        </div>
    </div>

  <Nintex:DialogBody runat="server" id="DialogBody">
  </Nintex:DialogBody>

</asp:Content>
