
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