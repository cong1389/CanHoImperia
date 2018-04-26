
var feature = (function () {

    var fancyMsgBox = function (title, msg) {
        if (title) {
            msg = "<h2 class='fancybox-skin__title'>" + title + "</h2><p>" + msg + "</p>";
        }
        msg +=
            '<div class="fancybox-skin__content">' +
            '<input class="button" type="button" value="OK" onclick="jQuery.fancybox.close();" />' +
            "</div>";
        
        if (!!jQuery.prototype.fancybox) {

            jQuery.fancybox(msg,
                {
                    'autoDimensions': false,
                    'autoSize': true,
                    'width': 500,
                    'height': "auto",
                    'openEffect': "none",
                    'closeEffect': "none"
                });
        }
        return false;
    }

    return {
        init: function () {

        },
        fancyMsgBox: function (title, msg) {
            fancyMsgBox(title, msg);
        }
    }

})();


function blockUI_front(options) {
    options = jQuery.extend(true, {}, options);
    var html = '';
    if (options.animate) {
        html = '<div class="loading-message ' + (options.boxed ? 'loading-message-boxed' : '') + '">' + '<div class="block-spinner-bar"><div class="bounce1"></div><div class="bounce2"></div><div class="bounce3"></div></div>' + '</div>';
    } else if (options.iconOnly) {
        html = '<div class="loading-message ' + (options.boxed ? 'loading-message-boxed' : '') + '"><img src="' + this.getGlobalImgPath() + 'loading-spinner-grey.gif" align=""></div>';
    } else if (options.textOnly) {
        html = '<div class="loading-message ' + (options.boxed ? 'loading-message-boxed' : '') + '"><span>&nbsp;&nbsp;' + (options.message ? options.message : 'LOADING...') + '</span></div>';
    } else {
        html = '<div class="loading-message ' + (options.boxed ? 'loading-message-boxed' : '') + '"><img src="' + this.getGlobalImgPath() + 'loading-spinner-grey.gif" align=""><span>&nbsp;&nbsp;' + (options.message ? options.message : 'LOADING...') + '</span></div>';
    }

    if (options.target) { // element blocking
        var el = jQuery(options.target);
        if (el.height() <= (jQuery(window).height())) {
            options.cenrerY = true;
        }
        el.block({
            message: html,
            baseZ: options.zIndex ? options.zIndex : 1000,
            centerY: options.cenrerY !== undefined ? options.cenrerY : false,
            css: {
                top: '10%',
                border: '0',
                padding: '0',
                backgroundColor: 'none'
            },
            overlayCSS: {
                backgroundColor: options.overlayColor ? options.overlayColor : '#555',
                opacity: options.boxed ? 0.05 : 0.1,
                cursor: 'wait'
            }
        });
    } else { // page blocking
        jQuery.blockUI({
            message: html,
            baseZ: options.zIndex ? options.zIndex : 1000,
            css: {
                border: '0',
                padding: '0',
                backgroundColor: 'none'
            },
            overlayCSS: {
                backgroundColor: options.overlayColor ? options.overlayColor : '#555',
                opacity: options.boxed ? 0.05 : 0.1,
                cursor: 'wait'
            }
        });
    }
}