var urlRegex = /(http(s)?:\/\/.)?(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)/g
var emailRegex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/

jQuery(document).ready(function () {
    if (jQuery('#healtChecks'))
        jQuery('#healtChecks').DataTable();
});


function CloseHealtCheckModal() {
    $('#errorMessage').html('');
    $('#healtCheckName').val('');
    $('#healtCheckUrl').val('');
    $('#healtCheckInterval').val('');
    $('#healtCheckNotificationEmail').val('');
    $('#healtCheckId').val('');
}

function ValidateEndPoint(name, healtCheckUrl, intervalSeconds, notificationEmailAddress) {
    if (!name) {
        swal("Oops", "Please provide a name", "error");
        return false;
    }

    if (!healtCheckUrl.match(urlRegex)) {
        swal("Oops", "Please provide a valid url", "error");
        return false;
    }
    if ((+intervalSeconds) <= 0) {
        swal("Oops", "Please provide a valid interval which is greate than zero", "error");
        return false;
    }

    if (!notificationEmailAddress.match(emailRegex)) {
        swal("Oops", "Please provide a valid email address", "error");
        return false;
    }

    return true;
}

function SaveEndpoint() {
    let id = $('#healtCheckId').val();
    let name = $('#healtCheckName').val();
    let healtCheckUrl = $('#healtCheckUrl').val();
    let intervalSeconds = $('#healtCheckInterval').val();
    let notificationEmailAddress = $('#healtCheckNotificationEmail').val();

    if (!ValidateEndPoint(name, healtCheckUrl, intervalSeconds, notificationEmailAddress)) {
        return;
    }
    let type = "POST";
    let data = {
        Name: name.trim(),
        HealtCheckUrl: healtCheckUrl.trim(),
        IntervalSeconds: intervalSeconds.trim(),
        NotificationEmailAddress: notificationEmailAddress.trim()
    };
    if (id) {
        data.Id = id;
        type = "PUT"
    }
    jQuery.ajax({
        url: "/api/HealtCheckApi",
        type: type,
        data: JSON.stringify(data),
        contentType: "application/json",
        dataType: 'json',
        success: function (result) {
            if (result.isSuccess) {
                swal("Good job!", "Your changes are successful !", "success", {
                    button: "Close",
                }).then((value) => {
                    window.location.reload();
                });
            }
            else {
                swal("Oops", result.errorMessage, "error");
            }
        },
        error: function (result) {
            console.log(result);
        }
    })

}

function ShowEndpoint(id) {
    jQuery.ajax({
        url: "/api/HealtCheckApi/" + id,
        type: "GET",
        contentType: "application/json",
        dataType: 'json',
        success: function (result) {
            if (result.isSuccess) {
                $('#healtCheckId').val(id);
                $('#healtCheckName').val(result.data.name);
                $('#healtCheckUrl').val(result.data.healtCheckUrl);
                $('#healtCheckInterval').val(result.data.intervalSeconds);
                $('#healtCheckNotificationEmail').val(result.data.notificationEmailAddress);
                $('#healtCheckModal').modal();
            }
        },
        error: function (result) {
            console.log(result);
        }
    })
}

function DeleteEndpoint(id) {
    swal({
        title: "Are you sure?",
        text: "Once deleted, you will not be able to recover this",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            jQuery.ajax({
                url: "/api/HealtCheckApi/" + id,
                type: "DELETE",
                contentType: "application/json",
                dataType: 'json',
                success: function (result) {
                    if (result.data && result.isSuccess) {
                        swal("The record has been deleted!", {
                            icon: "success",
                        }).then(()=>{
                            window.location.reload();
                        });
                    }
                    else {
                        swal("Oops", result.errorMessage, "error");
                    }
                },
                error: function (result) {
                    console.log(result);
                }
            })
        }
    });
}

function ShowMetrics(id) {
    jQuery.ajax({
        url: "/api/HealtCheckApi/GetMetricsHealtCheckById/" + id,
        type: "GET",
        contentType: "application/json",
        dataType: 'json',
        success: function (result) {
            if (result.isSuccess) {
                console.log(result.data);
            }
        },
        error: function (result) {
            console.log(result);
        }
    })

}