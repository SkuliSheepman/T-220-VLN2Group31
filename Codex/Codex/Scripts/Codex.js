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

    /* Helper functions */
    // Verify email. Source: http://stackoverflow.com/questions/2507030/email-validation-using-jquery
    function isEmail(email) {
        var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        return regex.test(email);
    }

    // Get user ID from row in admin view with supplied descendant
    function getUserId(elem) {
        return elem.closest("li").find("input[type='checkbox'][name='user-row']").val();
    }

    /* ADMIN */

    // Admin - Create new user form
    $("#create-user-form").on("submit", function() {
        var form = $(this);
        
        var name = $("#NewUserModel_Name").val();
        var email = $("#NewUserModel_Email").val();

        if (0 < name.length && isEmail(email)) {
            var formData = {
                "Name": name,
                "Email": email,
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
                success: function(responseData) {
                    if (responseData) {
                        Materialize.toast("The user " + formData.Email + " has been created", 4000);
                        $("#NewUserModel_Name").val("").blur();
                        $("#NewUserModel_Email").val("").blur();
                        $("#NewUserModel_Admin").attr("checked", false);
                    }
                    else {
                        Materialize.toast("The user " + formData.Email + " already exists", 4000);
                    }
                }
            });
        }
        else {
            Materialize.toast("Missing/Invalid information", 4000);
        }
        
        return false;
    });

    $("#create-user-button").on("click", function () {
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

    // Only display modal when at least 1 user is selected
    $("#delete-selected-users-modal-button").on("click", function (e) {
        if ($("input[type='checkbox'][name='user-row']:checked").length === 0) {
            Materialize.toast("No users selected", 2000);
        }
        else {
            $("#delete-selected-users-modal").openModal();
        }
    });

    // Delete button in the modal
    $("#delete-selected-users-button").on("click", function () {
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
                        $("#delete-selected-users-modal").closeModal();
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

    // Admin - Edit course
    $(".edit-course-form").on("submit", function () {
        var form = $(this);
        var formData = form.serializeArray();

        var sendData = {
            "Name": formData[0].value,
            "SemesterId": formData[1].value,
            "Year": formData[2].value,
            "Description": formData[3].value,
            // formData[4] is the __RequestVerificationToken
            "Id": formData[5].value,
            "CourseId": formData[6].value
        }

        $.ajax({
            url: form.attr("action"),
            data: sendData,
            method: "POST",
            success: function (responseData) {
                if (responseData) {
                    Materialize.toast("Course information updated", 4000);
                }
                else {
                    Materialize.toast("An error occurred", 4000);
                }
            },
            error: function () {
                // TODO
            }
        });
        return false;
    });

    $(".edit-course-button").on("click", function () {
        $(this).closest("form").submit();
    });

    // Admin - Add course button
    $(".add-course-button").on("click", function (e) {
        e.preventDefault();

        var container = $(this).parent().parent();

        var sendData = {
            "CourseId": container.find("select").val(),
            "UserId": container.closest("form").find(".user-id").val(),
            "Position": container.find("input[type='radio']:checked").val()
        }

        $.ajax({
            url: "/Admin/AddUserToCourse",
            data: sendData,
            method: "POST",
            success: function (responseData) {
                if (responseData) {
                    Materialize.toast("User added to course", 4000);
                }
                else {
                    Materialize.toast("An error occurred", 4000);
                }
            },
            error: function () {
                // TODO
            }
        });
    });

    // Admin - Remove user from course
    $(".remove-from-course").on("click", function (e) {
        e.preventDefault();

        var row = $(this).closest("tr");

        var userId = getUserId($(this));
        var courseInstanceId = row.find(".hiddendiv").html();
        var position = $(this).parent().prev().html().trim();

        switch (position) {
            case "Student":
                position = 1;
                break;

            case "Teacher":
                position = 2;
                break;

            case "Assistant":
                position = 3;
                break;
            default:
        }

        var sendData = {
            "UserId": userId,
            "CourseId": courseInstanceId,
            "Position": position
        }

        $.ajax({
            url: "/Admin/RemoveUserFromCourse",
            data: sendData,
            method: "POST",
            success: function (responseData) {
                if (responseData) {
                    row.remove();
                    Materialize.toast("User removed from course", 4000);
                }
                else {
                    Materialize.toast("An error occurred", 4000);
                }
            },
            error: function () {
                // TODO
            }
        });
    });

    // Admin - Update user
    $(".update-user-button").on("click", function () {
        var container = $(this).prev();

        var sendData = {
            "Id": getUserId($(this)),
            "FullName": container.find("input[type='text']:first").val(),
            "Email": container.find("input[type='text']:last").val()
        }

        $.ajax({
            url: "/Admin/EditUser",
            data: sendData,
            method: "POST",
            success: function (responseData) {
                if (responseData) {
                    Materialize.toast("User information updated", 4000);
                }
                else {
                    Materialize.toast("An error occurred", 4000);
                }
            },
            error: function () {
                // TODO
            }
        });
    });

    // Admin - Reset password for user
    $(".reset-password-button").on("click", function () {

        // Due to dropdowns acting strangely, getUserId cannot be used here
        var sendData = {
            "userId":  $(this).closest(".collapsible-header").parent().find("input[type='checkbox'][name='user-row']").val()
        }
        
        $.ajax({
            url: "/Admin/ChangePassword",
            data: sendData,
            method: "POST",
            success: function (responseData) {
                if (responseData) {
                    Materialize.toast("Password reset", 4000);
                }
                else {
                    Materialize.toast("An error occurred", 4000);
                }
            },
            error: function () {
                // TODO
            }
        });
    });
});