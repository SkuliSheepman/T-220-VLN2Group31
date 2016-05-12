$(document).ready(function () {

    /* Initialization */

    // Date picker
    $(".datepicker").pickadate({
        selectMonths: true, // Creates a dropdown to control month
        selectYears: 15 // Creates a dropdown of 15 years to control year
    });

    /* Ajax functions */

    // Set test cases for a problem
    function setTestCasesForProblem(id, testCases, problemCreationFlag) {
        $.ajax({
            url: "/Teacher/UpdateTestCases",
            data: JSON.stringify({ testCases: testCases, problemId : id }),
            method: "POST",
            contentType: "application/json",
            dataType: "json",
            success: function (responseData) {
                if (responseData) {
                    if (problemCreationFlag === true) {
                        setAttachmentForProblem(id, $("#new-problem-attachment").prop("files")[0], true);
                    }
                }
                else {
                    Materialize.toast("An error occurred assigning the test cases to the problem", 4000);
                }

            },
            error: function () {
                Materialize.toast("Something awful happened :(", 4000);
            }
        });
    }

    // Upload attachment for a problem
    function setAttachmentForProblem(id, attachment, problemCreationFlag) {
        var formData = new FormData();
        formData.append("attachment", attachment);
        formData.append("problemId", id);

        $.ajax({
            url: "/Teacher/UpdateAttachment",
            data: formData,
            method: "POST",
            contentType: false,
            processData: false,
            success: function (responseData) {
                if (responseData === "success") {
                    if (problemCreationFlag === true) {
                        Materialize.toast("Problem created", 4000);
                    }
                    else {
                        Materialize.toast("Problem updated", 4000);
                    }
                }
                else if (responseData === "write") {
                    Materialize.toast("An error occurred while writing to the server", 4000);
                }
                else if (responseData === "database") {
                    Materialize.toast("An error occurred while inserting in the database", 4000);
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

    // New problem form
    $("#new-problem-modal-create-button").on('click', function (e) {

        var testCases = [];
        $("#create-problem-form .new-problem-input").each(function () {
            var input = $(this).find("textarea:first").val();
            var output = $(this).find("textarea:last").val();

            testCases.push({ "Input": input, "Output": output });
        });

        var formData = {
            "CourseName": $("#new-problem-course").val(),
            "Name": $("#new-problem-name").val(),
            "Description": $("#new-problem-description").val(),
            "Filetype": $("#new-problem-filetype").val(),
            "Language": $("#new-problem-language").val()
            //"TestCases": JSON.stringify(testCases)
        }
        
        $.ajax({
            url: $("#create-problem-form").attr("action"),
            data: formData,
            method: "POST",
            dataType: "json",
            success: function (responseData) {
                if (responseData !== 0) {
                    // Set test cases
                    setTestCasesForProblem(responseData, testCases, true);
                }
                else {
                    Materialize.toast("An error occurred adding the problem to the database", 4000);
                }

            },
            error: function () {
                Materialize.toast("Something awful happened :(", 4000);
            }
        });
    });

    $("#create-problem-form .btn-floating").on("click", function () {
        var inputOutput = "<div class='card-panel new-problem-input'>" +
                            "<i class='material-icons red-text'>clear</i>" +
                            "<p>Input</p>" +
                            "<textarea class='materialize-textarea'></textarea>" +
                            "<p>Input</p>" +
                            "<textarea class='materialize-textarea'></textarea>" +
                          "</div>";
        $(this).before(inputOutput);
    });

    // New problem form
    $("body").on("click", ".new-problem-input i", function () {
        $(this).parent().fadeOut(500, function() {
            $(this).remove();
        });
    });

});