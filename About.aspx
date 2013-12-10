<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="AIS_Time.About" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <hgroup class="title">
        <h1><%: Title %>.</h1>
        <h2>What to put here, dunno.</h2>
    </hgroup>

    <article>
        <p>        
             Blah de Blah.
        </p>
        <p>        
             Blah de Blah.
        </p>
        <p>        
             Blah de Blah.
        </p>
    </article>

    <aside>
        <h3>Something Else</h3>
        <p>        
            Additional information.
        </p>
        <ul>
            <li><a runat="server" href="~/">Home</a></li>
            <li><a runat="server" href="~/About">About</a></li>
            <li><a runat="server" href="~/Contact">Contact</a></li>
        </ul>
    </aside>
</asp:Content>