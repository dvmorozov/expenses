
window.onresize = function () {
    //adjustElementSizes(document.getElementById("ExpenseIds"));
    //adjustElementSizes(document.getElementById("CategoryIds"));

    adjustElementWidth(document.getElementById("ExpenseIds"));
    adjustElementWidth(document.getElementById("CategoryIds"));
    
    adjustElementWidth(document.getElementById("Cost"));
    adjustElementWidth(document.getElementById("Date"));
};

window.onload = function () {
// ReSharper disable UseOfImplicitGlobalInFunctionScope
    deleteMultilpleAttr(document.getElementById("ExpenseIds"));
// ReSharper restore UseOfImplicitGlobalInFunctionScope
// ReSharper disable UseOfImplicitGlobalInFunctionScope
    deleteMultilpleAttr(document.getElementById("CategoryIds"));
// ReSharper restore UseOfImplicitGlobalInFunctionScope
};
