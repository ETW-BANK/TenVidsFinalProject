// wwwroot/js/videoInteractions.js
$(document).ready(function () {
    // Get IDs from hidden inputs
    const videoId = $('#videoId').val();
    const channelId = $('#channelId').val();

    // Subscribe button functionality
    $('#subscribeBtn').on('click', function (e) {
        e.preventDefault();
        var btn = $(this);

        $.ajax({
            url: "/Video/SubscribeChannel",
            type: "PUT",
            data: { channelId: channelId },
            success: function (data) {
                if (data.title === "Unsubscribed") {
                    btn.removeClass('btn-success');
                    btn.addClass('btn-danger');
                    btn.text('Subscribe');
                    toastr.success(data.message);
                } else if (data.title === "Subscribed") {
                    btn.removeClass('btn-danger');
                    btn.addClass('btn-success');
                    btn.text('Subscribed ✓');
                    toastr.success(data.message);
                } else {
                    toastr.error(data.message);
                }
            },
            error: function (xhr, status, error) {
                console.error("Subscription error:", error);
                toastr.error("Failed to update subscription");
            }
        });
    });

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

        // Calculate new counts based on action
        let newLikeCount = likeCount;
        let newDislikeCount = dislikeCount;

        if (action === 'like') {
            if (isLiked) {
                // Already liked - removing like
                newLikeCount = likeCount - 1;
            } else {
                // Not liked - adding like
                newLikeCount = likeCount + 1;
                if (isDisliked) {
                    // Remove existing dislike
                    newDislikeCount = dislikeCount - 1;
                }
            }
        } else { // dislike
            if (isDisliked) {
                // Already disliked - removing dislike
                newDislikeCount = dislikeCount - 1;
            } else {
                // Not disliked - adding dislike
                newDislikeCount = dislikeCount + 1;
                if (isLiked) {
                    // Remove existing like
                    newLikeCount = likeCount - 1;
                }
            }
        }

        // Optimistically update UI
        likeBtn.toggleClass('active', action === 'like' && !isLiked);
        dislikeBtn.toggleClass('active', action === 'dislike' && !isDisliked);

        // Update counts
        likeCountSpan.text(newLikeCount.toLocaleString());
        dislikeCountSpan.text(newDislikeCount.toLocaleString());

        // Disable buttons during request
        likeBtn.css('pointer-events', 'none');
        dislikeBtn.css('pointer-events', 'none');

        $.ajax({
            url: "/Video/LikeVideo",
            type: "PUT",
            data: {
                videoId: videoId,
                action: action
            },
            success: function (data) {
                if (!data.success) {
                    // Revert UI if server failed
                    likeBtn.toggleClass('active', isLiked);
                    dislikeBtn.toggleClass('active', isDisliked);
                    likeCountSpan.text(likeCount.toLocaleString());
                    dislikeCountSpan.text(dislikeCount.toLocaleString());
                    toastr.error(data.message);
                }
            },
            error: function (xhr) {
                // Revert UI on error
                likeBtn.toggleClass('active', isLiked);
                dislikeBtn.toggleClass('active', isDisliked);
                likeCountSpan.text(likeCount.toLocaleString());
                dislikeCountSpan.text(dislikeCount.toLocaleString());
                toastr.error(xhr.responseJSON?.message || "Failed to update reaction");
            },
            complete: function () {
                // Re-enable buttons
                likeBtn.css('pointer-events', 'auto');
                dislikeBtn.css('pointer-events', 'auto');
            }
        });
    };
});