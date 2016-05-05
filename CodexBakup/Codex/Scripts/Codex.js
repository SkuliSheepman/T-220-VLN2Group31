$(document).ready(function () {
    // Initialization
    $(".modal-trigger").leanModal();
    $(".tab > a").unbind("click");

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
                console.log(responseData);
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
                console.log(responseData);
            }
        });
        return false;
    });

    $("#create-course-button").on("click", function () {
        console.log("Clicked");
        $("#create-course-form").submit();
    });
});