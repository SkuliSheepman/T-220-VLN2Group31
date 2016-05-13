$(document).ready(function () {
    /* Submission process */

    // Submit button click
    $('button[type="submit"]').click(function (e) {
        e.preventDefault();
        
        if ($(this).hasClass("disabled")) {
            // Click the file input
            $(this).prev().click();
        }
        else {
            if (window.FormData !== undefined) {
                var input = $(this).prev();

                var formData = new FormData();

                var inputId = input.attr("id").split("-");

                formData.append("file", input[0].files[0]);
                formData.append("assignmentId", parseInt(inputId[1]));
                formData.append("problemId", parseInt(inputId[2]));

                // Display spinner
                var status = $(this).parent().parent().next();
                status.html("<div class='preloader-wrapper small active tooltipped' data-position='top' data-delay='50' data-tooltip='Evaluating...'>" +
                                "<div class='spinner-layer spinner-blue'>" +
                                    "<div class='circle-clipper left'>" +
                                        "<div class='circle'></div>" +
                                    "</div>" +
                                    "<div class='gap-patch'>" +
                                        "<div class='circle'></div>" +
                                    "</div>" +
                                    "<div class='circle-clipper right'>" +
                                        "<div class='circle'></div>" +
                                    "</div>" +
                                "</div>" +

                                "<div class='spinner-layer spinner-red'>" +
                                    "<div class='circle-clipper left'>" +
                                        "<div class='circle'></div>" +
                                    "</div>" +
                                    "<div class='gap-patch'>" +
                                        "<div class='circle'></div>" +
                                    "</div>" +
                                    "<div class='circle-clipper right'>" +
                                        "<div class='circle'></div>" +
                                    "</div>" +
                                "</div>" +

                                "<div class='spinner-layer spinner-yellow'>" +
                                    "<div class='circle-clipper left'>" +
                                        "<div class='circle'></div>" +
                                    "</div>" +
                                    "<div class='gap-patch'>" +
                                        "<div class='circle'></div>" +
                                    "</div>" +
                                    "<div class='circle-clipper right'>" +
                                        "<div class='circle'></div>" +
                                    "</div>" +
                                "</div>" +

                                "<div class='spinner-layer spinner-green'>" +
                                    "<div class='circle-clipper left'>" +
                                        "<div class='circle'></div>" +
                                    "</div>" +
                                    "<div class='gap-patch'>" +
                                        "<div class='circle'></div>" +
                                    "</div>" +
                                    "<div class='circle-clipper right'>" +
                                        "<div class='circle'></div>" +
                                    "</div>" +
                                "</div>" +
                            "</div>"
                );
                $(".preloader-wrapper").tooltip();

                console.log(Array.from(formData.entries()));

                // Ajax call
                $.ajax({
                    url: $(this).parent().attr("action"),
                    data: formData,
                    method: "POST",
                    processData: false,
                    contentType: false,
                    success: function (responseData) {
                        status.empty();
                        console.log(responseData);
                        if (responseData) {
                            Materialize.toast("Submission has been evaluated", 4000);
                        }
                        else {
                            Materialize.toast("Oh noes, something went wrong :(", 4000);
                        }
                    },
                    error: function (xhr) {
                        status.empty();
                        Materialize.toast("Error: " + xhr.status + " - " + xhr.statusText, 4000);
                    }
                });
            }
        }
        
        e.stopPropagation();
    });

    // Update tooltip and enable button
    $("input:file").change(function () {
        // Set tooltip to the file's name
        $(this).prev().attr("data-tooltip", $(this)[0].files[0].name);
        
        // Enable the submit button
        var submitButton = $(this).parent().find("button");
        submitButton.tooltip("remove");
        submitButton.removeClass("disabled grey lighten-4", 1000, "easeInBack");
        submitButton.addClass("waves-effect waves-light-blue blue-text light-blue lighten-5");
    });

    // Leave assignment group
    $(".leave-assignment-group").on('click', function (e) {

        var assignmentId = $("#assignmentid").html();

        var formData = {
            "assignmentId":assignmentId
        }
        
        $.ajax({
            url: "/Student/LeaveAssignmentGroup",
            data: JSON.stringify(formData),
            method: "POST",
            contentType: "application/json",
            success: function (responseData) {
                if (responseData) {
                    window.location.reload();
                }
                else {
                    Materialize.toast("An error occurred", 4000);
                }

            },
            error: function () {
                Materialize.toast("Something awful happened :(", 4000);
            }
        });

    });

    // Add user to group
    $(".add-user-to-group-button").on('click', function (e) {

        var formData = {

            "userId": $("#loners").val(),
            "assignmentId": $("#assignmentid").html(),
            "groupId": $("#groupid").html()

        }

        $.ajax({
            url: "/Student/AssignUserToGroup",
            data: JSON.stringify(formData),
            method: "POST",
            contentType: "application/json",
            success: function (responseData) {
                if (responseData) {
                    window.location.reload();
                }
                else {
                    Materialize.toast("An error occurred", 4000);
                }

            },
            error: function () {
                Materialize.toast("Something awful happened :(", 4000);
            }
        });

    });
});