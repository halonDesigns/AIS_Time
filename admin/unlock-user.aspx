<%@ Page Title="" Language="C#" MasterPageFile="~/mainMP.Master" AutoEventWireup="true" CodeBehind="unlock-user.aspx.cs" Inherits="AIS_Time.admin.unlock_user" %>
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
      <script type="text/javascript" src="<%= Page.ResolveUrl("~/admin/admin.js") %>"></script>
    <asp:UpdatePanel runat="server" ID="updEntries" UpdateMode="Conditional" ChildrenAsTriggers="True">
        <ContentTemplate>

            <div class="container">
                <div class="row">
                    <div class="col-md-12">
                       <br />
                        <br />
                         <h2> Manage Users</h2>
                        <br />
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-10">
                        <div class="panel panel-default">
                            <div class="panel-body">
                                <h4>This screen allows you to view the locked out / active
                            / role status of users and modify the settings as needed.</h4>

                                <br />
                    <p>
                        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4"
                            DataSourceID="UsersInfoDataSource" GridLines="None" 
                            DataKeyNames="UserName" Font-Overline="True" ForeColor="#333333" 
                            onrowcommand="GridView1_RowCommand" AllowPaging="True">
                            <EditRowStyle BackColor="#999999" />
                            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                            <Columns>
                                <asp:BoundField DataField="UserName" HeaderText="User" ReadOnly="True" SortExpression="UserName" />
                                <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
                                <asp:TemplateField HeaderText="Approved?">
                                    <ItemTemplate>
                                        <asp:CheckBox runat="server" OnCheckedChanged="ToggleApproved" AutoPostBack="True"
                                            ID="IsApproved" Checked='<%# Eval("IsApproved") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Unlock?">
                                    <ItemTemplate>
                                        <asp:CheckBox runat="server" Enabled="False" ID="IsLockedOut" Checked='<%# Eval("IsLockedOut") %>' />
                                        <%-- This LinkButton is only shown if the user is locked out --%>
                                        <asp:LinkButton ID="LinkButton1" runat="server" CommandName="UnlockUser" Visible='<%# Eval("IsLockedOut") %>'>Unlock</asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Administrator?">
                                    <ItemTemplate>
                                        <asp:CheckBox runat="server" OnCheckedChanged="ToggleAdministrator" AutoPostBack="True"
                                            ID="IsAdministrator" Checked='<%# Roles.IsUserInRole(Eval("UserName").ToString(), "Administrator") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Consultant?">
                                    <ItemTemplate>
                                        <asp:CheckBox runat="server" OnCheckedChanged="ToggleConsultant" AutoPostBack="True"
                                            ID="IsConsultant" Checked='<%# Roles.IsUserInRole(Eval("UserName").ToString(), "Consultant") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                            <SortedAscendingCellStyle BackColor="#E9E7E2" />
                            <SortedAscendingHeaderStyle BackColor="#506C8C" />
                            <SortedDescendingCellStyle BackColor="#FFFDF8" />
                            <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                        </asp:GridView>
                        <asp:ObjectDataSource ID="UsersInfoDataSource" runat="server" SelectMethod="GetAllUsers"
                            TypeName="System.Web.Security.Membership"></asp:ObjectDataSource>
                    </p>
                    <p>
                        &nbsp;</p>
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
                                        <img src="<%= Page.ResolveUrl("~/Images/head_black.png") %>" width="150px" alt="loading" title="loading" />
                                        <br />
                                        <img src="<%= Page.ResolveUrl("~/Images/ajax/loaders/ajax-loader.gif") %>" alt="loading" title="loading" /><br />
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
            <asp:Button ID="btnHidden" runat="server" Text="" Style="display: none;" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
