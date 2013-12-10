<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AIS_Time._Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1><%: Title %>.</h1>
                <h2>Welcome to the Athena Industrial Time Tracker App.</h2>
            </hgroup>
            <p>
               Blah de Blah. Blah de Blah. Blah de Blah. Blah de Blah. Blah de Blah. Blah de Blah. Blah de Blah.
                 Blah de Blah. Blah de Blah. Blah de Blah. Blah de Blah. Blah de Blah. Blah de Blah.
            </p>
        </div>
    </section>
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h3>We suggest the following:</h3>
    <ol class="round">
        <li class="one">
            <h5>Insert New Project record</h5>
            <a href="#">Learn more…</a>
        </li>
        <li class="two">
            <h5>View Your Project Records</h5>
            <a href="#">Learn more…</a>
        </li>
        <li class="three">
            <h5>Modify Records</h5>
            <a href="#">Learn more…</a>
        </li>
    </ol>
</asp:Content>
