// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $('[data-toggle="popover"]').popover();
});


let sel = document.getElementById('select');

sel.addEventListener('change', () => {
    
    if (sel.value === "B") {
        sel2.style.display = "none";
    } else {
        sel2.style.display = "block";
    }
});

let sel2 = document.getElementById('select2');


//let select1 = document.getElementById('selectItem');

//select1.addEventListener('change', () => {

//    if (select1.value === "B" || select1.value === "C" || select1.value === "D") {
//        select2.style.display = "none";
//    } else {
//        select2.style.display = "block";
//    }
//});

//let select2 = document.getElementById('showList');


var headers = ["H1", "H2", "H3", "H4", "H5", "H6"];

$(".accordion").click(function (e) {
    var target = e.target,
        name = target.nodeName.toUpperCase();

    if ($.inArray(name, headers) > -1) {
        var subItem = $(target).next();

        //slideUp all elements (except target) at current depth or greater
        var depth = $(subItem).parents().length;
        var allAtDepth = $(".accordion p, .accordion div").filter(function () {
            if ($(this).parents().length >= depth && this !== subItem.get(0)) {
                return true;
            }
        });
        $(allAtDepth).slideUp("fast");

        //slideToggle target content and adjust bottom border if necessary
        subItem.slideToggle("fast", function () {
            $(".accordion :visible:last").css("border-radius", "0 0 10px 10px");
        });
        $(target).css({ "border-bottom-right-radius": "0", "border-bottom-left-radius": "0" });
    }
});

//let selectActivity = document.getElementById('act');
//let selectActivity2 = document.getElementById('act2');
//selectActivity.addEventListener('change', () => {

//    if (selectActivity.value === "B" || selectActivity.value === "C" || selectActivity.value === "D") {
//        selectActivity2.style.display = "none";
//    } else {
//        selectActivity2.style.display = "block";
//    }
//});
