function popup(id, form) {
    var block = $('#' + id);
    if (form) {
        $('.popup_h1', block).text(form);
    }

    $('.popup_shadow').show();
    $('#' + id).addClass('activePopup');
    var Mtop = -($('.activePopup').outerHeight() / 2) + 'px';
    var Mleft = -($('.activePopup').outerWidth() / 2) + 'px';
    $('.activePopup').css({
        'margin-top': Mtop,
        'margin-left': Mleft,
        'left': '50%',
        'top': '50%'
    });
    $('.activePopup').show();
    $('.formname').attr("value", form);
}

function popup_out() {
    $('.popup_shadow').hide();
    $('.popup').hide();
    $('.popup').removeClass('activePopup');
}