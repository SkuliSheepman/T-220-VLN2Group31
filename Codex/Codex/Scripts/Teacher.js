$(document).ready(function () {

    /* Initialization */

    // Date picker
    $(".datepicker").pickadate({
        selectMonths: true, // Creates a dropdown to control month
        selectYears: 15 // Creates a dropdown of 15 years to control year
    });

    /* Global variables */
    var PROBLEMS = 1;
    var DROPDOWN_ASSIGNMENT_ID = 0;
    var DROPDOWN_PROBLEM_ID = 0;

    /* Ajax functions */

    // Set test cases for a problem
    function setTestCasesForProblem(id, testCases, problemCreationFlag, hasAttachment) {
        $.ajax({
            url: "/Teacher/UpdateTestCases",
            data: JSON.stringify({ testCases: testCases, problemId : id }),
            method: "POST",
            contentType: "application/json",
            dataType: "json",
            success: function (responseData) {
                if (responseData) {
                    if (problemCreationFlag == true) {
                        if (hasAttachment == true) {
                            setAttachmentForProblem(id, $("#new-problem-attachment").prop("files")[0], true);
                        }
                        else {
                            Materialize.toast("Problem created", 4000);
                        }
                    }
                    else {
                        if (hasAttachment == true) {
                            setAttachmentForProblem(id, $("#new-problem-attachment").prop("files")[0], false);
                        }
                        else {
                            Materialize.toast("Problem edited", 4000);
                        }
                        
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

    /* Helper functions */

    // Fill in the form the assignment's info to be edited
    function fillInAssignmentEdit(assignment) {
        $("#edit-assignment-id").val(DROPDOWN_ASSIGNMENT_ID).focus();
        $("#edit-assignment-name").val(assignment.Name).focus();
        $("#edit-assignment-description").val(assignment.Description).focus();

        // Start time
        var split = assignment.StartTime.split(".");

        $("#edit-assignment-start-date").pickadate("picker")
            .set("select", new Date(split[2].split(" ")[0], split[1] - 1, split[0]));

        // End time
        split = assignment.EndTime.split(".");

        $("#edit-assignment-end-date").pickadate("picker")
            .set("select", new Date(split[2].split(" ")[0], split[1] - 1, split[0]));

        $("#edit-assignment-collaborators").val(assignment.MaxCollaborators).focus();

        // Problems
        
        $.each(assignment.Problems, function(i, problem) {
            newProblemEntry($("#edit-assignment-form .btn-floating"));

            var row = $("#edit-assignment-form .btn-floating").prev();

            row.find("select").val(problem.ProblemId).material_select();
            row.find("input[id^='new-assignment-weight']").val(problem.Weight).focus();
            row.find("input[id^='new-assignment-submission']").val(problem.MaxSubmissions).focus();
        });
    }

    // Fill in the form the problems's info to be edited
    function fillInProblemEdit(problem) {
        $("#edit-problem-id").val(problem.Id).focus();
        $("#edit-problem-name").val(problem.Name).focus();
        $("#edit-problem-description").val(problem.Description).focus();
        $("#edit-problem-language").val(problem.Language).focus();
        $("#edit-problem-filetype").val(problem.Filetype).focus();

        // Test cases
        $.each(problem.TestCases, function (i, testCase) {
            newTestCaseEntry($("#edit-problem-form .btn-floating"));

            var panel = $("#edit-problem-form .btn-floating").prev();

            panel.find("textarea:first").val(testCase.Input);
            panel.find("textarea:last").val(testCase.Output);
            panel.find("input.test-case-id").val(testCase.Id).focus();
        });
    }

    // Add problem entry
    function newProblemEntry(button){
        PROBLEMS++;
        var row = $("<div class='row new-assignment-problemEntry'><i class='material-icons red-text'>clear</i></div>");

        // Problems
        var select = $("<select></select>");
        $("#new-assignment-problemlist option").clone().appendTo(select);

        select = select.after("<label>Problem</label>");

        var container = $("<div class='input-field col s4 new-assignment-problem-list'></div>");
        select.appendTo(container);

        container.appendTo(row);

        // Weight
        var weight = $("<div class='input-field col s4'><input type='number' min='0' id='new-assignment-weight" + PROBLEMS + "' name='new-assignment-weight" + PROBLEMS + "'/><span class='field-validation-valid text-danger' data-valmsg-for='new-assignment-weight" + PROBLEMS + "' data-valmsg-replace='true'></span><label for='new-assignment-weight" + PROBLEMS + "'>Weight %</label></div>");

        weight.appendTo(row);

        // Max submissions
        var maxSubmissions = $("<div class='input-field col s4'><input type='number' min='0' id='new-assignment-submission" + PROBLEMS + "' name='new-assignment-submission" + PROBLEMS + "'/><span class='field-validation-valid text-danger' data-valmsg-for='new-assignment-submission" + PROBLEMS + "' data-valmsg-replace='true'></span><label for='new-assignment-submission" + PROBLEMS + "'>Max submissions</label></div>");

        maxSubmissions.appendTo(row);

        // Insert
        button.before(row);

        $("select").material_select();
    }

    // Insert a new test case block before the button
    function newTestCaseEntry(button) {
        var inputOutput = "<div class='card-panel new-problem-input'>" +
                            "<input type='hidden' class='test-case-id' value=''/>" +
                            "<i class='material-icons red-text'>clear</i>" +
                            "<p>Input</p>" +
                            "<textarea class='materialize-textarea'></textarea>" +
                            "<p>Output</p>" +
                            "<textarea class='materialize-textarea'></textarea>" +
                          "</div>";
        button.before(inputOutput);
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
    $("#new-problem-modal-create-button").on("click", function (e) {

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
        
        var hasAttachment = ($("#new-problem-attachment").val().length != 0);


        $.ajax({
            url: $("#create-problem-form").attr("action"),
            data: formData,
            method: "POST",
            dataType: "json",
            success: function (responseData) {
                if (responseData !== 0) {
                    // Set test cases
                    setTestCasesForProblem(responseData, testCases, true, hasAttachment);
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

    // Edit problem form
    $("#edit-problem-modal-edit-button").on("click", function (e) {

        var testCases = [];
        $("#edit-problem-form .new-problem-input").each(function () {
            var id = $(this).find("input.test-case-id").val();
            var input = $(this).find("textarea:first").val();
            var output = $(this).find("textarea:last").val();

            testCases.push({ "Id": id, "Input": input, "Output": output });
        });

        var formData = {
            "Id": $("#edit-problem-id").val(),
            "CourseName": $("#edit-problem-course").val(),
            "Name": $("#edit-problem-name").val(),
            "Description": $("#edit-problem-description").val(),
            "Filetype": $("#edit-problem-filetype").val(),
            "Language": $("#edit-problem-language").val(),
            "TestCases": testCases
        }

        $.ajax({
            url: $("#edit-problem-form").attr("action"),
            data: formData,
            method: "POST",
            dataType: "json",
            success: function (responseData) {
                if (responseData) {
                    Materialize.toast("Problem edited", 4000);
                    $("#edit-problem-modal").closeModal();
                }
                else {
                    Materialize.toast("An error occurred editing the problem", 4000);
                }

            },
            error: function () {
                Materialize.toast("Something awful happened :(", 4000);
            }
        });
    });

    $("#create-problem-form .btn-floating, #edit-problem-form .btn-floating").on("click", function () {
        newTestCaseEntry($(this));
    });

    // Remove test case from new problem
    $("body").on("click", ".new-problem-input i, .new-assignment-problemEntry i", function () {
        $(this).parent().fadeOut(200, function() {
            $(this).remove();
        });
    });

    // New assignment form
    $("#new-assignment-modal-create-button").on("click", function (e) {

        var problems = [];
        $("#create-assignment-form .new-assignment-problemEntry").each(function () {
            var problemId = $(this).find("select").val();
            var weight = $(this).find("input[type='number']:first").val();
            var maxSubmissions = $(this).find("input[type='number']:last").val();

            problems.push({ "ProblemId": problemId, "Weight": weight, "MaxSubmissions": maxSubmissions });
        });

        var formData = {
            "CourseInstanceId": $("#new-assignment-course").val(),
            "Name": $("#new-assignment-name").val(),
            "Description": $("#new-assignment-description").val(),
            "MaxCollaborators": $("#new-assignment-collaborators").val(),
            "StartTime": $("#new-assignment-start-date").val(),
            "EndTime": $("#new-assignment-end-date").val(),
            "Problems": problems
        };

        $.ajax({
            url: $("#create-assignment-form").attr("action"),
            data: JSON.stringify(formData),
            method: "POST",
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (responseData) {
                if (responseData) {
                    Materialize.toast("Assignment created", 4000);
                }
                else {
                    Materialize.toast("An error occurred creating an assignment", 4000);
                }

            },
            error: function () {
                Materialize.toast("Something awful happened :(", 4000);
            }
        });
    });

    // Add problem to new assignment
    $("#create-assignment-form .btn-floating").on("click", function () {
        newProblemEntry($(this));
    });

    // Add problem to edit assignment
    $("#edit-assignment-form .btn-floating").on("click", function () {
        newProblemEntry($(this));
    });

    // Remove problem from new assignment
    $("body").on("click", ".new-problem-input i", function () {
        $(this).parent().fadeOut(500, function () {
            $(this).remove();
        });
    });

    // Edit assignment form
    $("#edit-assignment-modal-edit-button").on("click", function (e) {

        var problems = [];
        $("#edit-assignment-form .new-assignment-problemEntry").each(function () {
            var problemId = $(this).find("select").val();
            var weight = $(this).find("input[type='number']:first").val();
            var maxSubmissions = $(this).find("input[type='number']:last").val();

            problems.push({ "ProblemId": problemId, "Weight": weight, "MaxSubmissions": maxSubmissions });
        });

        var formData = {
            "Id": $("#edit-assignment-id").val(),
            "CourseInstanceId": $("#edit-assignment-course").val(),
            "Name": $("#edit-assignment-name").val(),
            "Description": $("#edit-assignment-description").val(),
            "MaxCollaborators": $("#edit-assignment-collaborators").val(),
            "StartTime": $("#edit-assignment-start-date").val(),
            "EndTime": $("#edit-assignment-end-date").val(),
            "Problems": problems
        };

        $.ajax({
            url: $("#edit-assignment-form").attr("action"),
            data: JSON.stringify(formData),
            method: "POST",
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (responseData) {
                if (responseData) {
                    Materialize.toast("Assignment edited", 4000);
                }
                else {
                    Materialize.toast("An error occurred editing an assignment", 4000);
                }

            },
            error: function () {
                Materialize.toast("Something awful happened :(", 4000);
            }
        });
    });

    // Set problem and assignment ID when dropdown is clicked
    $(".problem-dropdown").on("click", function() {
        var idSplit = $(this).attr("data-activates").split("-");

        DROPDOWN_ASSIGNMENT_ID = idSplit[1];
        DROPDOWN_PROBLEM_ID = idSplit[2];
    });

    // Set assignment ID when dropdown is clicked
    $(".assignment-dropdown").on("click", function () {
        var idSplit = $(this).attr("data-activates").split("-");
        
        DROPDOWN_ASSIGNMENT_ID = idSplit[1];
    });

    // Delete assignment
    $("#delete-assignment-button").on("click", function() {
        var formData = {
            "assignmentId": DROPDOWN_ASSIGNMENT_ID
        };

        $.ajax({
            url: "/Teacher/DeleteAssignment",
            data: JSON.stringify(formData),
            method: "POST",
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (responseData) {
                if (responseData) {
                    Materialize.toast("Assignment deleted", 4000);
                }
                else {
                    Materialize.toast("An error occurred deleting the assignment", 4000);
                }

            },
            error: function () {
                Materialize.toast("Something awful happened :(", 4000);
            }
        });
    });

    // Delete problem
    $("#delete-problem-button").on("click", function () {
        var formData = {
            "problemId": DROPDOWN_PROBLEM_ID
        };

        $.ajax({
            url: "/Teacher/DeleteProblem",
            data: JSON.stringify(formData),
            method: "POST",
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (responseData) {
                if (responseData) {
                    Materialize.toast("Problem deleted", 4000);
                }
                else {
                    Materialize.toast("An error occurred deleting the problem", 4000);
                }

            },
            error: function () {
                Materialize.toast("Something awful happened :(", 4000);
            }
        });
    });

    // Remove problem from assignment
    $("#remove-problem-button").on("click", function () {
        var formData = {
            "assignmentId": DROPDOWN_ASSIGNMENT_ID,
            "problemId": DROPDOWN_PROBLEM_ID
        };

        $.ajax({
            url: "/Teacher/RemoveProblem",
            data: JSON.stringify(formData),
            method: "POST",
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (responseData) {
                if (responseData) {
                    Materialize.toast("Problem removed", 4000);
                }
                else {
                    Materialize.toast("An error occurred removing the problem", 4000);
                }

            },
            error: function () {
                Materialize.toast("Something awful happened :(", 4000);
            }
        });
    });

    // Delete problem
    $(".delete-problem").on("click", function () {
        DROPDOWN_PROBLEM_ID = $(this).closest(".problem-list-entry").find(".hiddendiv").text();
    });

    // Edit assignment dropdown button
    $(".edit-assignment-button").on("click", function () {
        var formData = {
            "assignmentId": DROPDOWN_ASSIGNMENT_ID
        };
        
        $.ajax({
            url: "/Teacher/GetAssignmentForEdit",
            data: JSON.stringify(formData),
            method: "POST",
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (responseData) {
                if (responseData) {
                    fillInAssignmentEdit(responseData);
                }
                else {
                    Materialize.toast("An error occurred retrieving the assignment", 4000);
                }

            },
            error: function () {
                Materialize.toast("Something awful happened :(", 4000);
            }
        });
    });

    // Click problem entry in problem list
    $(".problem-list-entry").on("click", function () {
        $("#edit-problem-modal").openModal();

        var formData = {
            "problemId": $(this).find(".hiddendiv").text()
        };

        $.ajax({
            url: "/Teacher/GetProblemForEdit",
            data: JSON.stringify(formData),
            method: "POST",
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (responseData) {
                if (responseData) {
                    fillInProblemEdit(responseData);
                }
                else {
                    Materialize.toast("An error occurred retrieving the problem", 4000);
                }

            },
            error: function () {
                Materialize.toast("Something awful happened :(", 4000);
            }
        });
    });

});