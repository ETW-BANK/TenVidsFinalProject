$('#imageUpload').on('change', function () {
    if (this.files && this.files[0]) {
        $('#previewImage').empty();
        var reader = new FileReader();

        reader.onload = function (event) {
            $('#previewImage').append(
                $('<img>').attr({
                    'src': event.target.result,
                    'class': 'card-img-top rounded preview-image'
                   
                })
            );
        };

        reader.readAsDataURL(this.files[0]);
    }
});