﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
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

let table = document.getElementById("test22");
let table2 = table.cloneNode(table);
let uploadBtn = document.getElementById("uploadBtn");


uploadBtn.addEventListener("click", () => {
    console.log("click");
    let courseId = uploadBtn.getAttribute("data-target");
    let updatedView = document.querySelectorAll("TR");
    updatedView.forEach(row => {
        if (row.classList.contains("collapse")) {
            row.classList.remove("collapse");
        }
    })

});

var target = document.querySelector('#drop2');

// create an observer instance
var observer = new MutationObserver(function (mutations) {
    mutations.forEach(function (mutation) {
        console.log(mutation.type);
    });
});

// configuration of the observer:
var config = { attributes: true, childList: true, characterData: true }

// pass in the target node, as well as the observer options
observer.observe(target, config);

// later, you can stop observing
observer.disconnect();