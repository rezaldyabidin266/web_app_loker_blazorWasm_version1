window.myBrowser = () => {

    if (navigator.userAgent.indexOf("Edge") > -1 && navigator.appVersion.indexOf('Edge') > -1) {
        return 'Edge';
    }
    else if (navigator.userAgent.indexOf("Opera") != -1 || navigator.userAgent.indexOf('OPR') != -1) {
        return 'Opera';
    }
    else if (navigator.userAgent.indexOf("Chrome") != -1) {
        return 'Chrome';
    }
    else if (navigator.userAgent.indexOf("Safari") != -1) {
        return 'Safari';
    }
    else if (navigator.userAgent.indexOf("Firefox") != -1) {
        return 'Firefox';
    }
    else if ((navigator.userAgent.indexOf("MSIE") != -1) || (!!document.DOCUMENT_NODE == true)) //IF IE > 10
    {
        return 'IE';
    }
    else {
        return 'unknown';
    }
}

function deviceScreen(){

    let witdhScreen = screen.width;
    let heightScreen = screen.height;
    let colorDefth = screen.colorDepth;

    return `${witdhScreen} x ${heightScreen} Pixels / ColorDepth ${colorDefth} bit `;

}

window.doNotTrack = () => {
    if (window.doNotTrack || navigator.doNotTrack || window.doNotTrack) {
        // The browser supports Do Not Track!
        if (window.doNotTrack == "1" || navigator.doNotTrack == "yes" || navigator.doNotTrack == "1" || window.doNotTrack == "1") {
            // Do Not Track is enabled!
            return true
        } else {
            // Do Not Track is disabled!
            return false
        }
    } else {
        // Do Not Track is not supported
        return false

    }
}

function userReferrer() {
    return document.referrer;
}

window.getUserAgent = () => {
    let resource = navigator.userAgent;
    let startIndex = resource.indexOf('(');
    let stopIndex = resource.indexOf(')');
    let Hasil = resource.substring(startIndex, stopIndex)

    return Hasil;
}