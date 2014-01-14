<%@ Page Title="" Language="C#" MasterPageFile="~/mainMP.Master" AutoEventWireup="true" CodeBehind="employees.aspx.cs" Inherits="AIS_Time.admin.employees" %>

<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .input-group-addon
        {
            background-color: rgb(50, 118, 177);
            border-color: rgb(40, 94, 142);
            color: rgb(255, 255, 255);
        }

        .form-control:focus
        {
            background-color: rgb(50, 118, 177);
            border-color: rgb(40, 94, 142);
            color: rgb(255, 255, 255);
        }

        .form-signup input[type="text"], .form-signup input[type="password"]
        {
            border: 1px solid rgb(50, 118, 177);
        }
        /*********START POP-UP WINDOW **********/
        .modalBackground
        {
            background-color: Gray;
            filter: "progid:DXImageTransform.Microsoft.Alpha(Opacity=60)";
            filter: alpha(opacity=60);
            opacity: 0.6;
        }

        .modalPopup
        {
            background-color: #EFEFEF;
            padding: 10px 10px 10px 10px;
            border: solid 5px black;
            height: auto;
            margin-top: auto;
            margin-bottom: auto;
            overflow: auto;
            max-height: 70%;
        }

        .modalPopupProcessing
        {
            background-color: #EFEFEF;
            padding: 10px 10px 10px 10px;
            border: solid 5px black;
        }
        /*********END POP-UP WINDOW **********/
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <ajaxToolkit:ToolkitScriptManager ID="ScriptManager1" runat="server">
    </ajaxToolkit:ToolkitScriptManager>
    <script type="text/javascript" language="javascript">
        var ModalProgress = '<%= ModalProgress.ClientID %>';         
    </script>
    <script language="javascript" type="text/javascript" src="../js/jsUpdateProgress.js"></script>
    <script language="javascript" type="text/javascript">
        function ShowModalPopup(ModalBehaviour) {
            $find(ModalBehaviour).show();
        }

        function HideModalPopup(ModalBehaviour) {
            $find(ModalBehaviour).hide();
        }
    </script>
    <script src="admin.js" type="text/javascript"></script>
    <asp:UpdatePanel runat="server" ID="updEntries" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>

            <div class="container">
                <div class="row">
                    <div class="col-md-12">
                        <br />
                        <br />
                        <h2>Employee Manager</h2>
                        <br />
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-6">
                        <div class="panel panel-default">
                            <div class="panel-body">
                                <h4>New Employee</h4>

                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon"><span class="glyphicon glyphicon-user"></span></span>
                                        <asp:TextBox runat="server" ID="txtFname" class="form-control" placeholder="first name" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon"><span class="glyphicon glyphicon-user"></span></span>
                                        <asp:TextBox runat="server" ID="txtLname" class="form-control" placeholder="last name" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <h5>Company</h5>
                                    <div class="input-group">
                                        <span class="input-group-addon"><span class="glyphicon glyphicon-user"></span>
                                        </span>
                                        <asp:DropDownList ID="ddlCompany" class="form-control" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group">

                                    <div class="input-group">
                                        <span class="input-group-addon"><span class="glyphicon glyphicon-user"></span></span>
                                        <asp:TextBox runat="server" ID="txtEmail" class="form-control" placeholder="email" />
                                        <%--  <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="UserName" CssClass="field-validation-error" ErrorMessage="The user name field is required." />--%>
                                    </div>
                                </div>
                                <div class="form-group">

                                    <div class="input-group">
                                        <span class="input-group-addon"><span class="glyphicon glyphicon-user"></span></span>
                                        <asp:TextBox runat="server" ID="txtPhone" class="form-control" placeholder="phone" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon"><span class="glyphicon glyphicon-user"></span></span>
                                        <asp:TextBox runat="server" ID="UserName" class="form-control" placeholder="Username" />
                                        <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="UserName" CssClass="field-validation-error" ErrorMessage="The user name field is required." />--%>
                                    </div>
                                </div>
                                <div class="form-group">

                                    <div class="input-group">
                                        <span class="input-group-addon"><span class="glyphicon glyphicon-lock"></span></span>
                                        <asp:TextBox runat="server" ID="Password" TextMode="Password" class="form-control" placeholder="Password" />
                                        <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="Password" CssClass="field-validation-error" ErrorMessage="The password field is required." />--%>
                                    </div>
                                </div>
                                <div class="form-group">
                                   <%-- <h5>Role</h5>--%>
                                    <div class="input-group">
                                        <span class="input-group-addon"><span class="glyphicon glyphicon-user"></span>
                                        </span>
                                        <asp:DropDownList ID="ddlRoles" class="form-control" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon"><span class="glyphicon glyphicon-user"></span>
                                    </span>
                                    <asp:TextBox runat="server" ID="txtDescription" class="form-control" placeholder="description" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Button ID="cmdSubmit" class="btn btn-sm btn-primary btn-block" runat="server" CommandName="Submit" Text="Submit" OnClick="cmdSubmit_Click" />
                            </div>
                            <div class="form-group">
                                <asp:Label ID="lblError" runat="server" Text=""></asp:Label>
                            </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-6">
                    <div class="panel panel-default">
                        <div class="panel-body">
                            <h4>Existing Employees</h4>
                            <asp:Repeater ID="rptEmployee" runat="server" OnItemCommand="rptCustomers_ItemCommand">
                                <HeaderTemplate>
                                    <table>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblEmployeeName" runat="server" Text='<%# Eval("FirstName") + " " + Eval("LastName") + " - AIS-0" + Eval("TimeEmployeeID") %>'></asp:Label></td>
                                        <asp:Literal ID="editChildColumn" runat="server"></asp:Literal>
                                        <td id="Td1" style="width: 10%" runat="server">
                                            <asp:LinkButton ID="btnEditCustomer" runat="server" CssClass="btn btn-success" type="submit" CommandName="Edit" CommandArgument='<%# Eval("TimeEmployeeID") %>'> EDIT </asp:LinkButton>
                                        </td>
                                        <td id="deleteChildCol" style="width: 10%" runat="server">
                                            <asp:LinkButton ID="btnDeleteCustomer" runat="server" CssClass="btn btn-danger" type="submit" CommandName="Delete" CommandArgument='<%# Eval("TimeEmployeeID") %>' OnClientClick="return confirmdeleteEntry();">DELETE</asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>
            </div>
            </div>
            <!-- START PROGRESS LOADING PANEL -->
            <asp:ModalPopupExtender ID="ModalProgress" runat="server" PopupControlID="PanLoad"
                TargetControlID="PanLoad" BackgroundCssClass="modalBackground">
            </asp:ModalPopupExtender>
            <asp:Panel ID="PanLoad" runat="server" CssClass="modalPopupProcessing">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="0">
                            <ProgressTemplate>
                                <div style="padding: 5px; border: dotted 1px #c3c3c3; text-align: center;">
                                    <div align="center">
                                        <br />
                                        <img src="../Images/head_black.png" width="150px" alt="loading" title="loading" />
                                        <br />
                                        <img src="../Images/ajax/loaders/ajax-loader.gif" alt="loading" title="loading" /><br />
                                        <asp:Label Width="100%" ID="lblProcessing" class="label_field_desc" runat="server"
                                            Text="Loading Data, please wait..."></asp:Label>
                                        <br />
                                    </div>
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:Panel>
            <!-- END PROGRESS LOADING PANEL -->
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
