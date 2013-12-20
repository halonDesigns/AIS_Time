<%@ Page Title="Log in" Language="C#" MasterPageFile="~/defaultMP.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AIS_Time.Account.Login" %>

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

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="ContentPlaceHolder1">
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
    <hgroup class="title">
        <h1><%: Title %>.</h1>
    </hgroup>
    <asp:UpdatePanel runat="server" ID="updEntries" UpdateMode="Conditional" ChildrenAsTriggers="True">
        <ContentTemplate>
            <div class="container">
                <asp:Login ID="Login1" runat="server" ViewStateMode="Disabled" RenderOuterTable="false"
                    OnLoggedIn="LoginUser_LoggedIn" OnLoginError="LoginUser_LoginError" OnLoggingIn="LoginUser_LoggingIn">
                    <LayoutTemplate>
                        <p class="validation-summary-errors">
                            <asp:Literal runat="server" ID="FailureText" />
                        </p>
                        <div class="row">
                            <div class="col-md-4 col-md-offset-2">
                                <div class="panel panel-default">

                                    <div class="panel-body">
                                        <h5 class="text-center">SIGN IN</h5>
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon"><span class="glyphicon glyphicon-user"></span></span>
                                                <asp:TextBox runat="server" ID="UserName" class="form-control" placeholder="Username" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="UserName" CssClass="field-validation-error" ErrorMessage="The user name field is required." />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon"><span class="glyphicon glyphicon-lock"></span></span>
                                                <asp:TextBox runat="server" ID="Password" TextMode="Password" class="form-control" placeholder="Password" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="Password" CssClass="field-validation-error" ErrorMessage="The password field is required." />
                                            </div>
                                        </div>
                                    </div>
                                    <asp:Button class="btn btn-sm btn-primary btn-block" runat="server" CommandName="Login" Text="Log in" />
                                    <div class="row">
                                    </div>
                                </div>
                            </div>
                        </div>
                    </LayoutTemplate>
                </asp:Login>
                <div class="col-md-4 col-md-offset-2">
                    <asp:Label ID="LoginErrorDetails" runat="server" Font-Names="Arial" Font-Size="Small"
                        ForeColor="Red" Visible="true"></asp:Label>
                </div>
            </div>
            <asp:SqlDataSource ID="LOGS_InvalidCredentialsDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>"
                InsertCommand="LOGS_InvalidCredentials_Insert" InsertCommandType="StoredProcedure"
                ProviderName="System.Data.SqlClient">
                <InsertParameters>
                    <asp:Parameter Name="UserName" Type="String" />
                    <asp:Parameter Name="Password" Type="String" />
                    <asp:Parameter Name="IPAddress" Type="String" />
                </InsertParameters>
            </asp:SqlDataSource>
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
