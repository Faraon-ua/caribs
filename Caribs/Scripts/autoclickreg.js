//jQuery time
var current_fs, next_fs, previous_fs; //fieldsets
var left, opacity, scale; //fieldset properties which we will animate
var animating; //flag to prevent quick multi-click glitches
var payAmount, uah, usd, rub, accountIds = "", correctAccounts = 0;


$("#phone").mask("99(999)999-99-99");

$("#phone").on("blur", function () {
    var last = $(this).val().substr($(this).val().indexOf("-") + 1);

    if (last.length == 3) {
        var move = $(this).val().substr($(this).val().indexOf("-") - 1, 1);
        var lastfour = move + last;

        var first = $(this).val().substr(0, 9);

        $(this).val(first + '-' + lastfour);
    }
});

printPayAmount();

$('#accountRegForm').submit(function (event) {
    event.preventDefault();
});

$("#next_step1").click(function () {
    $("#next_step1").hide();
    $("#spinner").show();
    accountIds = "";
    var accountsNumber = $('[id^=login]').length;
    correctAccounts = 0;
    var processedAcounts = 0;
    $('[id^=login]').each(function () {
        var login = $(this);
        var password = $(this).next();
        $.post("ValidateCaribsAccount",
            { userName: login.val(), password: password.val(), email: $("#email").val(), phone: $("#phone").val(), inputId: login.attr("id") },
            function (data) {
                if (data.loginResult) {
                    accountIds = accountIds + data.userId + ',';
                    correctAccounts++;
                    $("#" + data.inputId).parent().removeClass().addClass("accBlock-success");
                } else {
                    $("#" + data.inputId).parent().removeClass().addClass("accBlock-fail");

                }
                processedAcounts++;
                if (processedAcounts == accountsNumber) {
                    $("#next_step1").show();
                    $("#spinner").hide();
                    if (accountsNumber == correctAccounts) {
                        if ($('#accountRegForm')[0].checkValidity()) {
                            printPayAmount();
                            $(".next").click();
                        }
                    } else {
                        alert("Вы ввели неправильный логин или пароль от некоторых ваших аккантов");
                    }
                }
            });
    });
});

$("#next_step2").click(function () {
    var yandexPaymentFrameSrc = $("#yandexPaymentFrame").attr("src");
    //replace sum value
    var startIndex = yandexPaymentFrameSrc.indexOf("default-sum=");
    startIndex = startIndex + "default-sum=".length;
    var endindex = yandexPaymentFrameSrc.indexOf("&", startIndex);
    yandexPaymentFrameSrc = yandexPaymentFrameSrc.replaceBetween(startIndex, endindex, rub);

    //replace label value
    if (accountIds) {
        if (accountIds.substr(accountIds.length - 1) == ',') {
            accountIds = accountIds.substr(0, accountIds.length - 1);
        }
        startIndex = yandexPaymentFrameSrc.indexOf("label=");
        startIndex = startIndex + "label=".length;
        endindex = yandexPaymentFrameSrc.length;
        yandexPaymentFrameSrc = yandexPaymentFrameSrc.replaceBetween(startIndex, endindex, accountIds);
    } else {
        alert("Пожалуйста вернитесь к шагу 1 и проверте ваши данные");
        $("#yandexPaymentFrame").attr("src", "");
    }
    $("#yandexPaymentFrame").attr("src", yandexPaymentFrameSrc);
});

function printPayAmount() {
    payAmount = getServicesPayAmount();
    usd = Math.round(payAmount * currencyRates.USD);
    uah = Math.round(payAmount * currencyRates.UAH);
    rub = Math.round(payAmount * currencyRates.RUB);
    $("#payAmount").html(usd + " $<br/>" + uah + "₴<br/>" + rub + "₽");
}

function getServicesPayAmount() {
    var payAmont = 0;
    if ($("#autoclickerSwitch").prop("checked")) {
        payAmont++;
    }
    if ($("#smsNot").prop("checked")) {
        payAmont++;
    }
    var subscriptionMonths = $("#subscriptionFor").val();
    payAmont = payAmont * subscriptionMonths * correctAccounts;
    return payAmont;
}

$("#servicesChoise input[type=checkbox], #subscriptionFor").change(function () {
    printPayAmount();
});

$(".next").click(function () {
    if (animating) return false;
    animating = true;

    current_fs = $(this).parent();
    next_fs = $(this).parent().next();

    //activate next step on progressbar using the index of next_fs
    $("#progressbar li").eq($("fieldset").index(next_fs)).addClass("active");

    //show the next fieldset
    next_fs.show();
    //hide the current fieldset with style
    current_fs.animate({ opacity: 0 }, {
        step: function (now, mx) {
            //as the opacity of current_fs reduces to 0 - stored in "now"
            //1. scale current_fs down to 80%
            scale = 1 - (1 - now) * 0.2;
            //2. bring next_fs from the right(50%)
            left = (now * 50) + "%";
            //3. increase opacity of next_fs to 1 as it moves in
            opacity = 1 - now;
            current_fs.css({ 'transform': 'scale(' + scale + ')' });
            next_fs.css({ 'left': left, 'opacity': opacity });
        },
        duration: 800,
        complete: function () {
            current_fs.hide();
            animating = false;
        },
        //this comes from the custom easing plugin
        easing: 'easeInOutBack'
    });
});

$(".previous").click(function () {
    if (animating) return false;
    animating = true;

    current_fs = $(this).parent();
    previous_fs = $(this).parent().prev();

    //de-activate current step on progressbar
    $("#progressbar li").eq($("fieldset").index(current_fs)).removeClass("active");

    //show the previous fieldset
    previous_fs.show();
    //hide the current fieldset with style
    current_fs.animate({ opacity: 0 }, {
        step: function (now, mx) {
            //as the opacity of current_fs reduces to 0 - stored in "now"
            //1. scale previous_fs from 80% to 100%
            scale = 0.8 + (1 - now) * 0.2;
            //2. take current_fs to the right(50%) - from 0%
            left = ((1 - now) * 50) + "%";
            //3. increase opacity of previous_fs to 1 as it moves in
            opacity = 1 - now;
            current_fs.css({ 'left': left });
            previous_fs.css({ 'transform': 'scale(' + scale + ')', 'opacity': opacity });
        },
        duration: 800,
        complete: function () {
            current_fs.hide();
            animating = false;
        },
        //this comes from the custom easing plugin
        easing: 'easeInOutBack'
    });
});

$(".submit").click(function () {
    return false;
});
