$(document).ready(function () {

    const videoId = $('#videoId').val();
    const channelId = $('#channelId').val();

    const commentTextarea = $('#textComment');
    const commentButtons = $('#sectionCommentBtns');

    commentButtons.hide();

    commentTextarea.on('input', function () {
        commentButtons.toggle($(this).val().trim().length > 0);
    });

    $('#btnCancel').on('click', function (e) {
        e.preventDefault();
        commentTextarea.val('');
        commentButtons.hide();
    });

    // Subscribe button functionality
    $('#subscribeBtn').on('click', function (e) {
        e.preventDefault();
        var btn = $(this);

        $.ajax({
            url: "/Video/SubscribeChannel",
            type: "PUT",
            data: { channelId: channelId },
            xhrFields: {
                withCredentials: true // ✅ SEND cookies
            },
            success: function (data) {
                if (data.title === "Unsubscribed") {
                    btn.removeClass('btn-success')
                        .addClass('btn-danger')
                        .text('Subscribe');
                    toastr.success(data.message);
                } else if (data.title === "Subscribed") {
                    btn.removeClass('btn-danger')
                        .addClass('btn-success')
                        .text('Subscribed ✓');
                    toastr.success(data.message);
                } else {
                    toastr.error(data.message);
                }
            },
            error: function (xhr, status, error) {
                if (xhr.status === 401) {
                    const returnUrl = encodeURIComponent(window.location.pathname + window.location.search);
                    window.location.href = `/Account/Login?returnurl=${returnUrl}`;
                } else {
                    console.error("Subscription error:", error);
                    toastr.error("Failed to update subscription");
                }
            }
        });
    }); // <-- Missing closing parenthesis here

    // Like/Dislike functionality
    window.likeDislike = function (id, action) {
        const likeBtn = $('#likeBtn');
        const dislikeBtn = $('#dislikeBtn');
        const likeCountSpan = likeBtn.find('span:last');
        const dislikeCountSpan = dislikeBtn.find('span:last');

        // Get current counts (handle formatted numbers)
        let likeCount = parseInt(likeCountSpan.text().replace(/,/g, '')) || 0;
        let dislikeCount = parseInt(dislikeCountSpan.text().replace(/,/g, '')) || 0;

        // Determine current active state
        const isLiked = likeBtn.hasClass('active');
        const isDisliked = dislikeBtn.hasClass('active');

        // Calculate new counts
        let newLikeCount = likeCount;
        let newDislikeCount = dislikeCount;

        if (action === 'like') {
            newLikeCount = isLiked ? likeCount - 1 : likeCount + 1;
            if (!isLiked && isDisliked) newDislikeCount = dislikeCount - 1;
        } else { // dislike
            newDislikeCount = isDisliked ? dislikeCount - 1 : dislikeCount + 1;
            if (!isDisliked && isLiked) newLikeCount = likeCount - 1;
        }

        // Optimistic UI update
        likeBtn.toggleClass('active', action === 'like' && !isLiked);
        dislikeBtn.toggleClass('active', action === 'dislike' && !isDisliked);
        likeCountSpan.text(newLikeCount.toLocaleString());
        dislikeCountSpan.text(newDislikeCount.toLocaleString());

        // Disable during request
        likeBtn.add(dislikeBtn).css('pointer-events', 'none');

        $.ajax({
            url: "/Video/LikeVideo",
            type: "PUT",
            data: { videoId: videoId, action: action },
            success: function (data) {
                if (!data.success) revertUI();
            },
            error: function () {
                revertUI();
                toastr.error("Failed to update reaction");
            },
            complete: function () {
                likeBtn.add(dislikeBtn).css('pointer-events', 'auto');
            }
        });

        function revertUI() {
            likeBtn.toggleClass('active', isLiked);
            dislikeBtn.toggleClass('active', isDisliked);
            likeCountSpan.text(likeCount.toLocaleString());
            dislikeCountSpan.text(dislikeCount.toLocaleString());
        }
    };
});
