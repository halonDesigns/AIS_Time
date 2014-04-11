<%@ Page Title="" Language="C#" MasterPageFile="~/userMP.Master" AutoEventWireup="true" CodeBehind="reports.aspx.cs" Inherits="AIS_Time.user.reports" %>
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
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <ajaxToolkit:ToolkitScriptManager ID="ScriptManager1" runat="server">
    </ajaxToolkit:ToolkitScriptManager>
    <div class="container">
        <div class="row">
            <div class="col-md-12">
               <br />
                <h2>Time Card Manager</h2>
                <br />
            </div>
        </div>
          <div class="row">
            <div class="col-md-6">
                <div class="panel panel-default">
                    <div class="panel-body">
                        <h4>Weekly Employee Time Card</h4>
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon"><span class="glyphicon glyphicon-user"></span></span>
                                <asp:TextBox runat="server" ID="txtEmployeeWeekly" class="form-control" placeholder="Week of work" />
                                <asp:CalendarExtender ID="CalendarExtender4" runat="server" Enabled="True" TargetControlID="txtEmployeeWeekly" DefaultView="Months">
                                </asp:CalendarExtender>
                            </div>
                        </div>

                    </div>
                    <div class="form-group">
                        <div class="btn-group">
                            <asp:Button ID="btnWeeklyEmployeeReport" class="btn btn-primary btn-lg" runat="server" CommandName="Submit" Text="Weekly Report" OnClick="btnWeeklyEmployeeReport_Click" />
                        </div>

                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblErrorEmployeeWeekly" runat="server" Text=""></asp:Label>
                    </div>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-md-6">
                <div class="panel panel-default">
                    <div class="panel-body">
                        <h4>Monthly Employee Time Card</h4>
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon"><span class="glyphicon glyphicon-user"></span></span>
                                <asp:TextBox runat="server" ID="txtMonth" class="form-control" placeholder="Month of work" />
                                <asp:CalendarExtender ID="CalendarExtender2" runat="server" Enabled="True" TargetControlID="txtMonth" DefaultView="Months">
                                </asp:CalendarExtender>
                            </div>
                        </div>

                    </div>
                    <div class="form-group">
                        <div class="btn-group">
                            <asp:Button ID="btnMonthlyEmployeeReport" class="btn btn-primary btn-lg" runat="server" CommandName="Submit" Text="Monthly Report" OnClick="btnMonthlyEmployeeReport_Click" />
                        </div>

                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblErrorMonthly" runat="server" Text=""></asp:Label>
                    </div>
                </div>
            </div>
        </div>
           <div class="row">
            <div class="col-md-6">
                <div class="panel panel-default">
                    <div class="panel-body">
                        <h4>Monthly SRED Summary</h4>
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon"><span class="glyphicon glyphicon-user"></span>
                                </span>
                                <asp:DropDownList ID="ddlMonthlyProjectsSRED" class="form-control" runat="server"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon"><span class="glyphicon glyphicon-user"></span></span>
                                <asp:TextBox runat="server" ID="txtMonthSREDStart" class="form-control" placeholder="month start date" />
                                <asp:CalendarExtender ID="CalendarExtender5" runat="server" Enabled="True" TargetControlID="txtMonthSREDStart" DefaultView="Months">
                                </asp:CalendarExtender>
                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="btn-group">
                            <asp:Button ID="btnMonthlyEmployeeSREDReport" class="btn btn-primary btn-lg" runat="server" CommandName="Submit" Text="Monthly Project Report" OnClick="btnMonthlyEmployeeSREDReport_Click" />
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblErrorMonthlySRED" runat="server" Text=""></asp:Label>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
