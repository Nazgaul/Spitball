function changeLanguage($langElem){
    $langElem.click(function () {
        document.cookie = "l3 =" + $(this).data('cookie') + "; path=/;domain=spitball.co";
        location.reload();
    });
}