/* Helper functions */
// Verify email. Source: http://stackoverflow.com/questions/2507030/email-validation-using-jquery
function isEmail(email) {
    var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test(email);
}

$(document).ready(function () {
    /* Initialization */

    // Modals
    $(".modal-trigger").leanModal();

    // Select tags
    $('select').material_select();

    /* Fixes */

    // Move hidden inputs at the bottom of their container, to avoid CSS styling errors in Materialize
    $("input[type='hidden']").each(function () {
        var copy = $(this).clone();
        var parent = $(this).parent();
        $(this).remove();
        parent.append(copy);
        
    });

    // Stop propagation for all elements with this class
    // This is mainly used to prevent collapsable from opening when clicking on buttons
    $(".stop-propagation").on("click", function (e) {
        e.stopPropagation();
    });

    /* Actions */

    // Search functionality
    // Source: http://stackoverflow.com/questions/12433304/live-search-through-table-rows
    //         http://jsfiddle.net/FranWahl/rFGWZ/
    $("#search").on("keyup", function() {
        var searchVal = $(this).val().toLowerCase();

        $(".collapsible-header").each(function() {
            row = $(this);

            row.removeClass("active");
            row.parent().removeClass("active");
            row.next().css("display", "none");

            var match = false;
            row.find(".search-criteria").each(function () {
                if ($(this).text().toLowerCase().indexOf(searchVal) !== -1) {
                    match = true;
                    return false;
                }
            });

            if (!match) {
                row.hide();
            }
            else {
                row.show();
            }
        });
    });

    // Teacher

});