toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": false,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "10000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};



var toastrAlert = function (alertMessage) {


    if (alertMessage.length > 0) {
        if (alertMessage != null && alertMessage != '') {

            var msgarray = alertMessage.split(';');

            console.log(msgarray);

            let type = msgarray[0];

            let title = 'Note';
            let message = msgarray[1];

            if (msgarray.length >= 3) {
                title = msgarray[msgarray.length - 1];
            }

            if (type == 'success') {

                title = title == 'Note' ? 'Success' : title;

                toastr.success(message, title);

            } else if (type == 'info') {

                title = title == 'Note' ? 'Message' : title;

                toastr.info(message, title);

            } else {

                title = title == 'Note' ? 'Error' : title;

                toastr.options.timeOut = 0;
                toastr.options.extendedTimeOut = 0;

                toastr.error(message, title);
            }

        }
    }

    

    
};