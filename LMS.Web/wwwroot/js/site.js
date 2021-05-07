// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $('[data-toggle="popover"]').popover();
});



let sel = document.getElementById('select');

sel.addEventListener('change', () => {
    
    if (sel.value === "teacher") {
        sel2.style.display = "none";
    } else {
        sel2.style.display = "block";
    }
});

let sel2 = document.getElementById('select2');
//if (sel === "teacher") {
//    sel2.style.display = "none";
//} else {
//    sel2.style.display = "block";
//}


