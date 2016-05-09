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

    // Get course ID from row in admin view with supplied descendant
    function getCourseId(elem) {
        return elem.closest("li").find("input[type='checkbox'][name='course-row']").val();
    }

    /* ADMIN */

    // Admin - Create new user form
    $("#create-user-form").on("submit", function() {
        var form = $(this);

        var formData = {
            "Name": $("#NewUserModel_Name").val(),
            "Email": $("#NewUserModel_Email").val(),
            "Admin": $("#NewUserModel_Admin").is(":checked"),
            "UserCourses": [
                {

                }
            ]
        }
        
        if (formData.Name.length <= 0) {
            Materialize.toast("Name missing!", 2000);
        }
        else if (formData.Email.length <= 0) {
            Materialize.toast("Email missing!", 2000);
        }
        else if (!isEmail(formData.Email)) {
            Materialize.toast("Invalid email!", 2000);
        }
        else{
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
        
        return false;
    });

    $("#create-user-button").on("click", function () {
        $("#create-user-form").submit();
    });

    // Admin - Create new course form
    $("#create-course-form").on("submit", function () {
        var form = $(this);

        var formData = {
            "Name": $("#NewCourseModel_Name").val(),
            "Description": "",
            "Semester": $("input[type='radio'][name='NewCourseModel.Semester']:checked").val(),
            "Year": $("#NewCourseModel_Year").val()
        }

        if (formData.Name.length <= 0) {
            Materialize.toast("Name missing!", 2000);
        }
        else if (formData.Year.length <= 0) {
            Materialize.toast("Year missing!", 2000);
        }
        else if (formData.Year < 2000) {
            Materialize.toast("Year must be at least 2000!", 2000);
        }
        else {
            $.ajax({
                url: form.attr("action"),
                data: formData,
                method: "POST",
                success: function (responseData) {
                    Materialize.toast("The course " + formData.Name + " has been created", 4000);
                },
                error: function () {
                    // TODO
                }
            });
        }
        
        return false;
    });

    $("#create-course-button").on("click", function (e) {
        e.preventDefault();

        $("#create-course-form").submit();
    });

    // Admin - Delete selected users

    // Only display modal when at least 1 user is selected
    $("#delete-selected-users-modal-button").on("click", function (e) {
        e.preventDefault();

        if ($("input[type='checkbox'][name='user-row']:checked").length === 0) {
            Materialize.toast("No users selected", 2000);
        }
        else {
            $("#delete-selected-users-modal").openModal();
        }
    });

    // Delete button in the modal
    $("#delete-selected-users-button").on("click", function (e) {
        e.preventDefault();

        var courses = $("input[type='checkbox'][name='user-row']:checked").map(function () {
            return this.value;
        }).get();

        if (0 < courses.length) {
            $.ajax({
                url: "/Admin/DeleteSelectedUsers",
                data: JSON.stringify(courses),
                method: "POST",
                contentType: "application/json",
                success: function (responseData) {
                    if (responseData) {
                        Materialize.toast("Users deleted", 4000);
                        $("input[type='checkbox'][name='user-row']:checked").each(function () {
                            $(this).closest("li").remove();
                        });
                        $("#delete-selected-users-modal").closeModal();
                    }
                    else {
                        Materialize.toast("An error occurred", 4000);
                    }

                },
                error: function () {
                    // TODO
                }
            });
        }
        else {
            Materialize.toast("No users selected", 2000);
        }
    });

    // Admin - Delete selected courses

    // Only display modal when at least 1 course is selected
    $("#delete-selected-courses-modal-button").on("click", function (e) {
        e.preventDefault();

        if ($("input[type='checkbox'][name='course-row']:checked").length === 0) {
            Materialize.toast("No courses selected", 2000);
        }
        else {
            $("#delete-selected-courses-modal").openModal();
        }
    });

    // Delete button in the modal
    $("#delete-selected-courses-button").on("click", function (e) {
        e.preventDefault();

        var courses = $("input[type='checkbox'][name='course-row']:checked").map(function() {
            return this.value;
        }).get();

        if (0 < courses.length) {
            $.ajax({
                url: "/Admin/DeleteSelectedCourses",
                data: JSON.stringify(courses),
                method: "POST",
                contentType: "application/json",
                success: function(responseData) {
                    if (responseData) {
                        Materialize.toast("Courses deleted", 4000);
                        $("input[type='checkbox'][name='course-row']:checked").each(function() {
                            $(this).closest("li").remove();
                        });
                        $("#delete-selected-courses-modal").closeModal();
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
            Materialize.toast("No courses selected", 2000);
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
            //"Description": formData[2].value,
            // formData[3] is the __RequestVerificationToken
            "Id": formData[4].value,
            "CourseId": formData[5].value
        }

        if (sendData.Name.length <= 0) {
            Materialize.toast("Name missing!", 2000);
        }
        else if (sendData.Year.length <= 0) {
            Materialize.toast("Year missing!", 2000);
        }
        else if (sendData.Year < 2000) {
            Materialize.toast("Year must be at least 2000!", 2000);
        }
        else {
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
        }

        return false;
    });

    $(".edit-course-button").on("click", function (e) {
        e.preventDefault();
        $(this).closest("form").submit();
    });

    // Admin - Add course button for users
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

    // Admin - Remove teacher from course
    $(".remove-teacher-from-course").on("click", function(e) {
        e.preventDefault();

        var row = $(this).closest("tr");

        var userId = row.find(".hiddendiv").text();
        var courseInstanceId = getCourseId($(this));
        var position = $(this).parent().prev().text().trim();

        switch (position) {
            case "Teacher":
                position = 2;
                break;

            case "Assistant":
                position = 3;
                break;

            default:
                break;
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

    // Admin - Remove user from course
    $(".remove-from-course").on("click", function (e) {
        e.preventDefault();

        var row = $(this).closest("tr");

        var userId = getUserId($(this));
        var courseInstanceId = row.find(".hiddendiv").text();
        var position = $(this).parent().prev().text().trim();

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
                break;
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
        
        if (sendData.Name.length <= 0) {
            Materialize.toast("Name missing!", 2000);
        }
        else if (sendData.Email.length <= 0) {
            Materialize.toast("Email missing!", 2000);
        }
        else if (!isEmail(sendData.Email)) {
            Materialize.toast("Invalid email!", 2000);
        }
        else {
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
        }
        
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
});