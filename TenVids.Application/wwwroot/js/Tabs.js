// Global variables
let pageNumber = 1;
let pageSize = 10;
let sortBy = '';
let isLoading = false;

$(document).ready(function () {
    initializeTabs();
    initializeSorting();
    initializePageSizeButtons();
    loadVideos();
});

function initializeTabs() {
    // Set active tab from localStorage or default to myvideos
    const activeTab = localStorage.getItem('activeTab') || '#myvideos';
    $(`a[href="${activeTab}"]`).tab('show');

    // Handle tab changes
    $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
        const targetTab = $(e.target).attr('href');
        localStorage.setItem('activeTab', targetTab);

        if (targetTab === '#myvideos') {
            loadVideos();
        }
    });
}

function initializeSorting() {
    $('.sortBy').off('click').on('click', function () {
        const sortValue = $(this).data('value');
        $('.sortBy i').remove();

        if (sortBy === sortValue + '-a') {
            sortBy = sortValue + '-d';
            $(this).append('<i class="bi bi-arrow-down-short ms-1"></i>');
        } else {
            sortBy = sortValue + '-a';
            $(this).append('<i class="bi bi-arrow-up-short ms-1"></i>');
        }

        pageNumber = 1;
        loadVideos();
    });
}

function initializePageSizeButtons() {
    $('.pageSizeBtn').off('click').on('click', function () {
        const newPageSize = parseInt($(this).data('value'));
        if (pageSize !== newPageSize) {
            pageSize = newPageSize;
            pageNumber = 1;
            loadVideos();
        }
    });
}

function loadVideos() {
    if (isLoading) return;

    isLoading = true;
    $('#loadingIndicator').show();

    const parameters = {
        pageNumber: pageNumber,
        pageSize: pageSize,
        sortBy: sortBy
    };

    $.ajax({
        url: "/Video/GetVideosForChannelGrid",
        type: "GET",
        data: parameters,
        success: function (response) {
            if (response && response.isSuccess && response.result) {
                const result = response.result;
                populateVideoTable(result.items);
                updatePaginationControls(result);
            } else {
                showError(response?.message || 'Failed to load videos');
                $('#videosTableBody').html('<tr><td colspan="10" class="text-center">No videos found</td></tr>');
                resetPaginationControls();
            }
        },
        error: function (xhr) {
            showError('Error loading videos. Please try again.');
            console.error('Error loading videos:', xhr.responseText);
        },
        complete: function () {
            isLoading = false;
            $('#loadingIndicator').hide();
        }
    });
}

function populateVideoTable(videos) {
    let html = '';

    if (videos.length > 0) {
        videos.forEach((video, index) => {
            const rowNumber = ((pageNumber - 1) * pageSize) + index + 1;
            const watchUrl = `/Video/WatchVideos/${video.id}`;

            html += `
                            <tr>
                                <td>${rowNumber}</td>
                                <td><a href="${watchUrl}"><img src="${video.thumbnailUrl}" class="img-thumbnail" style="width: 120px; height: 80px;"></a></td>
                                <td><a href="${watchUrl}" class="video-title">${video.title}</a></td>
                                <td>${formatDate(video.createdAt)}</td>
                                <td>${video.views?.toLocaleString() || 0}</td>
                                <td>${video.comments?.toLocaleString() || 0}</td>
                                <td>${video.likes?.toLocaleString() || 0}</td>
                                <td>${video.dislikes?.toLocaleString() || 0}</td>
                                <td>${video.categoryName || ''}</td>
                                <td>
                                    <a href="/Video/Upsert/${video.id}" class="btn btn-warning btn-sm me-1"><i class="bi bi-pencil"></i></a>
                                    <button onclick="deleteVideo(${video.id}, '${escapeSingleQuote(video.title)}')" class="btn btn-danger btn-sm"><i class="bi bi-trash"></i></button>
                                </td>
                            </tr>`;
        });
    } else {
        html = '<tr><td colspan="10" class="text-center">No videos found</td></tr>';
    }

    $('#videosTableBody').html(html);
}

