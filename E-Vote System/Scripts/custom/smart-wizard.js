var wizard = $('#smartwizard').smartWizard({
    selected: 0, // Initial selected step, 0 = first step
    loader: 'show',
    theme: 'dots', // theme for the wizard, related css need to include for other than default theme
    justified: true, // Nav menu justification. true/false
    darkMode: false, // Enable/disable Dark Mode if the theme supports. true/false
    autoAdjustHeight: true, // Automatically adjust content height
    cycleSteps: false, // Allows to cycle the navigation of steps
    backButtonSupport: true, // Enable the back button support
    enableURLhash: false, // Enable selection of the step based on url hash
    transition: {
        animation: 'none', // Effect on navigation, none/fade/slide-horizontal/slide-vertical/slide-swing
        speed: '400', // Transion animation speed
        easing: '' // Transition animation easing. Not supported without a jQuery easing plugin
    },
    toolbarSettings: {
        toolbarPosition: 'bottom', // none, top, bottom, both
        toolbarButtonPosition: 'right', // left, right, center
        showNextButton: true, // show/hide a Next button
        showPreviousButton: true, // show/hide a Previous button 
        toolbarExtraButtons: [ // Extra buttons to show on toolbar, array of jQuery input/buttons elements
            $('<button></button>').text('Submit')
                //.attr("type", "button")
                .css("background-color", "#ff5252")
                .css("border-color", "#ff5252")
                //.css("display", "none")
                .addClass('btn btn-danger smartwizard_finish sw-btn-group-extra d-none')
                .on('click', function () {
                    //alert('Finish button click');
                })
        ]



    },
    anchorSettings: {
        anchorClickable: false, // Enable/Disable anchor navigation
        enableAllAnchors: false, // Activates all anchors clickable all times
        markDoneStep: true, // Add done state on navigation
        markAllPreviousStepsAsDone: true, // When a step selected by url hash, all previous steps are marked done
        removeDoneStepOnNavigateBack: false, // While navigate back done step after active step will be cleared
        enableAnchorOnDoneStep: true // Enable/Disable the done steps navigation
    },
    keyboardSettings: {
        keyNavigation: true, // Enable/Disable keyboard navigation(left and right keys are used if enabled)
        keyLeft: [37], // Left key code
        keyRight: [39] // Right key code
    },
    lang: { // Language variables for button
        next: 'Next',
        previous: 'Previous'
    },
    disabledSteps: [], // Array Steps disabled
    errorSteps: [], // Highlight step with errors
    hiddenSteps: [], // Hidden steps

    
    //anchorClickable: false

});



//The code for the final step
$(wizard).on("leaveStep", function (e, anchorObject, stepNumber, stepDirection) {


    let parent = $('#smartwizard');

    let panes = parent.find('.tab-pane');


    if (stepDirection == `${panes.length - 1}`)
    {
        $('.sw-btn-group-extra').removeClass('d-none');
    }
    else {
        $('.sw-btn-group-extra').addClass('d-none');
    }

    /*if (stepDirection == "2") {
        $('.sw-btn-group-extra').removeClass('d-none');
    }
    else {
        $('.sw-btn-group-extra').addClass('d-none');
    }*/
});