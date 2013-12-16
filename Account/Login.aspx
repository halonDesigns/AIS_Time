<%@ Page Title="Log in" Language="C#" MasterPageFile="~/defaultMP.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AIS_Time.Account.Login" %>
<%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="ContentPlaceHolder1">
    <hgroup class="title">
        <h1><%: Title %>.</h1>
    </hgroup>
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
    <%--<section id="loginForm">
        <asp:Login runat="server" ViewStateMode="Disabled" RenderOuterTable="false">
            <LayoutTemplate>
                <p class="validation-summary-errors">
                    <asp:Literal runat="server" ID="FailureText" />
                </p>
                <fieldset>
                    <legend>Log in Form</legend>
                    <ol>
                        <li>
                            <asp:Label runat="server" AssociatedControlID="UserName">User name</asp:Label>
                            <asp:TextBox runat="server" ID="UserName" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="UserName" CssClass="field-validation-error" ErrorMessage="The user name field is required." />
                        </li>
                        <li>
                            <asp:Label runat="server" AssociatedControlID="Password">Password</asp:Label>
                            <asp:TextBox runat="server" ID="Password" TextMode="Password" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Password" CssClass="field-validation-error" ErrorMessage="The password field is required." />
                        </li>
                        <li>
                            <asp:CheckBox runat="server" ID="RememberMe" />
                            <asp:Label runat="server" AssociatedControlID="RememberMe" CssClass="checkbox">Remember me?</asp:Label>
                        </li>
                    </ol>
                    <asp:Button runat="server" CommandName="Login" Text="Log in" />
                </fieldset>
            </LayoutTemplate>
        </asp:Login>--%>
       <%-- <p>
            <asp:HyperLink runat="server" ID="RegisterHyperLink" ViewStateMode="Disabled">Register</asp:HyperLink>
            if you don't have an account.
        </p>--%>
   <%-- </section>--%>
     <asp:SqlDataSource ID="LOGS_InvalidCredentialsDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>"
                InsertCommand="LOGS_InvalidCredentials_Insert" InsertCommandType="StoredProcedure"
                ProviderName="System.Data.SqlClient">
                <InsertParameters>
                    <asp:Parameter Name="UserName" Type="String" />
                    <asp:Parameter Name="Password" Type="String" />
                    <asp:Parameter Name="IPAddress" Type="String" />
                </InsertParameters>
            </asp:SqlDataSource>
    <section id="socialLoginForm">
      <%--  <h2>Use another service to log in.</h2>
        <uc:OpenAuthProviders runat="server" ID="OpenAuthLogin" />--%>
    </section>
</asp:Content>