function updatePaginationControls(result) {
    $('#paginationBtnGroup').empty();

    if (result.totalCount <= 0) {
        resetPaginationControls();
        return;
    }

    // Calculate display values
    const from = ((result.pageNumber - 1) * result.pageSize) + 1;
    const to = Math.min(result.pageNumber * result.pageSize, result.totalCount);

    // Update summary displays
    $('#paginationSummery').text(`${from}-${to} of ${result.totalCount.toLocaleString()}`);
    $('#itemsPerPageDisplay').text(result.pageSize);

    // Create pagination buttons
    const buttons = [];

    // First Page button
    buttons.push(`
                        <button type="button" class="btn btn-secondary btn-sm paginationBtn"
                            ${result.pageNumber === 1 ? 'disabled' : ''}
                            data-value="1"
                            title="First Page">
                            <i class="bi bi-chevron-bar-left"></i>
                        </button>
                    `);

    // Previous Page button
    buttons.push(`
                        <button type="button" class="btn btn-secondary btn-sm paginationBtn"
                            ${result.pageNumber === 1 ? 'disabled' : ''}
                            data-value="${result.pageNumber - 1}"
                            title="Previous Page">
                            <i class="bi bi-chevron-left"></i>
                        </button>
                    `);

    // Page number buttons
    const startPage = Math.max(1, result.pageNumber - 2);
    const endPage = Math.min(result.totalPages, result.pageNumber + 2);

    for (let i = startPage; i <= endPage; i++) {
        buttons.push(`
                            <button type="button" class="btn btn-sm paginationBtn
                                ${i === result.pageNumber ? 'btn-primary' : 'btn-secondary'}"
                                data-value="${i}">${i}</button>
                        `);
    }

    // Next Page button
    buttons.push(`
                        <button type="button" class="btn btn-secondary btn-sm paginationBtn"
                            ${result.pageNumber >= result.totalPages ? 'disabled' : ''}
                            data-value="${result.pageNumber + 1}"
                            title="Next Page">
                            <i class="bi bi-chevron-right"></i>
                        </button>
                    `);

    // Last Page button
    buttons.push(`
                        <button type="button" class="btn btn-secondary btn-sm paginationBtn"
                            ${result.pageNumber >= result.totalPages ? 'disabled' : ''}
                            data-value="${result.totalPages}"
                            title="Last Page">
                            <i class="bi bi-chevron-bar-right"></i>
                        </button>
                    `);

    $('#paginationBtnGroup').html(buttons.join(''));

    // Bind click events to pagination buttons
    $('.paginationBtn').off('click').on('click', function () {
        pageNumber = parseInt($(this).data('value'));
        loadVideos();
    });
}

function resetPaginationControls() {
    $('#paginationSummery').text('0-0 of 0');
    $('#itemsPerPageDisplay').text('0');
    $('#paginationBtnGroup').empty();
}

function formatDate(dateString) {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString();
}

function escapeSingleQuote(str) {
    return str.replace(/'/g, "\\'");
}

function showError(message) {
    // Using Toastr for notifications (make sure you have toastr.js included)
    if (typeof toastr !== 'undefined') {
        toastr.error(message);
    } else {
        alert(message);
    }
}

async function deleteVideo(id, name) {
    const result = await Swal.fire({
        title: 'Are you sure?',
        text: `You are about to delete "${name}"`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Yes, delete it!'
    });

    if (result.isConfirmed) {
        try {
            const response = await fetch(`/Video/DeleteVideo`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                body: JSON.stringify({ Id: id })
            });

            const data = await response.json();

            if (data.statusCode === 200) {
                await Swal.fire('Deleted!', 'Your video has been deleted.', 'success');
                loadVideos();
            } else {
                await Swal.fire('Error!', data.message || 'Failed to delete video', 'error');
            }
        } catch (error) {
            await Swal.fire('Error!', 'An error occurred while deleting the video', 'error');
        }
    }
}

function editProfile() {
    $('#profile_name, #profile_about').prop('disabled', false);
    $('#divEditButton').hide();
    $('#updateProfileButtons').show();
}

function cancelEdit() {
    $('#profile_name, #profile_about').prop('disabled', true);
    $('#divEditButton').show();
    $('#updateProfileButtons').hide();
    // Reset values
    $('#profile_name').val('@Model.Name');
    $('#profile_about').val('@Model.About');
}