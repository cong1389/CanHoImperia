//if ((typeof Haravan) === 'undefined') {
//    Haravan = {};
//}
//Haravan.culture = 'vi-VN';
//Haravan.shop = 'kute-shop.myharavan.com';
//Haravan.theme = {
//    "name": "Bản sao chép của Update - Fashion store",
//    "id": 1000138323,
//    "role": "main"
//};
//Haravan.domain = 'kute-shop.myharavan.com';
////]]>

jQuery(document).ready(function ($) {
    //var date = new Date();
    //var minutes = 60;
    //date.setTime(date.getTime() + (minutes * 60 * 1000));
    //if (jQuery.cookie('popupNewLetterStatus') != 'closed') {
    //    jQuery.fancybox.open({
    //        padding: 0,
    //        'beforeLoad': function () {
    //            $("#popup-newletter").removeClass('hide');
    //            var Form = $("mc-embedded-subscribe-form").clone();
    //            //$('.pnewle-form',$("#popup-newletter")).html($("#mc-embedded-subscribe-form").clone().removeAttr("id"));
    //        },
    //        href: "#popup-newletter",
    //        afterClose: function () {
    //            $("#popup-newletter").addClass('hide');
    //            jQuery.cookie('popupNewLetterStatus', 'closed', {
    //                expires: date,
    //                path: '/'
    //            });
    //        }
    //    });
    //}
});

var callBack = function (variant, selector) {
    if (variant) {
        modal = $("#quick-view-modal");
        $(".p-price").html(Haravan.formatMoney(variant.price, "{{ amount }}₫"));
        if (variant.compare_at_price > 0)
            modal.find("del").html(Haravan.formatMoney(variant.compare_at_price, "{{ amount }}₫"));
        else
            modal.find("del").html("");
        if (variant.available) {
            modal.find(".btn-addcart").css("display", "block");
            modal.find(".btn-soldout").css("display", "none");
        } else {
            modal.find(".btn-addcart").css("display", "none");
            modal.find(".btn-soldout").css("display", "block");
        }
        if (variant.sku != null) {
            modal.find(".m-sku").html("<span>Mã sản phẩm: </span>" + variant.sku);
        }
    } else {
        modal.find(".btn-addcart").css("display", "none");
        modal.find(".btn-soldout").css("display", "block");
    }
}
var p_select_data = $(".p-option-wrapper").html();
var p_zoom = $(".image-zoom").html();
var quickViewProduct = function (purl) {

    if ($(window).width() < 680) {
        window.location = purl;
        return false;
    }
    modal = $("#quick-view-modal");
    modal.modal("show");
    $.ajax({
        url: purl + ".js",
        async: false,
        success: function (product) {
            $.each(product.options, function (i, v) {
                product.options[i] = v.name;
            })
            modal.find(".p-title").html("<h1>" + product.title + "</h1>");
            modal.find(".p-option-wrapper").html(p_select_data);
            modal.find(".m-vendor").html("<span>Nhà cung cấp: </span>" + product.vendor);
            $(".image-zoom").html(p_zoom);
            modal.find(".p-url").attr("href", product.url);
            if (product.images.length == 0) {
                modal.find(".p-product-image-feature").attr("src", "/assets/0/0/global/noDefaultImage6_large.gif");
            } else {
                $("#p-sliderproduct").remove();
                $(".image-zoom").append("<div id='p-sliderproduct' class='flexslider'>");
                $("#p-sliderproduct").append("<ul class='slides'>");

                $.each(product.images, function (i, v) {
                    elem = $('<li class="product-thumb">').append('<a href="#" data-image="" data-zoom-image=""><img /></a>');
                    elem.find("a").attr("data-image", Haravan.resizeImage(v, "medium"));
                    elem.find("a").attr("data-zoom-image", v);
                    elem.find("img").attr("data-image", Haravan.resizeImage(v, "medium"));
                    elem.find("img").attr("data-zoom-image", v);
                    elem.find("img").attr("src", Haravan.resizeImage(v, "small"));
                    modal.find(".slides").append(elem);
                })

                modal.find(".p-product-image-feature").attr("src", product.featured_image);
                $(".modal-footer .btn-readmore").attr("href", purl);
                var iflag = 0;
                $("#p-sliderproduct img").load(function () {
                    iflag++;
                    if (iflag == $("#p-sliderproduct img").length) {
                        $("#p-sliderproduct").flexslider({
                            animation: "slide",
                            controlNav: false,
                            animationLoop: false,
                            slideshow: false,
                            itemWidth: 80
                        });
                    }
                })
                modal.find(".owl-item:first-child img").click();
            }
            $.each(product.variants, function (i, v) {
                modal.find("select#p-select").append("<option value='" + v.id + "'>" + v.title + " - " + v.price + "</option>");
            })
            if (product.variants.length == 1 && product.variants[0].title.indexOf("Default") != -1)
                $(".p-option-wrapper").hide();
            else
                $(".p-option-wrapper").show();
            if (product.variants.length == 1 && product.variants[0].title.indexOf("Default") != -1) {
                callBack(product.variants[0], null);
            } else {
                new Haravan.OptionSelectors("p-select", {
                    product: product,
                    onVariantSelected: callBack
                });
                debugger
                if (product.options.length == 1 && product.options[0].indexOf("Tiêu đề") == -1)

                    modal.find(".selector-wrapper:eq(0)").prepend("<label>" + product.options[0] + "</label>");

                $(".p-option-wrapper select:not(#p-select)").each(function () {
                    $(this).wrap('<span class="custom-dropdown custom-dropdown--white"></span>');
                    $(this).addClass("custom-dropdown__select custom-dropdown__select--white");
                });
                callBack(product.variants[0], null);
            }

        }
    });

    //$('.modal-backdrop').css('opacity', '0');
    return false;
}

$("#quick-view-modal").on("click", ".product-thumb img", function (event) {
    event.preventDefault();

    modal = $("#quick-view-modal");
    modal.find(".p-product-image-feature").attr("src", $(this).attr("data-zoom-image"));
    modal.find(".product-thumb").removeClass("active");
    $(this).parents("li").addClass("active");
    return false;
})

$(document).ready(function () {

    if ($("#contact_modal").length > 0) {
        $("#contact_modal").click(function () {

            feature.fancyMsgBox("aaaa", "aaaa");
        });
    }

    $("a.btn-quickview-1").click(function (event) {
        //console.log('abc')
        event.preventDefault();
        quickViewProduct($(this).attr("data-handle"));
    });
});


var feature = (function () {

    var fancyMsgBox = function (title, msg) {
        if (title) {
            msg = "<h2 class='fancybox-skin__title'>" + title + "</h2><p>" + msg + "</p>";
        }
        msg +=
            '<div class="fancybox-skin__content">' +
            '<input class="button" type="button" value="OK" onclick="$.fancybox.close();" />' +
            "</div>";
        debugger;
        if (!!$.prototype.fancybox) {
           
            $.fancybox(msg,
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