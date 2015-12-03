
//This script is the same for each jquery-datatable.
//Define your own functions to extend the column rendering or custom error/succes message

var mvc = { 'JQuery': { 'Datatables': { ajax: {}, tableTools: {}, column: {} } } };

mvc.JQuery.Datatables.column.createMailToLink = function (data, type, full, meta) {
    return '<a href=mailto:' + data + '>' + data + '</a>';
}
mvc.JQuery.Datatables.column.editButton = function (data, type, full, meta) {
    //return '<div class="btn btn-primary btn-sm edit">Edit</div>';
    return '<i class="mdi-content-create edit" ></i>';
}
mvc.JQuery.Datatables.column.deleteButton = function (data, type, full, meta) {
    //return '<div class="btn btn-primary btn-sm edit">Edit</div>';
    return '<i class="mdi-content-remove-circle delete" ></i>';
}
mvc.JQuery.Datatables.column.editDeleteButton = function (data, type, full, meta) {
    //return '<div class="btn btn-primary btn-sm edit">Edit</div>';
    return '<i class="mdi-content-create edit" ></i><i class="mdi-content-remove-circle delete" ></i>';
}

mvc.JQuery.Datatables.column.formatDate = function (data, type, full, meta) {
    return data ? window.moment(data).format(meta.settings.aoColumns[meta.col].mvc6Par || 'YYYY-MM-DD HH:mm:ss') : null;
}
mvc.JQuery.Datatables.getLengthMenu = function () { return [[5, 50, 100, -1], [5, 50, 100, 'All']] };
mvc.JQuery.Datatables.getLanguage = function () { return {} };

var dbc = {};
//obj $this or javascript obj,bool,date,int,string
dbc.serverRequest = function (url, obj, success) {
    if (typeof obj === 'object' && typeof date.getMonth !== 'function')
    {
        $.ajax({
            type: 'post',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            url: url,
            data: JSON.stringify(obj),
            success: success,
            error: dbc.error
        });
    }
    if (obj instanceof jQuery) {
        $.ajax({
            type: 'post',
            url: url,
            data: obj.closest('form').serialize(),
            success: success,
            error: dbc.error
        });
    }
    else {
        $.ajax({
            type: 'post',
            url: url+'/'+ obj,
            success: success,
            error: dbc.error
        });
    }
};
dbc.error = function (jqXHR, textStatus, errorThrown) {
    var newWindow = window.open();
    if (newWindow)
        newWindow.document.write(jqXHR.responseText || textStatus + ':' + errorThrown);
    else
        window.alert(errorThrown + "\n\n\nAllow popups to view full errors details!");
};
dbc.snackbar = function (data) {
    $.snackbar({ content: data });
}






// Bind every table with the class datatable
$(document).ready(function () {
    $('table.datatables').each(function () {
        $(this).MvcDatatable({
            leaveMessage: 'De wijzigingen zijn niet opgeslagen! toch door gaan?',
            error: dbc.error,
            succes: function (data, textStatus, jqXHR) { }
        });
    });
});

/* start progress-bar */
(function ($) {
    $.fn.progress = function (precent) {
        if (precent === 0) {//this.hide();
            this.css("display", "none")
            this.css("width", precent + "%");
        }
        else
            this.show(0, function () {
                $(this).css("width", precent + "%");
            });
    };
}(jQuery));


