let pageNumber = 1;
let pageSize = 10;
let searchBy = "";
let categoryId = 0;

function hideLoading() {
    $('#loadingIndicator').hide();
}

function showError(message) {
   
    alert(message); 
}
$(function () {

    const currentPage = '@Model.Page'?.toLowerCase() || 'home';


    $('[data-page]').removeClass('active');
    $(`[data-page="${currentPage}"]`).addClass('active');


    if (currentPage === 'home') {
        loadVideos();
    } else {
        console.log(`Loading ${currentPage} content...`);

    }
});
function loadVideos() {
    $('#loadingIndicator').show();

    const parameters = {
        pageNumber: pageNumber,
        pageSize: pageSize,
        searchBy: searchBy,
        categoryId: categoryId
    };

    console.log('Loading videos with parameters:', parameters);

    $.ajax({
        url: "/Home/GetVideosForHomeGrid",
        type: "GET",
        data: parameters,
        success: function (response) {
            hideLoading();
            if (response.isSuccess) {
                const result = response.result;
                $('#videosTableBody').empty();
                $('#paginationSummery').empty();
                $('#paginationBtnGroup').empty();
                $('#itemsPerPageDisplay').empty();

                DisplayVideos(result.items);

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

    
    const startPage = Math.max(1, currentPage - 2);
    const endPage = Math.min(totalPages, currentPage + 2);

    for (let i = startPage; i <= endPage; i++) {
        buttons += `<button type="button" class="btn btn-sm paginationBtn 
                   ${i === currentPage ? 'btn-primary' : 'btn-secondary'}" 
                   data-value="${i}">${i}</button>`;
    }

  
    buttons += `<button type="button" class="btn btn-secondary btn-sm paginationBtn" 
               ${currentPage >= totalPages ? 'disabled' : ''} 
               data-value="${currentPage + 1}" 
               title="Next Page">
               <i class="bi bi-chevron-right"></i>
               </button>`;

    buttons += `<button type="button" class="btn btn-secondary btn-sm paginationBtn" 
               ${currentPage >= totalPages ? 'disabled' : ''} 
               data-value="${totalPages}" 
               title="Last Page">
               <i class="bi bi-chevron-bar-right"></i>
               </button>`;

    $('#paginationBtnGroup').html(buttons);

   
    $('.paginationBtn').off('click').on('click', function () {
        pageNumber = parseInt($(this).data('value'));
        loadVideos(); 
    });
}

function DisplayVideos(videos) {
    let divTag = '';
    let rowOpen = false;

    if (videos.length > 0) {
        videos.forEach((v, index) => {
            if (index % 4 === 0) {
                if (rowOpen) divTag += '</div>';
                divTag += '<div class="row">';
                rowOpen = true;
            }

            divTag += `<div class="col-xl-3 col-md-6 pt-2">
                <div class="p-2 border rounded text-center">
                    <div><a href="/Video/Watch/${v.id}"><img src="${v.thumbnailUrl}" class="rounded preview-image" /></a></div>
                    <a href="/Video/Watch/${v.id}" class="text-danger-emphasis" style="text-decoration: none;">${v.title}</a>
                    <div><span style="font-size: small">
                        <a href="/Member/Channel/${v.channelId}" style="text-decoration: none;" class="text-primary">${v.channelName}</a> <br />
                        10K Views - 2 Years Ago</span>
                    </div>
                </div>
            </div>`;

            if ((index + 1) % 4 === 0) {
                divTag += '</div>';
                rowOpen = false;
            }
        });

        if (rowOpen) divTag += '</div>';
    } else {
        divTag = '<div class="row"><div class="col text-center">No Videos Found</div></div>';
    }

    $('#videosTableBody').append(divTag);
}

// Initialize on page load
$(document).ready(function () {
    loadVideos();
});