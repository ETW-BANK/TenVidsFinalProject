let pageNumber = 1;
let pageSize = 5;
let sortBy = '';

$(function () {
    // Initialize tab persistence
    $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
        localStorage.setItem('activeTab', $(e.target).attr('href'));
    });

    // Restore active tab
    var activeTab = localStorage.getItem('activeTab');
    if (activeTab) {
        $('a[href="' + activeTab + '"]').tab('show');
    } else {
        $('a[href="#myvideos"]').tab('show');
    }

    // Initialize tooltips
    $('[data-bs-toggle="tooltip"]').tooltip();

    // Load initial videos
    getMyVideos();
});

// Profile Editing Functions
function editProfile() {
    $('#updateProfileButtons').show();
    $('#profile_name').prop('disabled', false).trigger('focus');
    $('#profile_discription').prop('disabled', false);
    $('#divEditButton').hide();
}

function cancelEdit() {
    // Get values from data attributes
    const profileData = $('#profile-section');
    const originalName = profileData.data('name');
    const originalDescription = profileData.data('description');

    // Reset form fields
    $('#profile_name').val(originalName).prop('disabled', true);
    $('#profile_discription').val(originalDescription).prop('disabled', true);

    // Reset validation
    $('#profile_name-error').empty();
    $('#profile_discription-error').empty();
    $('#profile_name').removeClass('input-validation-error');
    $('#profile_discription').removeClass('input-validation-error');

    // Toggle buttons
    $('#divEditButton').show();
    $('#updateProfileButtons').hide();
}

// Video Management Functions
function getMyVideos() {
    showLoading();
    const parameters = {
        pageNumber: pageNumber,
        pageSize: pageSize,
        sortBy: sortBy
    };

    $.ajax({
        type: 'GET',
        url: '/Video/GetVideosForChannelGrid',
        data: parameters,
        success: function (response) {
            hideLoading();
            if (response.isSuccess) {
                const result = response.result;
                $('#videosTableBody').empty();
                $('#paginationSummery').empty();
                $('#paginationBtnGroup').empty();
                $('#itemsPerPageDisplay').empty();

                populateVideoTableBody(result.items);

                if (result.totalCount > 0) {
                    $('#itemsPerPageDisplay').text(pageSize);

                    const from = (result.pageNumber - 1) * result.pageSize + 1;
                    const to = Math.min(result.pageNumber * result.pageSize, result.totalCount);
                    $('#paginationSummery').text(`${from}-${to} of ${result.totalCount}`);

                    buildPaginationButtons(result);
                } else {
                    $('#itemsPerPageDropdown').hide();
                }
            } else {
                showError(response.message);
            }
        },
        error: function (xhr) {
            hideLoading();
            showError('Error loading videos. Please try again.');
            console.error('Error loading videos:', xhr.responseText);
        }
    });
}

function buildPaginationButtons(result) {
    let buttons = '';
    const currentPage = result.pageNumber;
    const totalPages = result.totalPages;

    // First Page button
    buttons += `<button type="button" class="btn btn-secondary btn-sm paginationBtn" 
               ${currentPage === 1 ? 'disabled' : ''} 
               data-value="1" 
               title="First Page">
               <i class="bi bi-chevron-bar-left"></i>
               </button>`;

    // Previous Page button
    buttons += `<button type="button" class="btn btn-secondary btn-sm paginationBtn" 
               ${currentPage === 1 ? 'disabled' : ''} 
               data-value="${currentPage - 1}" 
               title="Previous Page">
               <i class="bi bi-chevron-left"></i>
               </button>`;

    // Page number buttons
    const startPage = Math.max(1, currentPage - 2);
    const endPage = Math.min(totalPages, currentPage + 2);

    for (let i = startPage; i <= endPage; i++) {
        buttons += `<button type="button" class="btn btn-sm paginationBtn 
                   ${i === currentPage ? 'btn-primary' : 'btn-secondary'}" 
                   data-value="${i}">${i}</button>`;
    }

    // Next Page button
    buttons += `<button type="button" class="btn btn-secondary btn-sm paginationBtn" 
               ${currentPage >= totalPages ? 'disabled' : ''} 
               data-value="${currentPage + 1}" 
               title="Next Page">
               <i class="bi bi-chevron-right"></i>
               </button>`;

    // Last Page button
    buttons += `<button type="button" class="btn btn-secondary btn-sm paginationBtn" 
               ${currentPage >= totalPages ? 'disabled' : ''} 
               data-value="${totalPages}" 
               title="Last Page">
               <i class="bi bi-chevron-bar-right"></i>
               </button>`;

    $('#paginationBtnGroup').html(buttons);

    // Rebind click events
    $('.paginationBtn').off('click').on('click', function () {
        pageNumber = parseInt($(this).data('value'));
        getMyVideos();
    });
}

