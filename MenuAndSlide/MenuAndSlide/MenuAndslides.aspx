<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MenuAndslides.aspx.cs" Inherits="MenuAndSlide.MenuAndSlides" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.12.0/themes/base/jquery-ui.css">
    <link rel="stylesheet" href="/css/style.css">
    <script src="https://code.jquery.com/jquery-1.12.4.js"></script>
    <script src="https://code.jquery.com/ui/1.12.0/jquery-ui.js"></script>
    <script>
    $( function() {
        $( "#menu" ).menu();
    } );
    </script>
    
</head>
<body>
    <div class="menu-display">
        <ul id="menu" runat="server">
        </ul>
    </div>
    <div id="detailsView" class="details-display" runat="server">
        
    </div>
</body>
</html>
