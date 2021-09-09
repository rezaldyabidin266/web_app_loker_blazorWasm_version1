function navbarScroll() {
    var prevScrollpos = window.pageYOffset;
    window.onscroll = function () {
        var currentScrollPos = window.pageYOffset;
        if (prevScrollpos > currentScrollPos) {
            document.getElementById("navbar").style.top = "0";
        } else {
            document.getElementById("navbar").style.top = "-80px";
        }
        prevScrollpos = currentScrollPos;
    }
}

//open Cv in URL OBJECT
window.openCv = (byte) => {
    var byteCharacters = atob(byte);
    var byteNumbers = new Array(byteCharacters.length);
    for (var i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    var byteArray = new Uint8Array(byteNumbers);
    var file = new Blob([byteArray], { type: 'application/pdf;base64' });
    var fileURL = URL.createObjectURL(file);
    window.open(fileURL);
}

//Password Show/Hide
function showHidePassword(cssClass, showPassword) {
    var InputItem = document.querySelector("." + cssClass + " input"); 
    if (InputItem) {
        InputItem.type = showPassword ? "text" : "password";
    }
}