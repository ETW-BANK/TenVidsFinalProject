
let pageNumber = 1;
let pageSize = 5;
let sortBy='';
$(function () {
  
    $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
        localStorage.setItem('activeTab', $(e.target).attr('href'));
    });

    var activeTab = localStorage.getItem('activeTab');
    if (activeTab) {
        $('a[href="' + activeTab + '"]').tab('show');
    } else {
        $('a[href="#myvideos"]').tab('show');
    }
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

    $('#profile_name').val(originalName).prop('disabled', true);
    $('#profile_discription').val(originalDescription).prop('disabled', true);

  
    $('#profile_name-error').empty();
    $('#profile_discription-error').empty();
    $('#profile_name').removeClass('input-validation-error');
    $('#profile_discription').removeClass('input-validation-error');

   
    $('#divEditButton').show();
    $('#updateProfileButtons').hide();
}

function getMyVideos() {
    const parameters = {
        pageNumber,
        pageSize,
        sortBy
    };

    $.ajax({
        type: 'GET',
        url: '/Video/GetVideosForChannelGrid',
        data: parameters,
        success: function (response) {
            console.log(response);
            const result = response.result;

            // Clear previous content
            $('#videosTableBody').empty();
            $('#paginationSummery').empty();
            $('#paginationBtnGroup').empty();
            $('#itemsPerPageDisplay').empty();

            // Populate table with videos
            populateVideoTableBody(result.items);

            if (result.totalCount > 0) {
                // Update items per page display
                $('#itemsPerPageDisplay').text(pageSize);

             
                const from = (result.pageNumber - 1) * result.pageSize + 1;
                const to = Math.min(result.pageNumber * result.pageSize, result.totalCount);
                $('#paginationSummery').text(`${from}-${to} of ${result.totalCount}`);

                
                buildPaginationButtons(result);
            } else {
                $('#itemsPerPageDropdown').hide();
            }
        },
        error: function (xhr) {
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

    // Page number buttons (show up to 5 pages around current)
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

    
    $('.paginationBtn').off('click').on('click', function () {
        pageNumber = parseInt($(this).data('value'));
        getMyVideos();
    });
}

// Initialize page size dropdown
$('.pageSizeBtn').on('click', function () {
    pageSize = parseInt($(this).data('value'));
    pageNumber = 1;
    getMyVideos();
});

// Initialize sorting
$('.sortBy').on('click', function () {
    const sortValue = $(this).data('value');

    // Toggle between ascending and descending
    if (sortBy === `${sortValue}-a`) {
        sortBy = `${sortValue}-d`;
    } else {
        sortBy = `${sortValue}-a`;
    }

    $('.sortBy span').text('');
    $(this).find('span').text(sortBy.endsWith('-a') ? '↑' : '↓');

    getMyVideos();
});

// Your existing populateVideoTableBody function remains the same
function populateVideoTableBody(videos) {
    let trTag = '';

    if (videos.length > 0) {
        videos.map((v, index) => {
            trTag += '<tr>';
            trTag += `<td>${index + 1}</td>`;
            trTag += `<td><a href="/Video/Watch/${v.id}"><img src="${v.thumbnailUrl}" class="card-img-top rounded preview-image" /></a></td>`;
            trTag += `<td><p><a href="/Video/Watch/${v.id}" class="text-dark">${v.title}</a></p></td>`;
            trTag += `<td>${new Date(v.createdAt).toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' })}</td>`;
            trTag += `<td>${v.views.toLocaleString()}</td>`;
            trTag += `<td>${v.comments.toLocaleString()}</td>`;
            trTag += `<td>${v.likes.toLocaleString()}</td>`;
            trTag += `<td>${v.dislikes.toLocaleString()}</td>`;
            trTag += `<td>${v.categoryName}</td>`;
            trTag += '<td>';
            trTag += `<a href="/Video/Upsert/${v.id}" class="btn btn-warning text-white me-2"><i class="bi bi-pencil"></i></a>`;
            trTag += `<button class="btn btn-danger text-white me-2" onclick="deleteVideo(${v.id}, '${v.title}')"><i class="bi bi-trash3"></i></button>`;
            trTag += '</td>';
            trTag += '</tr>';
        });
    } else {
        trTag = '<tr><td colspan="10" class="text-center">You don\'t have any videos</td></tr>';
    }

    $('#videosTableBody').append(trTag);
}