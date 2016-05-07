$(document).ready(function () {
    /* Initialization */

    // Modals
    $(".modal-trigger").leanModal();

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


    /* ADMIN */

    // Admin - Create new user form
    $("#create-user-form").on("submit", function() {
        console.log("Creating new user...");
        var form = $(this);

        var formData = {
            "Name": $("#NewUserModel_Name").val(),
            "Email": $("#NewUserModel_Email").val(),
            "Admin": $("#NewUserModel_Admin").val(),
            "UserCourses": [
                {
                    
                }
            ]
        }
        
        $.ajax({
            url: form.attr("action"),
            data: formData,
            method: "POST",
            success: function (responseData) {
                console.log(responseData);
                if (responseData) {
                    Materialize.toast("The user " + formData.Email + " has been created", 4000);
                }
                else {
                    Materialize.toast("The user " + formData.Email + " already exists", 4000);
                }
            }
        });
        return false;
    });

    $("#create-user-button").on("click", function () {
        console.log("Clicked");
        $("#create-user-form").submit();
    });

    // Admin - Create new course form
    $("#create-course-form").on("submit", function () {
        console.log("Creating new course...");
        var form = $(this);

        var formData = {
            "Name": $("#NewCourseModel_Name").val(),
            "Description": $("#NewCourseModel_Description").val(),
            "Semester": $("input[type='radio'][name='NewCourseModel.Semester']:checked").val(),
            "Year": $("#NewCourseModel_Year").val()
        }

        $.ajax({
            url: form.attr("action"),
            data: formData,
            method: "POST",
            success: function (responseData) {
                Materialize.toast("The course " + formData.Name + " has been created", 4000);
            },
            error: function() {
                // TODO
            }
        });
        return false;
    });

    $("#create-course-button").on("click", function() {
        console.log("Clicked");
        $("#create-course-form").submit();
    });

    // Admin - Delete selected users
    $("#delete-selected-users").on("click", function () {
        var users = $("input[type='checkbox'][name='user-row']:checked").map(function() {
            return this.value;
        }).get();

        if (0 < users.length) {
            $.ajax({
                url: "/Admin/DeleteSelectedUsers",
                data: JSON.stringify(users),
                method: "POST",
                contentType: "application/json",
                success: function(responseData) {
                    if (responseData) {
                        Materialize.toast("Users deleted", 4000);
                        $("input[type='checkbox'][name='user-row']:checked").each(function() {
                            $(this).closest("li").remove();
                        });
                    }
                    else {
                        Materialize.toast("An error occurred", 4000);
                    }

                },
                error: function() {
                    // TODO
                }
            });
        }
        else {
            Materialize.toast("No users selected", 2000);
        }
    });
});