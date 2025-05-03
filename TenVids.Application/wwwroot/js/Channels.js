// memberVideos.js
document.addEventListener('DOMContentLoaded', function () {
    // Initialize DataTable with configuration
    const dataTable = $('#tbData').DataTable({
        "ajax": {
            "url": `/Member/GetMemberChannelVideos?channelId=${$('#tbData').data('channel-id')}`,
            "dataSrc": 'result'
        },
        "columns": [
            {
                "data": {
                    id: "id",
                    title: "title",
                    thumbnailUrl: "thumbnailUrl"
                },
                "render": function (data) {
                    return `
                        <div class="row">
                            <div class="col-lg-6 text-center">
                                <a href="/Video/WatchVideos/${data.id}" class="text-dark video-link">
                                    ${data.title}
                                </a>
                            </div>
                            <div class="col-lg-6 text-center">
                                <a href="/Video/WatchVideos/${data.id}" class="thumbnail-link">
                                    <img src="${data.thumbnailUrl}" class="card-img-top rounded preview-image" />
                                </a>
                            </div>
                        </div>`;
                }
            },
            {
                "data": {
                    createdAt: "createdAt",
                    createdAtTimeAgo: "createdAtTimeAgo"
                },
                "render": function (data) {
                    return `<span class="text-dark"><span class="hidden">${data.createdAt}</span>${data.createdAtTimeAgo}</span>`;
                }
            },
            { "data": "numberOfViews" }
        ]
    });
});