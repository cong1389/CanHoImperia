jQuery(function () {
    window.setLocation = function (url) {
        window.location.href = url;
    };
});

function LoadCategoryHome(virtualId, parentId) {
    jQuery.post('/fixitem-content.html',
        { virtualId: virtualId },
        function (response) {
            if (response.success) {
                jQuery(".leftProductContent_" + parentId + "").empty().html(response.data);
            }
        });
}

function showTool() {
    var heightToShow = jQuery(".top-head").height() +
        jQuery(".header").height() +
        jQuery(".nav-menu").height() +
        jQuery("#homeslide").height();

    jQuery(window).scroll(function () {
        if (jQuery(window).scrollTop() >= heightToShow) {
            jQuery(".nav-tools").stop().animate({
                left: '0',
                top: jQuery(window).height() / 3
            });
        } else {
            jQuery(".nav-tools").stop().animate({
                left: '-80px',
                top: heightToShow
            });
        }
    });
}
function goToByScroll(id) {
    // Remove "link" from the ID
    id = id.replace("link", "");
    // Scroll
    jQuery('html,body').animate({
        scrollTop: jQuery("." + id).offset().top
    },
        'slow');
}

function handleError(msg) {
    jQuery("#msg-error").html(msg).show();
}
function handleSuccess(msg) {
    jQuery("#msg-success").html(msg).show();
}

jQuery(function () {
    jQuery("#btn-send-contact").click(function (e) {
        var fromId = "#form-send-Contact";
        formAjax(fromId);
        return false;
    });

    jQuery("#CheckProduct").click(function (e) {
        var fromId = "#checking";
        formAjax(fromId);
        return false;
    });
    jQuery("#BuyProduct").click(function (e) {
        var fromId = "#formbuy";
        formAjax(fromId);
        return false;
    });
    jQuery("#checkorder").click(function (e) {
        var code = jQuery("#oderCode").val();
        var name = jQuery("#NameOrPhome").val();
        jQuery.post('/home/CheckOrder'
            , { phone: name, ordercode: code }
            , function (response) {
                jQuery(".result_check").empty().html(response.data);
            }
        );
        return false;
    });
});

function formAjax(element) {
    var jQueryform = jQuery(element);
    var options = {
        beforeSend: function () {
            jQuery(".ajax-loading").show();
        },
        dataType: 'json',
        complete: function (responseText, statusText, xhr) {
            var resonse = responseText.responseJSON;
            if (resonse.success) {
                jQueryform[0].reset();
                feature.fancyMsgBox(resonse.title, resonse.message);

            } else {
                feature.fancyMsgBox(resonse.title, resonse.errors);
               
                jQuery('html, body').animate({
                    scrollTop: 0
                },
                    2000);
            }
            jQuery(".ajax-loading").hide();
        }
    };

    if (jQueryform.valid()) {
        jQueryform.ajaxSubmit(options);
    }
    return false;
}
