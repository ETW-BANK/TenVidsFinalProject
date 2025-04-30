var dataTable;

$(document).ready(function () {
    const currentPage = getCurrentPage();
    console.log('Current page:', currentPage);  // Debug

    if (currentPage === 'subscriptions') {
        loadSubscriptions();
    } else if (currentPage === 'history') {
        loadHistories();
    } else if (currentPage === 'likes') {
        console.log('Initializing likes table');
        loadLikeDislike(true);  // true for likes
    } else if (currentPage === 'dislikes') {
        console.log('Initializing dislikes table');
        loadLikeDislike(false);  // false for dislikes
    }
});

function getCurrentPage() {
    const url = window.location.href.toLowerCase();
    if (url.includes('subscriptions')) return 'subscriptions';
    if (url.includes('history')) return 'history';
    if (url.includes('likes')) return 'likes';  // Must match Model.Page
    if (url.includes('dislikes')) return 'dislikes';  // Must match Model.Page

    return $('body').data('current-page') || 'home';
}
function loadLikeDislike(liked) {
    if ($.fn.DataTable.isDataTable('#tbData')) {
        $('#tbData').DataTable().destroy();
    }

    dataTable = $('#tbData').DataTable({
        "ajax": {
            "url": "/Home/GetLikesDislikesVideos?liked=" + liked,
            "dataSrc": "result",
            "error": function (xhr, status, error) {
                console.error("Error loading likes/dislikes:", error);
            }
        },
        "columns": [
            {
                "data": null,
                "render": function (data, type, row) {
                    return `
                        <div class="row">
                            <div class="col-lg-6 text-center">
                                <a href="/Video/WatchVideos/${row.id}" class="text-dark">${row.title}</a>
                            </div>
                            <div class="col-lg-6 text-center">
                                <img src="${row.thumbnail}" class="card-img-top rounded preview-image" />
                            </div>
                        </div>`;
                }
            },
            {
                "data": "channelName",
                "render": function (data, type, row) {
                    return `<a href="/Member/Channel/${row.channelId}" class="text-dark">${data}</a>`;
                }
            },
            {
                "data": "createdAtTimeAgo",
                "render": function (data, type, row) {
                    return `<span class="text-dark"><span class="hidden">${row.createdAt}</span>${data}</span>`;
                }
            }
        ]
    });
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