$(document).ready(function () {

    $('button[type="submit"]').click(function (e) {
        $(this).prev().click();


        e.stopPropagation();
        e.preventDefault();
    });

    // Update tooltip and enable button
    $("input:file").change(function (e) {
        e.stopPropagation();

        var form = $(this).parent();

        form.find("label").attr("data-tooltip", $(this)[0].files[0].name);
        
        var submitButton = form.find("button");
        submitButton.tooltip("remove");
        submitButton.removeClass("disabled grey lighten-4", 1000, "easeInBack");
        submitButton.addClass("waves-effect waves-light-blue blue-text light-blue lighten-5");
    });
});