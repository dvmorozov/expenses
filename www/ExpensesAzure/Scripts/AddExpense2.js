
function  adjustElementSizes() {
    adjustElementWidth(document.getElementById("ExpenseIds"));
    adjustElementWidth(document.getElementById("CategoryIds"));

    adjustElementWidth(document.getElementById("Cost"));
    adjustElementWidth(document.getElementById("Date"));
}

window.onresize = function () {
    adjustElementSizes();
};

window.onload = function () {
    adjustElementSizes();
    fillDateElements();
    /* Debug output.
    var el = document.getElementById("Cost");
    if (el !== null && typeof el !== "undefined") {
        el.value = "" + $(window).width() + "x" + $(window).height();
    }
    */
};
