$(function () {
    if ($('#videoId').val() > 0) {
        // display preview image
        $('#videoUpload').prop("disabled", true);
        const url = $('#ImageUrl').val();  // Fixed: Changed 'imageUrl' to 'ImageUrl' to match your HTML
        $('#previewImage').append(`<img src="${url}" class="card-img-top rounded preview-image" />`);
    }
});

$('#imageUpload').on('change', function () {
    if (this.files && this.files[0]) {  // Added check for files[0] to prevent errors
        $('#previewImage').empty();
        var reader = new FileReader();
        reader.onload = function (event) {
            $('#previewImage').append(`<img src="${event.target.result}" class="card-img-top rounded preview-image" />`);
        };
        reader.readAsDataURL(this.files[0]);
    }
});