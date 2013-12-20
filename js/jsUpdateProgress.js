Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginReq);
Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endReq);

function beginReq(sender, args) {
    $find(ModalProgress).show();
}

function endReq(sender, args) {
    $find(ModalProgress).hide();
} 