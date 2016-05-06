$(".button-collapse").sideNav();

$('button[type="submit"]').click(function (e) {



    e.stopPropagation();
    //e.preventDefault();
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

  $(document).ready(function() {
    $('select').material_select();
  });

$(document).ready(function(){
  $('.collapsible').collapsible({
    accordion : false // A setting that changes the collapsible behavior to expandable instead of the default accordion style
  });
});

  $('.dropdown-button').dropdown({
      inDuration: 300,
      outDuration: 225,
      constrain_width: false, // Does not change width of dropdown to that of the activator
      hover: true, // Activate on hover
      gutter: 0, // Spacing from edge
      belowOrigin: false, // Displays dropdown below the button
      alignment: 'right' // Displays dropdown with edge aligned to the left of button
    }
  );

  $('.modal-trigger').leanModal({
      dismissible: false, // Modal can be dismissed by clicking outside of the modal
      opacity: .5, // Opacity of modal background
      in_duration: 300, // Transition in duration
      out_duration: 200, // Transition out duration
      // ready: function() { alert('Ready'); }, // Callback for Modal open
      // complete: function() { alert('Closed'); } // Callback for Modal close
    }
  );

  $('.datepicker').pickadate()

     