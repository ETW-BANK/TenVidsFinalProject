var dataTable;

$(document).ready(function () {
    const currentPage = getCurrentPage();

    if (currentPage === 'subscriptions') {
        loadSubscriptions();
    } else if (currentPage === 'history') {
        loadHistories();
    }
});

function getCurrentPage() {
   
    const url = window.location.href.toLowerCase();
    if (url.includes('subscriptions')) return 'subscriptions';
    if (url.includes('history')) return 'history';

   
    return $('body').data('current-page') || 'home';
}

function loadSubscriptions() {
    if ($.fn.DataTable.isDataTable('#tbData')) {
        $('#tbData').DataTable().destroy();
    }

    dataTable = $('#tbData').DataTable({
        "ajax": {
            "url": "/Home/GetSubscription",
            "dataSrc": "result"
        },
        "columns": [
            {
                "data": {
                    channelName: "channelName",
                    id: "id"
                },
                "render": function (data) {
                    return `<a href="/Member/Channel/${data.id}" class="text-dark">${data.channelName}</a>`;
                }
            },
            {
                "data": "videosCount",
                "className": "text-center"
            }
        ]
    });
}

function loadHistories() {
    if ($.fn.DataTable.isDataTable('#tbData')) {
        $('#tbData').DataTable().destroy();
    }

    dataTable = $('#tbData').DataTable({
        "ajax": {
            "url": "/Home/GetHistories",
            "dataSrc": "result"
        },
        "columns": [
            {
                "data": {
                    "title": "title",
                    "id": "id"
                },
                "render": function (data) {
                    return `<a href="/Video/WatchVideos/${data.id}" class="text-dark">${data.title}</a>`;
                }
            },
            {
                "data": {
                    "channelName": "channelName",
                    "channelId": "channelId"
                },
                "render": function (data) {
                    return `<a href="/Member/Channel/${data.channelId}" class="text-dark">${data.channelName}</a>`;
                }
            },
            {
                "data": {
                    "lastVisit": "lastVisit",
                    "lastVisitTimeAgo": "lastVisitTimeAgo"
                },
                "render": function (data) {
                    return `<span class="text-dark"><span class="hidden">${data.lastVisit}</span>${data.lastVisitTimeAgo}</span>`;
                }
            }
        ]
    });
}