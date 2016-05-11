$(document).ready(function () {

    $("#new-problem-modal-button").on('click', function (e) {
        //$("#new-problem-modal").openModal();
    });

    /* Initialization */
    $("#dateFilter .input-field").change(function(e) {
       

        var input = $("#date").val().split(' ');

        var formData = {

            year: input[0],
            semester: input[1]

        }

        DisplayTeacherCourses(formData);
        
    });

    /* Display courses */
    var DisplayTeacherCourses = function(formData) {

        if (formData == undefined) {
            //display current year's courses
        }

        $.ajax({
            url: "/Teacher/GetTeacherCoursesByDate",
            data: JSON.stringify(formData),
            method: "POST",
            contentType: "application/json",
            success: function (responseData) {
                if (responseData) {
                    var courses = responseData;
                    $("#teacherCourses").empty();
                    $.each(courses, function (key, course) {
                        $("#teacherCourses").append("<div class='col s'" + (4)+ "><a href='?year=" + course.Year + "&semester=" + course.Semester + "&courseInstanceId=" + course.Id + "' class='waves-effect waves-light btn'>" + course.Name + "</a></div>");
                    });
                }
                else {
                    Materialize.toast("An error occurred", 4000);
                }

            },
            error: function () {
                Materialize.toast("Something awful happened :(", 4000);
            }
        });

    }

    $("#add-test-case-button").on('click', function (e) {

        

    });

    $("#new-problem-modal-create-button").on('click', function(e) {

        var formData = {
            CourseId: 1,
            Name: "TestProblem",
            Description: "Hello world",
            Filetype: "cs",
            Attachment: "problem.cs",
            Language: "See sharp"
        }

        $.ajax({
            url: "/Teacher/UpdateProblem",
            data: JSON.stringify(formData),
            method: "POST",
            contentType: "application/json",
            success: function (responseData) {
                if (responseData) {
                    alert(JSON.stringify(responseData));
                    Materialize.toast("wohooo", 2000);
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