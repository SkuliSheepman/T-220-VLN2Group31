$(".button-collapse").sideNav();

$('button[type="submit"]').click(function (e) {



    e.stopPropagation();
    e.preventDefault();
});

$("input:file").change(function (){
       var fileName = $(this)[0].files[0].name;
       var id = $(this).attr('id');
       var button = $('label[for="' + id + '"] > span');
       button.attr('data-tooltip', fileName);

       var form = $(this).parent();
       var submitButton = form.find('button');
       submitButton.removeClass('disabled', 1000, 'easeInBack');
       submitButton.tooltip('remove');
       submitButton.addClass("teal-text");
       submitButton.addClass("waves-effect");
       submitButton.addClass("waves-teal");
});

$('a[data-activates="dropdown1"]').click(function(e){
    e.preventDefault();
});
