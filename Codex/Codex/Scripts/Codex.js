$(document).ready(function () {
    // Initialization
    $(".modal-trigger").leanModal();
    $(".tab > a").click(function() {
        $(this).unbind("click");
    });

    // Admin - Create new user form
    $("#create-user-form").on("submit", function() {
        console.log("Creating new user...");
        var form = $(this);

        var formData = {
            "Name": $("#NewUserModel_Name").val(),
            "Email": $("#NewUserModel_Email").val(),
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
        console.log("Clicked");
        var users = $("input[type='checkbox'][name='user-row']:checked").map(function() {
            return this.value;
        }).get();

        $.ajax({
            url: form.attr("action"),
            data: users,
            method: "POST",
            success: function (responseData) {
                if (responseData) {
                    Materialize.toast("Users deleted", 4000);
                    
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