
function ketikan(kalimat,id){

    const kalimatFilter = () => {
        var idNumber = Math.floor(Math.random() * id) + 1;
        var kalimatpersatu = kalimat[idNumber].kalimat
        var kalimatGloba = kalimatpersatu

        return kalimatGloba;
    }
    document.getElementById('tag');
    var type = new Typewriter(tag, {
        loop: true,
        delay:80
    });

    type.typeString(kalimatFilter())
        .pauseFor(1500)
        .deleteAll()
        .typeString(kalimatFilter())
        .pauseFor(1500)
        .deleteAll()
        .start();

}