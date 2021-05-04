// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $('[data-toggle="popover"]').popover();
});

let colorClass = document.querySelector(".change-color");
function changeColor() {
    colorClass.addEventListener("click", () => {
        colorClass.classList.toggle("text-white");
    })
}
