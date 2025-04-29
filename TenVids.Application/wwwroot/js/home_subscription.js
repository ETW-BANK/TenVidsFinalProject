var dataTable;

$(document).ready(function () {
    loadSubscriptions();
});
function loadSubscriptions() {
    if ($.fn.DataTable.isDataTable('#tblData')) {
        $('#tbData').DataTable().destroy();
    }
    dataTable = $('#tbData').DataTable({
        "ajax": {
            "url": "/Home/GetSubscription",
            dataSrc: 'result'
        },
        "columns": [
            {
                "data": {
                    channelName: "channelName"
                },
                "render": function (data) {
                    var aTag = `<a href="/Member/Channel/${data.id}" class="text-dark">${data.channelName}</a>`;
                    return aTag;
                }
            },
            { "data": "videosCount" }

        ]
    });
}