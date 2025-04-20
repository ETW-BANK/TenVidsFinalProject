
//global js
function formatView(views) {
    if (views >= 1000000) {
        return Math.floor(views / 1000000) + " M views";
    } else if (views >= 1000) {
        return Math.floor(views / 1000) + " K views";
    } else {
        return views + (views > 1 ? " views" : " view");
    }
}

function timeAgo(dateString) {
    const date = new Date(dateString);
    if (isNaN(date.getTime())) {
        console.error("Invalid date:", dateString);
        return "Some time ago";
    }

    const now = new Date();
    const seconds = Math.floor((now - date) / 1000);

    const intervals = {
        year: 31536000,
        month: 2592000,
        week: 604800,
        day: 86400,
        hour: 3600,
        minute: 60
    };

    if (seconds < intervals.minute) {
        return "Just now";
    }

    for (const [unit, secondsInUnit] of Object.entries(intervals)) {
        const interval = Math.floor(seconds / secondsInUnit);
        if (interval >= 1) {
            return `${interval} ${unit}${interval === 1 ? '' : 's'} ago`;
        }
    }

    return "Just now";
}