function populateVideoTableBody(videos) {
    let trTag = '';

    if (videos.length > 0) {
        videos.forEach((v, index) => {
            // Properly escape special characters in title
            const escapedTitle = v.title.replace(/"/g, '&quot;').replace(/'/g, "\\'");
            const position = (pageNumber - 1) * pageSize + index + 1;

            trTag += '<tr>';
            trTag += `<td>${position}</td>`;
            trTag += `<td><a href="/Video/Watch/${v.id}"><img src="${v.thumbnailUrl}" class="card-img-top rounded preview-image" alt="${escapedTitle}" /></a></td>`;
            trTag += `<td><p><a href="/Video/Watch/${v.id}" class="text-dark">${v.title}</a></p></td>`;
            trTag += `<td>${new Date(v.createdAt).toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' })}</td>`;
            trTag += `<td>${v.views.toLocaleString()}</td>`;
            trTag += `<td>${v.comments.toLocaleString()}</td>`;
            trTag += `<td>${v.likes.toLocaleString()}</td>`;
            trTag += `<td>${v.dislikes.toLocaleString()}</td>`;
            trTag += `<td>${v.categoryName}</td>`;
            trTag += '<td class="text-nowrap">';
            trTag += `<a href="/Video/Upsert/${v.id}" class="btn btn-warning text-white me-2" data-bs-toggle="tooltip" title="Edit"><i class="bi bi-pencil"></i></a>`;
            trTag += `<button class="btn btn-danger text-white me-2" onclick="deleteVideo(${v.id}, '${escapedTitle}')" data-bs-toggle="tooltip" title="Delete"><i class="bi bi-trash3"></i></button>`;
            trTag += '</td>';
            trTag += '</tr>';
        });
    } else {
        trTag = '<tr><td colspan="10" class="text-center">You don\'t have any videos</td></tr>';
    }

    $('#videosTableBody').html(trTag);
    $('[data-bs-toggle="tooltip"]').tooltip(); // Initialize tooltips for new elements
}

async function deleteVideo(id, title) {
    try {
        const result = await Swal.fire({
            title: 'Are you sure?',
            text: `You are about to delete "${title}"`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        });

        if (result.isConfirmed) {
            showLoading();
            const token = $('input[name="__RequestVerificationToken"]').val();

            // Debug: Log the ID before sending
            console.log('Attempting to delete video with ID:', id);

            const response = await $.ajax({
                url: '/Video/DeleteVideo',
                type: 'DELETE',
                contentType: 'application/json', // Important for parameter binding
                data: JSON.stringify({ id: id }), // Stringify the payload
                headers: {
                    'RequestVerificationToken': token,
                    'Content-Type': 'application/json'
                }
            });

            hideLoading();

            if (response.statusCode === 200) {
                await Swal.fire('Deleted!', response.message, 'success');
                getMyVideos();
            } else {
                await Swal.fire('Error!', response.message, 'error');
            }
        }
    } catch (error) {
        hideLoading();
        console.error('Delete error:', error);
        await Swal.fire('Error!', 'Failed to delete video', 'error');
    }
}


// Utility Functions
function showLoading() {
    $('#loadingIndicator').show();
}

function hideLoading() {
    $('#loadingIndicator').hide();
}

function showError(message) {
    Swal.fire({
        title: 'Error!',
        text: message,
        icon: 'error'
    });
}

// Event Listeners
$(document).on('click', '.pageSizeBtn', function () {
    pageSize = parseInt($(this).data('value'));
    pageNumber = 1; // Reset to first page
    getMyVideos();
});

$(document).on('click', '.sortBy', function () {
    const sortValue = $(this).data('value');

    // Toggle between ascending and descending
    if (sortBy === `${sortValue}-a`) {
        sortBy = `${sortValue}-d`;
    } else {
        sortBy = `${sortValue}-a`;
    }

    // Update sort indicators
    $('.sortBy span').text('');
    $(this).find('span').text(sortBy.endsWith('-a') ? '↑' : '↓');

    getMyVideos();
});