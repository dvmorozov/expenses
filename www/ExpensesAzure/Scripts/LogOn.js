
function  adjustElementSizes() {
    adjustElementWidth(document.getElementById("UserName"));
    adjustElementWidth(document.getElementById("Password"));
}

window.onresize = function () {
    adjustElementSizes();
};

window.onload = function () {
    adjustElementSizes();
};
